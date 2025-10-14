using UnityEngine;
using System.Collections.Generic;

namespace miao
{
    public class PhysicsSystem : MonoBehaviour
    {
        public static PhysicsSystem Instance;

        [Header("全局重力设置")]
        public bool GlobalGravity = true;//全局使用重力
        public Vector3 gravityDirection = Vector3.down;//重力方向
        public float gravityStrength = 9.81f;//重力大小

        [SerializeField]
        private List<PhysicsBody> bodies = new List<PhysicsBody>(); // 动态物理体
        [SerializeField]
        private List<Collider> staticColliders = new List<Collider>(); // 静态地面等碰撞体

        private void Awake()
        {
            Instance = this;
            // 自动收集场景中所有静态碰撞体（没有 PhysicsBody 的 Collider）
            foreach (var col in FindObjectsOfType<Collider>())
            {
                if (col.enabled && col.GetComponent<PhysicsBody>() == null)
                    staticColliders.Add(col);
            }
        }

        public void RegisterBody(PhysicsBody body)
        {
            if (!bodies.Contains(body))
                bodies.Add(body);
        }

        public void UnregisterBody(PhysicsBody body)
        {
            if (bodies.Contains(body))
                bodies.Remove(body);
        }

        private void FixedUpdate()
        {
            foreach (var body in bodies)
            {
                if (GlobalGravity && body.useGravity)
                {
                    // 应用重力
                    Vector3 gravityForce = gravityDirection.normalized * gravityStrength * body.mass;
                    body.ApplyForce(gravityForce);
                }
            }

            // 更新每个物体的位置
            foreach (var body in bodies)
                body.PhysicsUpdate(Time.fixedDeltaTime);

            // 计算碰撞与动量交换
            ResolveDynamicCollisions();

            // 计算动态物体与静态地面碰撞
            ResolveStaticCollisions();
        }

        // -----------------------------------------------
        // 动态物体之间的动量交换碰撞
        // -----------------------------------------------
        private void ResolveDynamicCollisions()
        {
            for (int i = 0; i < bodies.Count; i++)
            {
                for (int j = i + 1; j < bodies.Count; j++)
                {
                    PhysicsBody a = bodies[i];
                    PhysicsBody b = bodies[j];

                    Vector3 delta = b.transform.position - a.transform.position;
                    float dist = delta.magnitude;
                    float minDist = a.radius + b.radius;

                    if (dist < minDist)
                    {
                        Vector3 normal = delta.normalized;

                        // 动量守恒
                        Vector3 relativeVelocity = b.velocity - a.velocity;
                        float velAlongNormal = Vector3.Dot(relativeVelocity, normal);
                        if (velAlongNormal > 0) continue;

                        float e = Mathf.Min(a.bounciness, b.bounciness);
                        float impulseMag = -(1 + e) * velAlongNormal;
                        impulseMag /= (1 / a.mass + 1 / b.mass);
                        Vector3 impulse = impulseMag * normal;

                        a.velocity -= impulse / a.mass;
                        b.velocity += impulse / b.mass;

                        //  施加旋转力矩
                        Vector3 contactPoint = (a.transform.position + b.transform.position) / 2f;
                        Vector3 rA = contactPoint - a.transform.position;
                        Vector3 rB = contactPoint - b.transform.position;
                        a.ApplyTorque(Vector3.Cross(rA, -impulse)); // a 受力矩
                        b.ApplyTorque(Vector3.Cross(rB, impulse));  // b 受力矩

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
        // 动态物体与静态地面碰撞
        // -----------------------------------------------
        private void ResolveStaticCollisions()
        {
            foreach (var body in bodies)
            {
                body.isGrounded = false; // 每帧先假设物体不在地面

                foreach (var col in staticColliders)
                {
                    if (!col.enabled) continue;

                    // 使用脚底点检测
                    Vector3 contactPoint = body.footPoint != null ? body.footPoint.position : body.transform.position;

                    Vector3 closest = col.ClosestPoint(contactPoint);
                    Vector3 delta = contactPoint - closest;
                    float dist = delta.magnitude;

                    if (dist < body.radius)
                    {
                        Vector3 normal = delta.sqrMagnitude > 0 ? delta.normalized : Vector3.up;

                        // -----------------------------------------------------
                        // 判断碰撞法线与重力方向的夹角，用于区分地面与墙面
                        // -----------------------------------------------------
                        float dot = Vector3.Dot(normal, -gravityDirection.normalized);

                        float vDot = Vector3.Dot(body.velocity, normal);
                        if (vDot < 0)
                        {
                            if (dot > 0.5f)
                            {
                                // 与重力方向相反（例如地面）→ 标记地面接触
                                body.isGrounded = true;
                            }
                            else
                            {
                                // 非地面（墙或斜坡）→ 正常反弹
                                body.velocity -= (1 + body.bounciness) * vDot * normal;

                                // 施加旋转力矩
                                Vector3 r = contactPoint - body.transform.position;
                                body.ApplyTorque(Vector3.Cross(r, -vDot * normal));
                            }
                        }

                        // 防止继续下陷：修正穿透
                        float penetration = body.radius - dist;
                        body.transform.position += normal * penetration;
                    }
                }

                // 如果物体站稳，沿重力方向施加反向支持力，防止下陷
                if (body.isGrounded)
                {
                    Vector3 gravityDirNorm = gravityDirection.normalized;
                    // 支撑力 = 抵消重力
                    Vector3 supportForce = -gravityDirNorm * gravityStrength * body.mass;
                    body.ApplyForce(supportForce);
                }
            }
        }
    }
}
