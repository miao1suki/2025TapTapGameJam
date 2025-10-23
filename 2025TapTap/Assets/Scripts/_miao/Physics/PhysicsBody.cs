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

        [Header("物理参数")]
        public float mass = 1f;
        public float bounciness = 0.5f;
        [Tooltip("速度衰减系数，0-1")]
        public float linearDamping = 0.8f;
        [Tooltip("旋转衰减系数，0-1")]
        public float angularDamping = 0.8f;
        [HideInInspector] public Vector3 angularVelocity;

        [Header("碰撞体参数（球体）")]
        [Tooltip("球形半径")]
        public float radius = 0.5f;

        [Header("重力控制")]
        public bool useGravity = true;

        [Header("脚底检测点")]
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
                Debug.LogError("[PhysicsBody] 未找到 Rigidbody！");
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

                Vector3 angularAcceleration = accumulatedTorque / mass; // 简化惯性
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
