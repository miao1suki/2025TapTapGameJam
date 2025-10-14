using UnityEngine;
using System.Collections.Generic;

namespace miao
{
    public class PhysicsSystem : MonoBehaviour
    {
        public static PhysicsSystem Instance;

        [Header("ȫ����������")]
        public bool GlobalGravity = true;//ȫ��ʹ������
        public Vector3 gravityDirection = Vector3.down;//��������
        public float gravityStrength = 9.81f;//������С

        [SerializeField]
        private List<PhysicsBody> bodies = new List<PhysicsBody>(); // ��̬������
        [SerializeField]
        private List<Collider> staticColliders = new List<Collider>(); // ��̬�������ײ��

        private void Awake()
        {
            Instance = this;
            // �Զ��ռ����������о�̬��ײ�壨û�� PhysicsBody �� Collider��
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
                    // Ӧ������
                    Vector3 gravityForce = gravityDirection.normalized * gravityStrength * body.mass;
                    body.ApplyForce(gravityForce);
                }
            }

            // ����ÿ�������λ��
            foreach (var body in bodies)
                body.PhysicsUpdate(Time.fixedDeltaTime);

            // ������ײ�붯������
            ResolveDynamicCollisions();

            // ���㶯̬�����뾲̬������ײ
            ResolveStaticCollisions();
        }

        // -----------------------------------------------
        // ��̬����֮��Ķ���������ײ
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

                        // �����غ�
                        Vector3 relativeVelocity = b.velocity - a.velocity;
                        float velAlongNormal = Vector3.Dot(relativeVelocity, normal);
                        if (velAlongNormal > 0) continue;

                        float e = Mathf.Min(a.bounciness, b.bounciness);
                        float impulseMag = -(1 + e) * velAlongNormal;
                        impulseMag /= (1 / a.mass + 1 / b.mass);
                        Vector3 impulse = impulseMag * normal;

                        a.velocity -= impulse / a.mass;
                        b.velocity += impulse / b.mass;

                        //  ʩ����ת����
                        Vector3 contactPoint = (a.transform.position + b.transform.position) / 2f;
                        Vector3 rA = contactPoint - a.transform.position;
                        Vector3 rB = contactPoint - b.transform.position;
                        a.ApplyTorque(Vector3.Cross(rA, -impulse)); // a ������
                        b.ApplyTorque(Vector3.Cross(rB, impulse));  // b ������

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
        // ��̬�����뾲̬������ײ
        // -----------------------------------------------
        private void ResolveStaticCollisions()
        {
            foreach (var body in bodies)
            {
                body.isGrounded = false; // ÿ֡�ȼ������岻�ڵ���

                foreach (var col in staticColliders)
                {
                    if (!col.enabled) continue;

                    // ʹ�ýŵ׵���
                    Vector3 contactPoint = body.footPoint != null ? body.footPoint.position : body.transform.position;

                    Vector3 closest = col.ClosestPoint(contactPoint);
                    Vector3 delta = contactPoint - closest;
                    float dist = delta.magnitude;

                    if (dist < body.radius)
                    {
                        Vector3 normal = delta.sqrMagnitude > 0 ? delta.normalized : Vector3.up;

                        // -----------------------------------------------------
                        // �ж���ײ��������������ļнǣ��������ֵ�����ǽ��
                        // -----------------------------------------------------
                        float dot = Vector3.Dot(normal, -gravityDirection.normalized);

                        float vDot = Vector3.Dot(body.velocity, normal);
                        if (vDot < 0)
                        {
                            if (dot > 0.5f)
                            {
                                // �����������෴��������棩�� ��ǵ���Ӵ�
                                body.isGrounded = true;
                            }
                            else
                            {
                                // �ǵ��棨ǽ��б�£��� ��������
                                body.velocity -= (1 + body.bounciness) * vDot * normal;

                                // ʩ����ת����
                                Vector3 r = contactPoint - body.transform.position;
                                body.ApplyTorque(Vector3.Cross(r, -vDot * normal));
                            }
                        }

                        // ��ֹ�������ݣ�������͸
                        float penetration = body.radius - dist;
                        body.transform.position += normal * penetration;
                    }
                }

                // �������վ�ȣ�����������ʩ�ӷ���֧��������ֹ����
                if (body.isGrounded)
                {
                    Vector3 gravityDirNorm = gravityDirection.normalized;
                    // ֧���� = ��������
                    Vector3 supportForce = -gravityDirNorm * gravityStrength * body.mass;
                    body.ApplyForce(supportForce);
                }
            }
        }
    }
}
