using UnityEngine;

namespace miao
{
    public class PlayerMoving : MonoBehaviour
    {
        private Transform centerOfMass;

        [Header("�ƶ�����")]
        public float maxSpeed = 40f; // �������ٶ�
        public float sprintMultiplier = 2f;  // ��̱���
        public float moveForce = 100f; // ���Ĵ�С

        private int jumpCount = 0;       // ��ǰ��Ծ����
        private int maxJump = 4;         // �����Ծ����
        private bool jumpBuffered = false; // �Ƿ�Ԥ����
        private float jumpBufferTime = 0.6f; // ����ʱ��
        private float jumpBufferCounter = 0f;
        private float jumpResetTimer = 0f;
        private float jumpResetDelay = 0.2f; // ֹͣ��Ծ���������Ծ����
        private float verticalThreshold = 0.05f; // �ж��Ƿ��д�ֱ�ƶ�
        private Vector3 lastPosition;
        private PhysicsBody body;
        private Rigidbody rb;

        // �������߼��
        public float groundCheckDistance = 1f; // ���߼���������
        public float groundProximityThreshold = 0.2f; // ��ײ�������ҵ������ֵ
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

        void Update()
        {
            CheckJumpInput();  // ��Ծ���
            HandleJumpReset();

            // ������Ӵ�
            CheckGrounded();
        }

        private void FixedUpdate()
        {
            if (!rb)
            {
                Debug.LogError("δ�ҵ�rb");
                return;
            }

            // �ƶ�����
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

                // ���㵱ǰ�ٶȵ�ˮƽ������X, Z��
                Vector3 velocityFlat = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                float currentSpeed = velocityFlat.magnitude;  // ��ǰ�ٶȵĴ�С

                float speedThreshold = maxSpeed * 0.95f;  // 95% �� maxSpeed

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

            // ��֤ˮƽ�˶�
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.velocity = flatVel + Vector3.up * rb.velocity.y;

            HandleJump();  // ��Ծ�߼�
        }

        // ���߼�����
        private void CheckGrounded()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance))
            {
                // ������߼�⵽��ײ�壬��������Ƿ��㹻�ӽ�
                Vector3 closestPoint = hit.collider.ClosestPointOnBounds(transform.position);
                float distance = Vector3.Distance(closestPoint, transform.position);

                if (distance <= groundProximityThreshold)
                {
                    // ֻ�е��Ӵ����㹻�ӽ�ʱ������Ϊ���ڵ�����
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

            // ���� PhysicsBody �� isGrounded ״̬
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
                    jumpCount = 0; // ������Ծ����
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
            // ���������Ծ������Ծ����δ�ﵽ���ֵ
            if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJump)
            {
                jumpBuffered = true;
                jumpBufferCounter = jumpBufferTime;
            }

            // ����ﵽ�����Ծ������ֹͣ��Ծ����
            if (jumpCount >= maxJump)
            {
                jumpBuffered = false;  // ����Ԥ����
                jumpBufferCounter = 0f; // ���ü�ʱ��
            }

            // �������ʱ���������ԾԤ�������
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

                // ��Ծ�׶�ִ�в�ͬ����Ծ��
                switch (jumpCount)
                {
                    case 1:
                        body.ApplyForce(new Vector3(0, 150f * 6 * body.mass, 0));  // һ����
                        break;
                    case 2:
                        body.ApplyForce(new Vector3(0, 100f * 6 * body.mass, 0));  // ������
                        break;
                    case 3:
                        body.ApplyForce(new Vector3(0, 500f * 6 * body.mass, 0));  // ����
                        break;
                    case 4:
                        body.ApplyForce(new Vector3(Random.Range(-100f, 100f) * 6 * body.mass, Random.Range(-100f, 40f) * 30 * body.mass, Random.Range(-100f, 100f) * 6 * body.mass)); // ����ҷ�
                        break;
                }
            }
        }
    }
}
