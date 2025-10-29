using UnityEngine;

namespace miao
{
    public class SimpleTongueController : MonoBehaviour
    {
        [Header("��ͷ����")]
        public Transform tongueModel;  // ��ͷģ��
        public Transform tongueRoot;   // ��ͷ������ͨ�����������ͣ�

        [Header("��������")]
        public float extendSpeed = 5f;       // �����ٶ�
        public float moveSpeed = 2.5f;       // λ���ƶ��ٶ�
        public float maxStretch = 1f;        // ��� z ��������
        public float maxMoveForward = 0.5f;  // ���ǰ�ƾ���

        [Header("�ӷ���ȴ������Լӷ֣�")]
        public float scoreCooldown = 1.5f;   // ���ӷֵ���ȴ���룩

        private bool isExtending = false;
        private float currentStretch = 0f;
        private float currentForward = 0f;
        private Vector3 baseLocalScale;
        private Vector3 baseLocalPosition;

        // �ӷ���ȴ��ʱ����Only for scoring��
        private float scoreCooldownTimer = 0f;

        private bool hasPlayedSound = false; // ��ֹ����ͷδ�ջ�ǰ�ظ�����
        public string tongueSoundName = "��ͷ�³���ճҺճ���"; // ��Ч��

        void Start()
        {
            if (tongueModel == null || tongueRoot == null)
            {
                Debug.LogError("��ȷ�� tongueModel �� tongueRoot �Ѿ���ֵ");
                return;
            }

            baseLocalScale = tongueModel.localScale;
            baseLocalPosition = tongueModel.localPosition;
        }

        void Update()
        {
            HandleInput();

            // ���¼ӷ���ȴ��ʱ��ֻӰ���Ƿ����ٴμӷ֣���Ӱ����ͷ������
            if (scoreCooldownTimer > 0f)
                scoreCooldownTimer -= Time.deltaTime;

            UpdateTongueRootRotation();
            UpdateTongue();

            // ����������ಢ����ȴ��λ���򴥷�һ�μӷֲ�������ȴ
            if (isExtending && scoreCooldownTimer <= 0f)
            {
                TryScore();
            }
        }

        void HandleInput()
        {
            // ������ͷ������Ӧ������Ч��������ȴ���ƣ�
            isExtending = Input.GetMouseButton(0);
        }

        void TryScore()
        {
            // ֻ������ڵ� ScoreTrigger ����
            if (ScoreTrigger.Instance != null)
            {
                ScoreTrigger.Instance.AddScore("�����ͷ", 10);
            }
            // ���üӷ���ȴ����Ӱ��ӷ֣�
            scoreCooldownTimer = scoreCooldown;
        }

        void UpdateTongueRootRotation()
        {
            if (Camera.main == null) return;

            // ��ͷ�����峯�����������һ��
            tongueRoot.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Vector3.up);
        }

        void UpdateTongue()
        {
            // Ŀ��ֵ
            float targetStretch = isExtending ? maxStretch : 0f;
            float targetForward = isExtending ? maxMoveForward : 0f;

            // ƽ����ֵ
            currentStretch = Mathf.MoveTowards(currentStretch, targetStretch, extendSpeed * Time.deltaTime);
            currentForward = Mathf.MoveTowards(currentForward, targetForward, moveSpeed * Time.deltaTime);

            // ���¾ֲ����� z
            Vector3 newScale = baseLocalScale;
            newScale.z = baseLocalScale.z + currentStretch;
            tongueModel.localScale = newScale;

            // ���¾ֲ�λ�� z���� tongueRoot ���� Z �ᣩ
            Vector3 newPos = baseLocalPosition;
            newPos.z = baseLocalPosition.z + currentForward;
            tongueModel.localPosition = newPos;

            HandleTongueSound();
        }

        void HandleTongueSound()
        {
            // ����ͷ��ʼ���ʱ����Ч��û����
            if (isExtending && !hasPlayedSound && currentStretch <= 0.01f)
            {
                hasPlayedSound = true;
                AudioManager.Instance?.PlayAudio(tongueSoundName, tongueRoot.position);
            }

            // ����ͷ��ȫ�ջ�ʱ�������´β���
            if (!isExtending && currentStretch <= 0.01f)
            {
                hasPlayedSound = false;
            }
        }

        private void OnDrawGizmos()
        {
            if (tongueRoot != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(tongueRoot.position, tongueRoot.forward * 2f);
            }
        }
    }
}
