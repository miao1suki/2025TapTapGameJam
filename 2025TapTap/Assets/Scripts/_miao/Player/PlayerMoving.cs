using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace miao
{
    public class PlayerMoving : MonoBehaviour
    {
        private bool W_Flag;
        private bool A_Flag;
        private bool S_Flag;
        private bool D_Flag;
        private bool Left_Shift_Flag;
        private Transform centerOfMass;


        [Header("移动参数")]
        public float maxSpeed = 30f;//玩家最大速度
        public float sprintMultiplier = 2f;  // 冲刺倍数
        public float drag = 4f;                // 阻力

        [HideInInspector]
        public float moveForce = 100f;// 力的大小


        private int jumpCount = 0;       // 当前跳跃段数
        private int maxJump = 4;         // 最大跳跃次数
        private bool jumpBuffered = false; // 是否预输入
        private float jumpBufferTime = 0.2f; // 缓冲时间
        private float jumpBufferCounter = 0f;
        private Vector3 lastPosition;
        private float jumpResetTimer = 0f;
        private float jumpResetDelay = 0.2f; // 停止跳跃多久重置跳跃次数
        private float verticalThreshold = 0.05f; // 判断是否有垂直移动

        private Rigidbody rb;
        private void Awake()
        {
            if (rb == null) rb = GetComponent<Rigidbody>();
            if (centerOfMass == null)
            {
                Transform t = transform.Find("CenterOfMass");
                if (t != null) centerOfMass = t;
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
            rb.drag = drag;
            rb.angularDrag = 2f;
        }

        void Start()
        {
            W_Flag = false;
            A_Flag = false;
            S_Flag = false;
            D_Flag = false;
            Left_Shift_Flag = false;
        }


        void Update()
        {
            //每帧检测按键部分
            key_Reset();
            key_Down();
            key_Keep();
            //////////////////
            
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
            if(!rb)
            {
                Debug.LogError("未找到rb");
                return;
            }
            ////////////////////////////////////

            Vector3 moveDir = Vector3.zero;

            if (W_Flag) moveDir += transform.forward;
            if (S_Flag) moveDir -= transform.forward;
            if (A_Flag) moveDir -= transform.right;
            if (D_Flag) moveDir += transform.right;

            moveDir.Normalize();

            float currentMultiplier = Left_Shift_Flag ? sprintMultiplier : 1f;
            Vector3 force = moveDir * moveForce * currentMultiplier * rb.mass;

            // 限制最大速度
            if (rb.velocity.magnitude < maxSpeed)
            {
                rb.AddForce(force, ForceMode.Force);
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
            if (Input.GetKeyDown(KeyCode.Space))
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
                        rb.velocity = new Vector3(rb.velocity.x, 15f , rb.velocity.z); // 一段跳
                        break;
                    case 2:
                        rb.velocity = new Vector3(rb.velocity.x, 10f, rb.velocity.z); // 二段跳
                        break;
                    case 3:
                        rb.velocity = new Vector3(rb.velocity.x, 50f, rb.velocity.z); // 弹飞
                        break;
                    case 4:
                        Vector3 randomDir = new Vector3(Random.Range(-100f, 100f), 120f, Random.Range(-100f, 100f));
                        rb.velocity = randomDir; // 随机乱飞
                        break;
                }
            }
        }



        private void key_Down()
        {
            if (Input.GetKeyDown(KeyCode.W)) W_Flag = true;
            if (Input.GetKeyDown(KeyCode.A)) A_Flag = true;
            if (Input.GetKeyDown(KeyCode.S)) S_Flag = true;
            if (Input.GetKeyDown(KeyCode.D)) D_Flag = true;
            if (Input.GetKeyDown(KeyCode.LeftShift)) Left_Shift_Flag = true;

        }

        private void key_Keep()
        {
            if (Input.GetKey(KeyCode.W)) W_Flag = true;
            if (Input.GetKey(KeyCode.A)) A_Flag = true;
            if (Input.GetKey(KeyCode.S)) S_Flag = true;
            if (Input.GetKey(KeyCode.D)) D_Flag = true;
            if (Input.GetKey(KeyCode.LeftShift)) Left_Shift_Flag = true;
        }

        private void key_Reset()
        {
                W_Flag = false;
                A_Flag = false;
                S_Flag = false;
                D_Flag = false;
                Left_Shift_Flag = false;

        }
        
    }
}
