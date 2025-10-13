using System.Collections;
using UnityEngine;

namespace miao
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))] // 自动确保有刚体
    public class PhysicsBody : MonoBehaviour
    {
        [Header("物理参数")]
        public float mass = 1f;
        public float bounciness = 0.5f;
        [Tooltip("速度衰减系数，0-1")]
        public float linearDamping = 0.1f;
        [Tooltip("旋转衰减系数，0-1")]
        public float angularDamping = 0.1f;
        [HideInInspector] public Vector3 angularVelocity;

        [Tooltip("球形碰撞半径，用于物理系统")]
        public float radius = 0.5f;

        [Header("重力控制")]
        public bool useGravity = true;  // 启用重力（单物品）

        [Header("脚底检测点")]
        public Transform footPoint; // 用于碰撞检测的底部点

        [HideInInspector] public Vector3 velocity;
        [HideInInspector] public bool isGrounded = false;

        private Vector3 accumulatedForce;
        private Vector3 accumulatedTorque; // 用于计算旋转力矩

        [HideInInspector] public Rigidbody rb;

        private void Awake()
        {
            // 获取刚体
            rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.mass = mass;
                rb.drag = linearDamping;      // 线性阻力交给 Rigidbody
                rb.angularDrag = angularDamping; // 角速度阻力交给 Rigidbody
                rb.useGravity = false;        // 关闭 Rigidbody 重力，由 PhysicsSystem 控制
            }
            else
            {
                Debug.LogError("[PhysicsBody] 未找到 Rigidbody！");
            }
        }

        private void OnEnable()
        {
            StartCoroutine(WaitAndRegister());
        }

        private IEnumerator WaitAndRegister()
        {
            // 等待 PhysicsSystem 初始化
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
            accumulatedTorque += torque; //  施加旋转力矩
        }

        public void PhysicsUpdate(float deltaTime)
        {
            // -------------------------
            // 使用累加力更新 Rigidbody
            // -------------------------
            if (rb != null)
            {
                // 加速度 = F / m
                Vector3 acceleration = accumulatedForce / mass;

                // -------------------------
                // 线性运动
                // -------------------------
                rb.velocity += acceleration * deltaTime;

                // -------------------------
                // 角运动
                // -------------------------
                Vector3 angularAcceleration = accumulatedTorque / mass; // 简化惯性
                angularVelocity += angularAcceleration * deltaTime;
                rb.angularVelocity += angularVelocity * deltaTime;



                // -------------------------
                // 清空累计力和力矩
                // -------------------------
                accumulatedForce = Vector3.zero;
                accumulatedTorque = Vector3.zero;

                // 同步 velocity 便于 PhysicsSystem 使用
                velocity = rb.velocity;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;

            Vector3 center = footPoint != null ? footPoint.position : transform.position;

            // 绘制球形半径
            Gizmos.DrawWireSphere(center, radius);

            // 绘制向下的重力方向（如果使用）
            if (useGravity && PhysicsSystem.Instance != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(center, center + PhysicsSystem.Instance.gravityDirection.normalized * radius);
            }
        }
    }
}
