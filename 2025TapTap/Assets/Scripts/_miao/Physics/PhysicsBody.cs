using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace miao
{
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

        [Header("��ײ����������壩")]
        [Tooltip("���ΰ뾶")]
        public float radius = 0.5f;

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
            Gizmos.DrawWireSphere(center, radius);

            if (useGravity && PhysicsSystem.Instance != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(center, center + PhysicsSystem.Instance.gravityDirection.normalized * radius);
            }
        }
    }
}
