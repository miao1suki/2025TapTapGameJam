using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace miao
{
    public class PlayerMoving : MonoBehaviour
    {
        private Transform centerOfMass;


        [Header("移动参数")]
        public float maxSpeed = 40f;//玩家最大速度
        public float sprintMultiplier = 2f;  // 冲刺倍数

        [HideInInspector]
        public float moveForce = 100f;// 力的大小


        private int jumpCount = 0;       // 当前跳跃段数
        private int maxJump = 4;         // 最大跳跃次数
        private bool jumpBuffered = false; // 是否预输入
        private float jumpBufferTime = 0.6f; // 缓冲时间
        private float jumpBufferCounter = 0f;
        private Vector3 lastPosition;
        private float jumpResetTimer = 0f;
        private float jumpResetDelay = 0.2f; // 停止跳跃多久重置跳跃次数
        private float verticalThreshold = 0.05f; // 判断是否有垂直移动

        private PhysicsBody body;

        private Rigidbody rb;
        private void Awake()
        {
            if (rb == null) rb = GetComponent<Rigidbody>();
            if (centerOfMass == null)
            {
                Transform t = transform.Find("CenterOfMass");
                if (t != null) centerOfMass = t;
            }
            body = GetComponent<PhysicsBody>();
            if(!body)
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

        void Start()
        {

        }


        void Update()
        {
            CheckJumpInput();//跳跃检测
            HandleJumpReset();
            //Debug.Log("///////" + jumpCount);


            /*
            if (Input.GetKeyDown(KeyCode.Space))
            {
                JiMiUnlockManager.Instance.OnPlayerEvent(Player.Instance);
                Debug.Log("哈气一次");
                Player.Instance.haQiCount += 1;
                JiMiUnlockManager.Instance.CheckUnlocks(Player.Instance);
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                Player.Instance.SetJiMiType(JiMiType.HaNiuMo);
            }
            */
        }

        private void FixedUpdate()
        {
            Debug.LogWarning(jumpCount);
            if(!rb)
            {
                Debug.LogError("未找到rb");
                return;
            }
            ////////////////////////////////////

            Vector3 moveDir = Vector3.zero;
  
            if (InputController.Instance.get_Key("W")) moveDir += transform.forward; 
            if (InputController.Instance.get_Key("S")) moveDir -= transform.forward;
            if (InputController.Instance.get_Key("A")) moveDir -= transform.right; 
            if (InputController.Instance.get_Key("D")) moveDir += transform.right;

            // 在操控速度前检查 moveDir 是否为零
            if (moveDir.magnitude > 0f)
            {
                moveDir.Normalize();

                float currentMultiplier = InputController.Instance.get_Key("Left_Shift") ? sprintMultiplier : 1f;
                Vector3 force = moveDir * moveForce * currentMultiplier * rb.mass;

                // 计算当前速度的水平分量（X, Z）
                Vector3 velocityFlat = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                float currentSpeed = velocityFlat.magnitude;  // 当前速度的大小

                // 设定阈值，控制在接近最大速度时就停止施加力
                float speedThreshold = maxSpeed * 0.95f;  // 95% 的 maxSpeed

                if (currentSpeed < speedThreshold)
                {
                    // 如果当前速度低于阈值，继续施加力
                    body.ApplyForce(force);
                }
                else
                {
                    // 预测下次施加的力是否会超速
                    Vector3 predictedVelocity = velocityFlat + force.normalized * Time.fixedDeltaTime; // 预估下一帧的速度
                    float predictedSpeed = predictedVelocity.magnitude;

                    // 如果预测速度超过最大速度，调整力使其恰好达到最大速度
                    if (predictedSpeed > maxSpeed)
                    {
                        Vector3 velocityAdjustment = predictedVelocity.normalized * maxSpeed - velocityFlat;
                        body.ApplyForce(velocityAdjustment / Time.fixedDeltaTime);  // 用调整后的力来确保速度不超限
                    }
                    else
                    {
                        body.ApplyForce(force);  // 如果不会超速，继续施加正常的力
                    }
                }
            }

            // 保证水平运动（防止翻滚时乱飞）
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.velocity = flatVel + Vector3.up * rb.velocity.y;



            //跳跃预输入执行
            HandleJump();
        }

       

        private void HandleJumpReset()
        {
            float verticalDelta = transform.position.y - lastPosition.y;

            // 如果下落或者预输入结束，则禁用跳跃预输入
            if (verticalDelta < -verticalThreshold || (jumpBuffered && jumpBufferCounter <= 0f))
            {
                jumpBuffered = false;
            }

            // 只有当几乎没有垂直移动，并且预输入结束后，才能重置跳跃次数
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
                // 有明显垂直移动或预输入未结束，重置计时器
                jumpResetTimer = 0f;
            }

            lastPosition = transform.position;
        }


        private void CheckJumpInput()
        {
            if (InputController.Instance.get_Key("Space"))
            {
                jumpBuffered = true;
                jumpBufferCounter = jumpBufferTime;
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
                        body.ApplyForce(new Vector3(0, 150f * 5 * body.mass, 0));// 一段跳
                        break;
                    case 2:
                        body.ApplyForce(new Vector3(0, 100f * 5 * body.mass, 0));// 二段跳
                        break;
                    case 3:
                        body.ApplyForce(new Vector3(0, 500f * 5 * body.mass, 0));// 弹飞
                        break;
                    case 4:
                        body.ApplyForce(new Vector3(Random.Range(-1000f, 1000f) * 2 * body.mass, 1200f * 2 * body.mass, Random.Range(-1000f, 1000f) * 2 * body.mass)); // 随机乱飞
                        break;
                }
            }
        }        
    }
}
