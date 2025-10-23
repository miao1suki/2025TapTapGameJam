using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using System.Threading;
using UnityEditor;
using System;

namespace miao
{
    public class PhysicsSystem : MonoBehaviour
    {
        public static PhysicsSystem Instance;

        [Header("全局重力设置")]
        public bool GlobalGravity = true; // 全局使用重力
        public Vector3 gravityDirection = Vector3.down; // 重力方向
        public float gravityStrength = 9.81f; // 重力大小

        [SerializeField]
        private List<PhysicsBody> bodies = new List<PhysicsBody>(); // 动态物理体
        [SerializeField]
        private List<Collider> staticColliders = new List<Collider>(); // 静态地面等碰撞体

        private List<Task> activeTasks = new List<Task>(); // 当前运行的任务列表
        private CancellationTokenSource cancellationTokenSource; // 用于任务取消

        // ------------------------------
        // 1. Awake 初始化实例和场景事件
        // ------------------------------
        private void Awake()
        {
            Instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
            cancellationTokenSource = new CancellationTokenSource(); // 初始化取消标记
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // 场景切换时清空静态物体
            staticColliders.Clear();
            GatherStaticColliders(); // 重新收集静态碰撞体
        }

        private void GatherStaticColliders()
        {
            // 自动收集场景中所有静态碰撞体（没有 PhysicsBody 的 Collider）
            foreach (var col in FindObjectsOfType<Collider>())
            {
                if (col.enabled && !col.isTrigger && col.GetComponent<PhysicsBody>() == null)
                    staticColliders.Add(col);
            }
        }

        private void ClearPhysicsBodies()
        {
            // 清空静态和动态物体列表
            bodies.Clear();
            staticColliders.Clear();
        }

        public void RegisterBody(PhysicsBody body)
        {
            if (!bodies.Contains(body))
            {
                body.ClearForces(); // 清空 accumulatedForce/accumulatedTorque
                bodies.Add(body);
            }
        }

        public void UnregisterBody(PhysicsBody body)
        {
            if (bodies.Contains(body))
                bodies.Remove(body);
        }

        // ------------------------------
        // 2. FixedUpdate：主线程控制流程
        // ------------------------------
        private void FixedUpdate()
        {
            // ------------------------------
            // 2.1 获取 deltaTime（必须主线程）
            // ------------------------------
            float deltaTime = Time.fixedDeltaTime;

            // ------------------------------
            // 2.2 力学计算多线程处理（批量更新速度/力/旋转力矩）
            // ------------------------------
            int batchSize = 8; // 每批处理的物体数量

            // 清理已经完成的任务
            activeTasks.RemoveAll(task => task.IsCompleted);

            for (int i = 0; i < bodies.Count; i += batchSize)
            {
                int start = i;
                int end = Mathf.Min(i + batchSize, bodies.Count);

                // 使用线程池来管理任务，而不是每次都创建独立的 Task
                var task = Task.Run(() => ComputeBatch(start, end, deltaTime, cancellationTokenSource.Token));
                activeTasks.Add(task);
            }

            // 等待任务完成
            Task.WhenAll(activeTasks).ContinueWith(_ =>
            {
                // 清理任务池
                activeTasks.Clear();
            });

            // ------------------------------
            // 2.3 主线程处理 Transform 更新和碰撞
            // ------------------------------
            // 更新位置和旋转（Transform 只能主线程操作）
            foreach (var body in bodies)
            {
                if (!body.isActive) continue;
                body.PhysicsUpdate(deltaTime);
            }

            // 动态物体间碰撞（涉及 Transform 修改）
            ResolveDynamicCollisions();

            // ------------------------------
            // 关键优化：静态地面碰撞
            // ------------------------------
            ResolveStaticCollisions();
        }

        // ------------------------------
        // 3. 多线程批处理计算力学（数学部分）
        // ------------------------------
        private void ComputeBatch(int start, int end, float deltaTime, CancellationToken token)
        {
            for (int i = start; i < end; i++)
            {
                if (token.IsCancellationRequested) break;

                PhysicsBody body = bodies[i];

                if (!body.isActive) continue;

                // 应用全局重力（数学计算，无需 Transform）
                if (GlobalGravity && body.useGravity)
                {
                    Vector3 gravityForce = gravityDirection.normalized * gravityStrength * body.mass;
                    body.ApplyForce(gravityForce);
                }
            }
        }

        // -----------------------------------------------
        // 动态物体间动量交换碰撞（主线程）
        // -----------------------------------------------
        private void ResolveDynamicCollisions()
        {
            for (int i = 0; i < bodies.Count; i++)
            {
                for (int j = i + 1; j < bodies.Count; j++)
                {
                    PhysicsBody a = bodies[i];
                    PhysicsBody b = bodies[j];

                    if (!a.isActive || !b.isActive) continue;

                    // 使用自身半径处理球体碰撞
                    Vector3 delta = b.transform.position - a.transform.position;
                    float dist = delta.magnitude;
                    float minDist = a.radius + b.radius;

                    if (dist < minDist)
                    {
                        Vector3 normal = delta.normalized;

                        // 动量守恒计算
                        Vector3 relativeVelocity = b.velocity - a.velocity;
                        float velAlongNormal = Vector3.Dot(relativeVelocity, normal);
                        if (velAlongNormal > 0) continue;

                        float e = Mathf.Min(a.bounciness, b.bounciness);
                        float impulseMag = -(1 + e) * velAlongNormal;
                        impulseMag /= (1 / a.mass + 1 / b.mass);
                        Vector3 impulse = impulseMag * normal;

                        a.velocity -= impulse / a.mass;
                        b.velocity += impulse / b.mass;

                        // 施加旋转力矩（可选，简化惯性）
                        Vector3 contactPoint = (a.transform.position + b.transform.position) / 2f;
                        Vector3 rA = contactPoint - a.transform.position;
                        Vector3 rB = contactPoint - b.transform.position;
                        a.ApplyTorque(Vector3.Cross(rA, -impulse));
                        b.ApplyTorque(Vector3.Cross(rB, impulse));

                        // 防穿透修正
                        float penetration = minDist - dist;
                        Vector3 correction = normal * (penetration / 2f);
                        a.transform.position -= correction;
                        b.transform.position += correction;
                    }
                }
            }
        }

        // -----------------------------------------------
        // 静态地面碰撞（主线程）
        // -----------------------------------------------
        private void ResolveStaticCollisions()
        {
            // 先一次性清理 null Collider，减少每帧循环判断开销
            staticColliders.RemoveAll(col => col == null);

            foreach (var body in bodies)
            {
                body.isGrounded = false; // 每帧假设物体不在地面

                if (!body.isActive) continue;

                // 静态物体碰撞检测
                foreach (var col in staticColliders)
                {
                    if (!col.enabled) continue;

                    Vector3 contactPoint = body.footPoint != null ? body.footPoint.position : body.transform.position;
                    Vector3 closest = col.ClosestPoint(contactPoint);
                    Vector3 delta = contactPoint - closest;
                    float dist = delta.magnitude;

                    if (dist < body.radius)
                    {
                        Vector3 normal = delta.sqrMagnitude > 0 ? delta.normalized : Vector3.up;
                        float dot = Vector3.Dot(normal, -gravityDirection.normalized);
                        float vDot = Vector3.Dot(body.velocity, normal);

                        if (vDot < 0)
                        {
                            if (dot > 0.5f)
                            {
                                // 标记地面接触
                                body.isGrounded = true;
                            }
                            else
                            {
                                // 非地面 → 正常反弹
                                body.velocity -= (1 + body.bounciness) * vDot * normal;
                                Vector3 r = contactPoint - body.transform.position;
                                body.ApplyTorque(Vector3.Cross(r, -vDot * normal));
                            }
                        }

                        // 防止继续下陷
                        float penetration = body.radius - dist;
                        body.transform.position += normal * penetration;
                    }
                }

                // 如果物体站稳，施加反向支持力
                if (body.isGrounded)
                {
                    Vector3 gravityDirNorm = gravityDirection.normalized;
                    Vector3 supportForce = -gravityDirNorm * gravityStrength * body.mass;
                    body.ApplyForce(supportForce);
                }
            }
        }

        // ------------------------------
        // 游戏退出时清理
        // ------------------------------
        public void GameExit()
        {
            // 取消所有任务
            cancellationTokenSource.Cancel();
            Task.WhenAll(activeTasks).ContinueWith(_ =>
            {
                activeTasks.Clear();
                GC.Collect(); // 强制执行垃圾回收
            });
        }


    }
}
