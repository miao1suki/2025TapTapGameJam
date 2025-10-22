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

        private bool isExtending = false;
        private float currentStretch = 0f;
        private float currentForward = 0f;
        private Vector3 baseLocalScale;
        private Vector3 baseLocalPosition;

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
            UpdateTongueRootRotation();
            UpdateTongue();
        }

        void HandleInput()
        {
            isExtending = Input.GetMouseButton(0);
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
