using System.Collections;
using UnityEngine;

namespace miao
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))] // �Զ�ȷ���и���
    public class PhysicsBody : MonoBehaviour
    {
        [Header("�������")]
        public float mass = 1f;
        public float bounciness = 0.5f;
        [Tooltip("�ٶ�˥��ϵ����0-1")]
        public float linearDamping = 0.1f;
        [Tooltip("��ת˥��ϵ����0-1")]
        public float angularDamping = 0.1f;
        [HideInInspector] public Vector3 angularVelocity;

        [Tooltip("������ײ�뾶����������ϵͳ")]
        public float radius = 0.5f;

        [Header("��������")]
        public bool useGravity = true;  // ��������������Ʒ��

        [Header("�ŵ׼���")]
        public Transform footPoint; // ������ײ���ĵײ���

        [HideInInspector] public Vector3 velocity;
        [HideInInspector] public bool isGrounded = false;

        private Vector3 accumulatedForce;
        private Vector3 accumulatedTorque; // ���ڼ�����ת����

        [HideInInspector] public Rigidbody rb;

        private void Awake()
        {
            // ��ȡ����
            rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.mass = mass;
                rb.drag = linearDamping;      // ������������ Rigidbody
                rb.angularDrag = angularDamping; // ���ٶ��������� Rigidbody
                rb.useGravity = false;        // �ر� Rigidbody �������� PhysicsSystem ����
            }
            else
            {
                Debug.LogError("[PhysicsBody] δ�ҵ� Rigidbody��");
            }
        }

        private void OnEnable()
        {
            StartCoroutine(WaitAndRegister());
        }

        private IEnumerator WaitAndRegister()
        {
            // �ȴ� PhysicsSystem ��ʼ��
            yield return new WaitUntil(() => PhysicsSystem.Instance != null);
            PhysicsSystem.Instance.RegisterBody(this);
        }

        private void OnDisable()
        {
            PhysicsSystem.Instance?.UnregisterBody(this);
        }

        public void ApplyForce(Vector3 force)
        {
            accumulatedForce += force;
        }
        public void ApplyTorque(Vector3 torque)
        {
            accumulatedTorque += torque; //  ʩ����ת����
        }

        public void PhysicsUpdate(float deltaTime)
        {
            // -------------------------
            // ʹ���ۼ������� Rigidbody
            // -------------------------
            if (rb != null)
            {
                // ���ٶ� = F / m
                Vector3 acceleration = accumulatedForce / mass;

                // -------------------------
                // �����˶�
                // -------------------------
                rb.velocity += acceleration * deltaTime;

                // -------------------------
                // ���˶�
                // -------------------------
                Vector3 angularAcceleration = accumulatedTorque / mass; // �򻯹���
                angularVelocity += angularAcceleration * deltaTime;
                rb.angularVelocity += angularVelocity * deltaTime;



                // -------------------------
                // ����ۼ���������
                // -------------------------
                accumulatedForce = Vector3.zero;
                accumulatedTorque = Vector3.zero;

                // ͬ�� velocity ���� PhysicsSystem ʹ��
                velocity = rb.velocity;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;

            Vector3 center = footPoint != null ? footPoint.position : transform.position;

            // �������ΰ뾶
            Gizmos.DrawWireSphere(center, radius);

            // �������µ������������ʹ�ã�
            if (useGravity && PhysicsSystem.Instance != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(center, center + PhysicsSystem.Instance.gravityDirection.normalized * radius);
            }
        }
    }
}
