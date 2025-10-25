using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace miao
{
    public class PlayerMoving : MonoBehaviour
    {
        private Transform centerOfMass;
        private Rigidbody rb;
        private PhysicsBody body;
        private Animator animator;
        [SerializeField] GameObject model;

        [Header("移动参数")]
        public float maxSpeed = 40f;
        public float sprintMultiplier = 2f;
        public float moveForce = 100f;

        private int jumpCount = 0;
        private int maxJump = 2;
        private bool jumpBuffered = false;
        private float jumpBufferTime = 0.6f;
        private float jumpBufferCounter = 0f;
        private float jumpResetTimer = 0f;
        private float jumpResetDelay = 0.2f;
        private float verticalThreshold = 0.05f;
        private Vector3 lastPosition;

        [Header("地面检测参数")]
        public float groundCheckDistance = 1f;
        public float groundProximityThreshold = 0.2f;
        private bool isGrounded;

        // ------------------------------
        // 加分冷却
        [Header("加分冷却（同类事件冷却）")]
        public float cooldownDuration = 1.5f;
        private Dictionary<string, float> lastScoreTime = new Dictionary<string, float>();

        // ------------------------------
        // 🪶 滞空系统
        [Header("滞空系统参数")]
        public float airborneStartDelay = 0.8f;   // 超过此时间开始加分
        public float airborneScoreInterval = 0.1f; // 每次加分间隔
        public float airborneMultiplierInterval = 1f; // 每次加倍率间隔
        private float airborneTime = 0f;
        private float scoreTimer = 0f;
        private float multiplierTimer = 0f;
        private bool wasAirborne = false;
        private bool staySet = false; // 记录 Animator Stay 是否开启
        private float airborneDamageStartTime = 0.2f; // 记录滞空造成伤害的间隔时间

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            body = GetComponent<PhysicsBody>();
            animator = GetComponentInChildren<Animator>();

            if (rb == null)
                Debug.LogError("PlayerMoving: 未找到 Rigidbody！");
            if (body == null)
                Debug.LogWarning("PlayerMoving: 未找到 PhysicsBody！");
            if (animator == null)
                Debug.LogWarning("PlayerMoving: 未找到 Animator！");

            Transform t = transform.Find("CenterOfMass");
            if (t != null)
            {
                centerOfMass = t;
                rb.centerOfMass = centerOfMass.localPosition;
            }
        }

        void Update()
        {
            CheckJumpInput();
            HandleJumpReset();
            CheckGrounded();
            HandleAirborne(); // 每帧处理滞空系统

            // F 键触发攻击
            if (Input.GetKeyDown(KeyCode.F))
            {
                ScoreTrigger.Instance.AddScore("！！哈气！！",6);
                ScoreTrigger.Instance.AddMultiplier();
                StartCoroutine(TriggerAnimatorBool("Fight", 0.1f));
            }
        }

        private void LateUpdate()
        {
            if(model)
            {
                model.transform.localPosition = new Vector3(0f, -0.74f, 0f);
                model.transform.localRotation = Quaternion.identity;
            }
            else
            {
                Debug.LogWarning("未设置模型");
            }

        }

        private void FixedUpdate()
        {
            if (!rb) return;

            Vector3 moveDir = Vector3.zero;
            Transform cam = Camera.main.transform;

            Vector3 camForward = new Vector3(cam.forward.x, 0f, cam.forward.z).normalized;
            Vector3 camRight = new Vector3(cam.right.x, 0f, cam.right.z).normalized;

            // ----------- 移动输入 -------------
            if (InputController.Instance.get_Key("W")) { moveDir += camForward; TryTriggerScore("前进!", 1); }
            if (InputController.Instance.get_Key("S")) { moveDir -= camForward; TryTriggerScore("后退!", 1); }
            if (InputController.Instance.get_Key("A")) { moveDir -= camRight; TryTriggerScore("左转!", 1); }
            if (InputController.Instance.get_Key("D")) { moveDir += camRight; TryTriggerScore("右转!", 1); }

            // ----------- 动画参数更新 -------------
            if (animator != null)
            {
                // 将移动方向转换为相对于摄像机的局部坐标
                float moveX = Vector3.Dot(moveDir, camRight);
                float moveY = Vector3.Dot(moveDir, camForward);

                animator.SetFloat("MoveX", moveX, 0.1f, Time.fixedDeltaTime); // 平滑过渡
                animator.SetFloat("MoveY", moveY, 0.1f, Time.fixedDeltaTime);

            }
            

            // ----------- 实际移动 -------------
            if (moveDir.magnitude > 0f)
            {
                moveDir.Normalize();
                float currentMultiplier = InputController.Instance.get_Key("Left_Shift") ? sprintMultiplier : 1f;
                Vector3 force = moveDir * moveForce * currentMultiplier * rb.mass;

                Vector3 velocityFlat = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                float currentSpeed = velocityFlat.magnitude;
                float speedThreshold = maxSpeed * 0.95f;

                // ----------- 玩家朝向旋转 -------------
                Vector3 lookDir = new Vector3(cam.forward.x, 0f, cam.forward.z).normalized;
                if (lookDir.sqrMagnitude > 0.01f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(lookDir, Vector3.up);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
                }

                // ----------- 速度限制和施力 -------------
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

            // 保持水平速度
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.velocity = flatVel + Vector3.up * rb.velocity.y;
            // 更新 Animator MoveSpeed
            float speedSqr = flatVel.sqrMagnitude;
            animator.SetFloat("MoveSpeed", speedSqr, 0.1f, Time.fixedDeltaTime);

            HandleJump();
        }

        private void CheckGrounded()
        {
            // 射线起点从脚底稍微往上（防止贴地时误判）
            Vector3 rayOrigin = transform.position + Vector3.down * 0.9f;  // 根据你的模型高度调整
            RaycastHit hit;

            if (Physics.Raycast(rayOrigin, Vector3.down, out hit, groundCheckDistance))
            {
                float distance = hit.distance;
                isGrounded = distance <= groundProximityThreshold;
            }
            else
            {
                isGrounded = false;
            }

            body.isGrounded = isGrounded;
        }

        private void HandleJumpReset()
        {
            float verticalDelta = transform.position.y - lastPosition.y;
            if (verticalDelta < -verticalThreshold || (jumpBuffered && jumpBufferCounter <= 0f))
                jumpBuffered = false;

            if (Mathf.Abs(verticalDelta) < verticalThreshold && !jumpBuffered)
            {
                jumpResetTimer += Time.deltaTime;
                if (jumpResetTimer >= jumpResetDelay)
                {
                    jumpCount = 0;
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
            if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJump)
            {
                jumpBuffered = true;
                jumpBufferCounter = jumpBufferTime;
            }

            if (jumpCount >= maxJump)
            {
                jumpBuffered = false;
                jumpBufferCounter = 0f;
            }

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

                switch (jumpCount)
                {
                    case 1:
                        body.ApplyForce(new Vector3(0, 150f * 6 * body.mass, 0));
                        TryTriggerScore("跳跃!", 1);
                        break;
                    case 2:
                        body.ApplyForce(new Vector3(0, 100f * 6 * body.mass, 0));
                        TryTriggerScore("二段跳!!", 2);
                        break;
                }
            }
        }

        private void TryTriggerScore(string type, int value)
        {
            if (ScoreTrigger.Instance == null) return;
            float now = Time.time;

            if (lastScoreTime.TryGetValue(type, out float last))
            {
                if (now - last < cooldownDuration) return;
            }

            lastScoreTime[type] = now;
            ScoreTrigger.Instance.AddScore(type, value);
        }

        // 处理滞空逻辑
        private void HandleAirborne()
        {
            // 在 Title 场景中不执行滞空逻辑
            if (SceneManager.GetActiveScene().name == "Title")
                return;

            if (!body.isGrounded)
            {
                airborneTime += Time.deltaTime;

                // --- 滞空开始延迟 ---
                if (airborneTime > airborneStartDelay)
                {
                    // 加分逻辑
                    scoreTimer += Time.deltaTime;
                    multiplierTimer += Time.deltaTime;

                    if (scoreTimer >= airborneScoreInterval)
                    {
                        ScoreTrigger.Instance?.AddScore("滞空", 100);
                        scoreTimer = 0f;
                    }

                    if (multiplierTimer >= airborneMultiplierInterval)
                    {
                        ScoreTrigger.Instance?.AddMultiplier();
                        multiplierTimer = 0f;
                    }

                    // 滞空超过5秒执行一次特殊事件
                    if (airborneTime >= 5f && !wasAirborne)
                    {
                        wasAirborne = true;
                        OnLongAirborne(); // 留接口
                    }

                    // 动画触发（仅设置一次）
                    if (!staySet)
                    {
                        staySet = true;
                        animator.SetBool("Stay", true);
                    }
                }
            }
            else
            {
                // 落地 -> 重置所有逻辑
                if (staySet)
                {
                    animator.SetBool("Stay", false);
                    staySet = false;

                    // 落地时结算滞空扣血
                    float extraTime = airborneTime - airborneStartDelay;
                    if (extraTime > 0f)
                    {
                        int tickCount = Mathf.FloorToInt(extraTime / airborneDamageStartTime);
                        
                        int lostHealth = Mathf.FloorToInt(tickCount) * 10;
                        Player.Instance.ChangePlayerHealth(-lostHealth);
                        CheckPlayerHealth();
                    }
                }

                airborneTime = 0f;
                scoreTimer = 0f;
                multiplierTimer = 0f;
                wasAirborne = false;
            }
        }

        //  检查玩家生命值，触发死亡动画
        private void CheckPlayerHealth()
        {
            if (Player.Instance._PlayerHealth <= 0)
            {
                Player.Instance.ResetPlayerHealth();
                StartCoroutine(TriggerAnimatorBool("Dead", 0.5f));
                ScoreTrigger.Instance.AddScore("昏昏倒地",44444);
            }
        }

        //  通用延时清除标签协程
        private IEnumerator TriggerAnimatorBool(string paramName, float duration)
        {
            animator.SetBool(paramName, true);
            yield return new WaitForSeconds(duration);
            animator.SetBool(paramName, false);
        }


        // 5 秒滞空接口
        private void OnLongAirborne()
        {
            
            // 这里留做特殊逻辑，比如触发特效、特殊状态等
        }
    }
}
