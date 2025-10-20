using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace miao
{
    public enum BodyShape
    {
        Sphere,
        Box,
        Capsule
    }

    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class PhysicsBody : MonoBehaviour
    {
        public bool isActive { get; private set; } = false;

        [Header("�������")]
        public float mass = 1f;
        public float bounciness = 0.5f;
        [Tooltip("�ٶ�˥��ϵ����0-1")]
        public float linearDamping = 0.8f;
        [Tooltip("��ת˥��ϵ����0-1")]
        public float angularDamping = 0.8f;
        [HideInInspector] public Vector3 angularVelocity;

        [Header("��ײ����״")]
        public BodyShape shape = BodyShape.Sphere;
        [Tooltip("���ΰ뾶�����/���ҳߴ��һ��߶�")]
        public float radius = 0.5f;
        public Vector3 boxSize = Vector3.one;      // Box �ߴ�
        public float capsuleHeight = 2f;           // Capsule �߶ȣ��� Y �ᣩ
        public float capsuleRadius = 0.5f;         // Capsule �뾶

        [Header("��������")]
        public bool useGravity = true;

        [Header("�ŵ׼���")]
        public Transform footPoint;

        [HideInInspector] public Vector3 velocity;
        [HideInInspector] public bool isGrounded = false;

        private Vector3 accumulatedForce;
        private Vector3 accumulatedTorque;
        [HideInInspector] public Rigidbody rb;

        [HideInInspector] public Vector3 predictedPosition;
        [HideInInspector] public Quaternion predictedRotation;

        private void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;

            rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.mass = mass;
                rb.drag = linearDamping;
                rb.angularDrag = angularDamping;
                rb.useGravity = false;
            }
            else
            {
                Debug.LogError("[PhysicsBody] δ�ҵ� Rigidbody��");
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (this == null) return;
            StartCoroutine(WaitAndRegister());
        }

        private IEnumerator WaitAndRegister()
        {
            yield return new WaitUntil(() => PhysicsSystem.Instance != null);
            PhysicsSystem.Instance.RegisterBody(this);
        }

        public void EnableSimulation(bool enable)
        {
            isActive = enable;
        }

        private void OnEnable()
        {
            PhysicsSystem.Instance?.RegisterBody(this);
        }

        private void OnDisable()
        {
            PhysicsSystem.Instance?.UnregisterBody(this);
        }

        public void ApplyForce(Vector3 force) => accumulatedForce += force;
        public void ApplyTorque(Vector3 torque) => accumulatedTorque += torque;

        public void PhysicsUpdate(float deltaTime)
        {
            accumulatedForce -= (1 - linearDamping) * accumulatedForce;
            accumulatedTorque -= (1 - angularDamping) * accumulatedTorque;

            if (rb != null)
            {
                Vector3 acceleration = accumulatedForce / mass;
                rb.velocity += acceleration * deltaTime;

                Vector3 angularAcceleration = accumulatedTorque / mass; // �򻯹���
                accumulatedForce = Vector3.zero;
                accumulatedTorque = Vector3.zero;

                velocity = rb.velocity;
            }
        }

        public void ClearForces()
        {
            accumulatedForce = Vector3.zero;
            accumulatedTorque = Vector3.zero;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Vector3 center = footPoint != null ? footPoint.position : transform.position;

            switch (shape)
            {
                case BodyShape.Sphere:
                    Gizmos.DrawWireSphere(center, radius);
                    break;

                case BodyShape.Box:
                    Gizmos.DrawWireCube(center, boxSize);
                    break;

                case BodyShape.Capsule:
                    DrawWireCapsule(center, capsuleHeight, capsuleRadius);
                    break;
            }

            if (useGravity && PhysicsSystem.Instance != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(center, center + PhysicsSystem.Instance.gravityDirection.normalized * radius);
            }
        }

        // �򵥻��ƽ��ң��� Y �ᣩ
        private void DrawWireCapsule(Vector3 center, float height, float radius)
        {
            // ���°�������
            Vector3 up = center + Vector3.up * (height / 2 - radius);
            Vector3 down = center - Vector3.up * (height / 2 - radius);
            Gizmos.DrawWireSphere(up, radius);
            Gizmos.DrawWireSphere(down, radius);

            // ����������
            Vector3[] directions = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };
            foreach (var dir in directions)
            {
                Gizmos.DrawLine(up + dir * radius, down + dir * radius);
            }
        }
    }
}
