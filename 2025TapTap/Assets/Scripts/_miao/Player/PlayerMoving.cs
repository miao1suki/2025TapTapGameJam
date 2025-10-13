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


        [Header("�ƶ�����")]
        public float maxSpeed = 40f;//�������ٶ�
        public float sprintMultiplier = 2f;  // ��̱���

        [HideInInspector]
        public float moveForce = 100f;// ���Ĵ�С


        private int jumpCount = 0;       // ��ǰ��Ծ����
        private int maxJump = 4;         // �����Ծ����
        private bool jumpBuffered = false; // �Ƿ�Ԥ����
        private float jumpBufferTime = 0.6f; // ����ʱ��
        private float jumpBufferCounter = 0f;
        private Vector3 lastPosition;
        private float jumpResetTimer = 0f;
        private float jumpResetDelay = 0.2f; // ֹͣ��Ծ���������Ծ����
        private float verticalThreshold = 0.05f; // �ж��Ƿ��д�ֱ�ƶ�

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
                Debug.LogWarning("Playerû���ҵ�PhysicsBody");
            }

            // ���ø��������
            if (centerOfMass != null)
            {
                rb.centerOfMass = centerOfMass.localPosition;
            }
            else
            {
                Debug.LogWarning("[PlayerMoving] δ�ҵ� CenterOfMass �ڵ㣡");
            }

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
            //ÿ֡��ⰴ������
            key_Reset();
            key_Down();
            key_Keep();
            //////////////////
            
            CheckJumpInput();//��Ծ���
            HandleJumpReset();
            //Debug.Log("///////" + jumpCount);


            /*
            if (Input.GetKeyDown(KeyCode.Space))
            {
                JiMiUnlockManager.Instance.OnPlayerEvent(Player.Instance);
                Debug.Log("����һ��");
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
                Debug.LogError("δ�ҵ�rb");
                return;
            }
            ////////////////////////////////////

            Vector3 moveDir = Vector3.zero;

            if (W_Flag) moveDir += transform.forward; 
            if (S_Flag) moveDir -= transform.forward;
            if (A_Flag) moveDir -= transform.right; 
            if (D_Flag) moveDir += transform.right;


            // �ڲٿ��ٶ�ǰ��� moveDir �Ƿ�Ϊ��
            if (moveDir.magnitude > 0f)
            {
                moveDir.Normalize();

                float currentMultiplier = Left_Shift_Flag ? sprintMultiplier : 1f;
                Vector3 force = moveDir * moveForce * currentMultiplier * rb.mass;

                // ���㵱ǰ�ٶȵ�ˮƽ������X, Z��
                Vector3 velocityFlat = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                float currentSpeed = velocityFlat.magnitude;  // ��ǰ�ٶȵĴ�С

                // �趨��ֵ�������ڽӽ�����ٶ�ʱ��ֹͣʩ����
                float speedThreshold = maxSpeed * 0.95f;  // 95% �� maxSpeed

                if (currentSpeed < speedThreshold)
                {
                    // �����ǰ�ٶȵ�����ֵ������ʩ����
                    body.ApplyForce(force);
                }
                else
                {
                    // Ԥ���´�ʩ�ӵ����Ƿ�ᳬ��
                    Vector3 predictedVelocity = velocityFlat + force.normalized * Time.fixedDeltaTime; // Ԥ����һ֡���ٶ�
                    float predictedSpeed = predictedVelocity.magnitude;

                    // ���Ԥ���ٶȳ�������ٶȣ�������ʹ��ǡ�ôﵽ����ٶ�
                    if (predictedSpeed > maxSpeed)
                    {
                        Vector3 velocityAdjustment = predictedVelocity.normalized * maxSpeed - velocityFlat;
                        body.ApplyForce(velocityAdjustment / Time.fixedDeltaTime);  // �õ����������ȷ���ٶȲ�����
                    }
                    else
                    {
                        body.ApplyForce(force);  // ������ᳬ�٣�����ʩ����������
                    }
                }
            }

            // ��֤ˮƽ�˶�����ֹ����ʱ�ҷɣ�
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.velocity = flatVel + Vector3.up * rb.velocity.y;



            //��ԾԤ����ִ��
            HandleJump();
        }

       

        private void HandleJumpReset()
        {
            float verticalDelta = transform.position.y - lastPosition.y;

            // ����������Ԥ����������������ԾԤ����
            if (verticalDelta < -verticalThreshold || (jumpBuffered && jumpBufferCounter <= 0f))
            {
                jumpBuffered = false;
            }

            // ֻ�е�����û�д�ֱ�ƶ�������Ԥ��������󣬲���������Ծ����
            if (Mathf.Abs(verticalDelta) < verticalThreshold && !jumpBuffered)
            {
                jumpResetTimer += Time.deltaTime;

                if (jumpResetTimer >= jumpResetDelay)
                {
                    jumpCount = 0; // ������Ծ����
                    jumpResetTimer = 0f;
                }
            }
            else
            {
                // �����Դ�ֱ�ƶ���Ԥ����δ���������ü�ʱ��
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
                        body.ApplyForce(new Vector3(0, 150f * 5 * body.mass, 0));// һ����
                        break;
                    case 2:
                        body.ApplyForce(new Vector3(0, 100f * 5 * body.mass, 0));// ������
                        break;
                    case 3:
                        body.ApplyForce(new Vector3(0, 500f * 5 * body.mass, 0));// ����
                        break;
                    case 4:
                        body.ApplyForce(new Vector3(Random.Range(-1000f, 1000f) * 2 * body.mass, 1200f * 2 * body.mass, Random.Range(-1000f, 1000f) * 2 * body.mass)); // ����ҷ�
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
