using UnityEngine;

namespace miao
{
    public class PlayerMoving : MonoBehaviour
    {
        private Transform centerOfMass;

        [Header("移动参数")]
        public float maxSpeed = 40f; // 玩家最大速度
        public float sprintMultiplier = 2f;  // 冲刺倍数
        public float moveForce = 100f; // 力的大小

        private int jumpCount = 0;       // 当前跳跃段数
        private int maxJump = 4;         // 最大跳跃次数
        private bool jumpBuffered = false; // 是否预输入
        private float jumpBufferTime = 0.6f; // 缓冲时间
        private float jumpBufferCounter = 0f;
        private float jumpResetTimer = 0f;
        private float jumpResetDelay = 0.2f; // 停止跳跃多久重置跳跃次数
        private float verticalThreshold = 0.05f; // 判断是否有垂直移动
        private Vector3 lastPosition;
        private PhysicsBody body;
        private Rigidbody rb;

        // 用于射线检测
        public float groundCheckDistance = 1f; // 射线检测的最大距离
        public float groundProximityThreshold = 0.2f; // 碰撞体距离玩家的最大阈值
        private bool isGrounded;

        private void Awake()
        {
            if (rb == null) rb = GetComponent<Rigidbody>();
            if (centerOfMass == null)
            {
                Transform t = transform.Find("CenterOfMass");
                if (t != null) centerOfMass = t;
            }
            body = GetComponent<PhysicsBody>();
            if (!body)
            {
                Debug.LogWarning("Player没有找到PhysicsBody");
            }

            // 设置刚体的重心
            if (centerOfMass != null)
            {
                rb.centerOfMass = centerOfMass.localPosition;
            }
            else
            {
                Debug.LogWarning("[PlayerMoving] 未找到 CenterOfMass 节点！");
            }
        }

        void Update()
        {
            CheckJumpInput();  // 跳跃检测
            HandleJumpReset();

            // 检测地面接触
            CheckGrounded();
        }

        private void FixedUpdate()
        {
            if (!rb)
            {
                Debug.LogError("未找到rb");
                return;
            }

            // 移动处理
            Vector3 moveDir = Vector3.zero;
            if (InputController.Instance.get_Key("W")) moveDir += transform.forward;
            if (InputController.Instance.get_Key("S")) moveDir -= transform.forward;
            if (InputController.Instance.get_Key("A")) moveDir -= transform.right;
            if (InputController.Instance.get_Key("D")) moveDir += transform.right;

            if (moveDir.magnitude > 0f)
            {
                moveDir.Normalize();

                float currentMultiplier = InputController.Instance.get_Key("Left_Shift") ? sprintMultiplier : 1f;
                Vector3 force = moveDir * moveForce * currentMultiplier * rb.mass;

                // 计算当前速度的水平分量（X, Z）
                Vector3 velocityFlat = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                float currentSpeed = velocityFlat.magnitude;  // 当前速度的大小

                float speedThreshold = maxSpeed * 0.95f;  // 95% 的 maxSpeed

                if (currentSpeed < speedThreshold)
                {
                    body.ApplyForce(force);
                }
                else
                {
                    Vector3 predictedVelocity = velocityFlat + force.normalized * Time.fixedDeltaTime;
                    float predictedSpeed = predictedVelocity.magnitude;

                    if (predictedSpeed > maxSpeed)
                    {
                        Vector3 velocityAdjustment = predictedVelocity.normalized * maxSpeed - velocityFlat;
                        body.ApplyForce(velocityAdjustment / Time.fixedDeltaTime);
                    }
                    else
                    {
                        body.ApplyForce(force);
                    }
                }
            }

            // 保证水平运动
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.velocity = flatVel + Vector3.up * rb.velocity.y;

            HandleJump();  // 跳跃逻辑
        }

        // 射线检测地面
        private void CheckGrounded()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance))
            {
                // 如果射线检测到碰撞体，继续检测是否足够接近
                Vector3 closestPoint = hit.collider.ClosestPointOnBounds(transform.position);
                float distance = Vector3.Distance(closestPoint, transform.position);

                if (distance <= groundProximityThreshold)
                {
                    // 只有当接触点足够接近时，才认为是在地面上
                    isGrounded = true;
                }
                else
                {
                    isGrounded = false;
                }
            }
            else
            {
                isGrounded = false;
            }

            // 更新 PhysicsBody 的 isGrounded 状态
            body.isGrounded = isGrounded;
        }

        private void HandleJumpReset()
        {
            float verticalDelta = transform.position.y - lastPosition.y;

            if (verticalDelta < -verticalThreshold || (jumpBuffered && jumpBufferCounter <= 0f))
            {
                jumpBuffered = false;
            }

            if (Mathf.Abs(verticalDelta) < verticalThreshold && !jumpBuffered)
            {
                jumpResetTimer += Time.deltaTime;

                if (jumpResetTimer >= jumpResetDelay)
                {
                    jumpCount = 0; // 重置跳跃次数
                    jumpResetTimer = 0f;
                }
            }
            else
            {
                jumpResetTimer = 0f;
            }

            lastPosition = transform.position;
        }

        private void CheckJumpInput()
        {
            // 如果按下跳跃键且跳跃次数未达到最大值
            if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJump)
            {
                jumpBuffered = true;
                jumpBufferCounter = jumpBufferTime;
            }

            // 如果达到最大跳跃次数，停止跳跃输入
            if (jumpCount >= maxJump)
            {
                jumpBuffered = false;  // 结束预输入
                jumpBufferCounter = 0f; // 重置计时器
            }

            // 如果缓冲时间结束，跳跃预输入结束
            if (jumpBuffered)
            {
                jumpBufferCounter -= Time.deltaTime;
                if (jumpBufferCounter <= 0f)
                    jumpBuffered = false;
            }
        }

        private void HandleJump()
        {
            if (jumpBuffered && jumpCount < maxJump)
            {
                jumpCount++;
                jumpBuffered = false;

                // 跳跃阶段执行不同的跳跃力
                switch (jumpCount)
                {
                    case 1:
                        body.ApplyForce(new Vector3(0, 150f * 6 * body.mass, 0));  // 一段跳
                        break;
                    case 2:
                        body.ApplyForce(new Vector3(0, 100f * 6 * body.mass, 0));  // 二段跳
                        break;
                    case 3:
                        body.ApplyForce(new Vector3(0, 500f * 6 * body.mass, 0));  // 弹飞
                        break;
                    case 4:
                        body.ApplyForce(new Vector3(Random.Range(-100f, 100f) * 6 * body.mass, Random.Range(-100f, 40f) * 30 * body.mass, Random.Range(-100f, 100f) * 6 * body.mass)); // 随机乱飞
                        break;
                }
            }
        }
    }
}
