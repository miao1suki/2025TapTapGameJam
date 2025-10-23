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

        [Header("ȫ����������")]
        public bool GlobalGravity = true; // ȫ��ʹ������
        public Vector3 gravityDirection = Vector3.down; // ��������
        public float gravityStrength = 9.81f; // ������С

        [SerializeField]
        private List<PhysicsBody> bodies = new List<PhysicsBody>(); // ��̬������
        [SerializeField]
        private List<Collider> staticColliders = new List<Collider>(); // ��̬�������ײ��

        private List<Task> activeTasks = new List<Task>(); // ��ǰ���е������б�
        private CancellationTokenSource cancellationTokenSource; // ��������ȡ��

        // ------------------------------
        // 1. Awake ��ʼ��ʵ���ͳ����¼�
        // ------------------------------
        private void Awake()
        {
            Instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
            cancellationTokenSource = new CancellationTokenSource(); // ��ʼ��ȡ�����
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // �����л�ʱ��վ�̬����
            staticColliders.Clear();
            GatherStaticColliders(); // �����ռ���̬��ײ��
        }

        private void GatherStaticColliders()
        {
            // �Զ��ռ����������о�̬��ײ�壨û�� PhysicsBody �� Collider��
            foreach (var col in FindObjectsOfType<Collider>())
            {
                if (col.enabled && !col.isTrigger && col.GetComponent<PhysicsBody>() == null)
                    staticColliders.Add(col);
            }
        }

        private void ClearPhysicsBodies()
        {
            // ��վ�̬�Ͷ�̬�����б�
            bodies.Clear();
            staticColliders.Clear();
        }

        public void RegisterBody(PhysicsBody body)
        {
            if (!bodies.Contains(body))
            {
                body.ClearForces(); // ��� accumulatedForce/accumulatedTorque
                bodies.Add(body);
            }
        }

        public void UnregisterBody(PhysicsBody body)
        {
            if (bodies.Contains(body))
                bodies.Remove(body);
        }

        // ------------------------------
        // 2. FixedUpdate�����߳̿�������
        // ------------------------------
        private void FixedUpdate()
        {
            // ------------------------------
            // 2.1 ��ȡ deltaTime���������̣߳�
            // ------------------------------
            float deltaTime = Time.fixedDeltaTime;

            // ------------------------------
            // 2.2 ��ѧ������̴߳������������ٶ�/��/��ת���أ�
            // ------------------------------
            int batchSize = 8; // ÿ���������������

            // �����Ѿ���ɵ�����
            activeTasks.RemoveAll(task => task.IsCompleted);

            for (int i = 0; i < bodies.Count; i += batchSize)
            {
                int start = i;
                int end = Mathf.Min(i + batchSize, bodies.Count);

                // ʹ���̳߳����������񣬶�����ÿ�ζ����������� Task
                var task = Task.Run(() => ComputeBatch(start, end, deltaTime, cancellationTokenSource.Token));
                activeTasks.Add(task);
            }

            // �ȴ��������
            Task.WhenAll(activeTasks).ContinueWith(_ =>
            {
                // ���������
                activeTasks.Clear();
            });

            // ------------------------------
            // 2.3 ���̴߳��� Transform ���º���ײ
            // ------------------------------
            // ����λ�ú���ת��Transform ֻ�����̲߳�����
            foreach (var body in bodies)
            {
                if (!body.isActive) continue;
                body.PhysicsUpdate(deltaTime);
            }

            // ��̬�������ײ���漰 Transform �޸ģ�
            ResolveDynamicCollisions();

            // ------------------------------
            // �ؼ��Ż�����̬������ײ
            // ------------------------------
            ResolveStaticCollisions();
        }

        // ------------------------------
        // 3. ���߳������������ѧ����ѧ���֣�
        // ------------------------------
        private void ComputeBatch(int start, int end, float deltaTime, CancellationToken token)
        {
            for (int i = start; i < end; i++)
            {
                if (token.IsCancellationRequested) break;

                PhysicsBody body = bodies[i];

                if (!body.isActive) continue;

                // Ӧ��ȫ����������ѧ���㣬���� Transform��
                if (GlobalGravity && body.useGravity)
                {
                    Vector3 gravityForce = gravityDirection.normalized * gravityStrength * body.mass;
                    body.ApplyForce(gravityForce);
                }
            }
        }

        // -----------------------------------------------
        // ��̬����䶯��������ײ�����̣߳�
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

                    // ʹ������뾶����������ײ
                    Vector3 delta = b.transform.position - a.transform.position;
                    float dist = delta.magnitude;
                    float minDist = a.radius + b.radius;

                    if (dist < minDist)
                    {
                        Vector3 normal = delta.normalized;

                        // �����غ����
                        Vector3 relativeVelocity = b.velocity - a.velocity;
                        float velAlongNormal = Vector3.Dot(relativeVelocity, normal);
                        if (velAlongNormal > 0) continue;

                        float e = Mathf.Min(a.bounciness, b.bounciness);
                        float impulseMag = -(1 + e) * velAlongNormal;
                        impulseMag /= (1 / a.mass + 1 / b.mass);
                        Vector3 impulse = impulseMag * normal;

                        a.velocity -= impulse / a.mass;
                        b.velocity += impulse / b.mass;

                        // ʩ����ת���أ���ѡ���򻯹��ԣ�
                        Vector3 contactPoint = (a.transform.position + b.transform.position) / 2f;
                        Vector3 rA = contactPoint - a.transform.position;
                        Vector3 rB = contactPoint - b.transform.position;
                        a.ApplyTorque(Vector3.Cross(rA, -impulse));
                        b.ApplyTorque(Vector3.Cross(rB, impulse));

                        // ����͸����
                        float penetration = minDist - dist;
                        Vector3 correction = normal * (penetration / 2f);
                        a.transform.position -= correction;
                        b.transform.position += correction;
                    }
                }
            }
        }

        // -----------------------------------------------
        // ��̬������ײ�����̣߳�
        // -----------------------------------------------
        private void ResolveStaticCollisions()
        {
            // ��һ�������� null Collider������ÿ֡ѭ���жϿ���
            staticColliders.RemoveAll(col => col == null);

            foreach (var body in bodies)
            {
                body.isGrounded = false; // ÿ֡�������岻�ڵ���

                if (!body.isActive) continue;

                // ��̬������ײ���
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
                                // ��ǵ���Ӵ�
                                body.isGrounded = true;
                            }
                            else
                            {
                                // �ǵ��� �� ��������
                                body.velocity -= (1 + body.bounciness) * vDot * normal;
                                Vector3 r = contactPoint - body.transform.position;
                                body.ApplyTorque(Vector3.Cross(r, -vDot * normal));
                            }
                        }

                        // ��ֹ��������
                        float penetration = body.radius - dist;
                        body.transform.position += normal * penetration;
                    }
                }

                // �������վ�ȣ�ʩ�ӷ���֧����
                if (body.isGrounded)
                {
                    Vector3 gravityDirNorm = gravityDirection.normalized;
                    Vector3 supportForce = -gravityDirNorm * gravityStrength * body.mass;
                    body.ApplyForce(supportForce);
                }
            }
        }

        // ------------------------------
        // ��Ϸ�˳�ʱ����
        // ------------------------------
        public void GameExit()
        {
            // ȡ����������
            cancellationTokenSource.Cancel();
            Task.WhenAll(activeTasks).ContinueWith(_ =>
            {
                activeTasks.Clear();
                GC.Collect(); // ǿ��ִ����������
            });
        }


    }
}
