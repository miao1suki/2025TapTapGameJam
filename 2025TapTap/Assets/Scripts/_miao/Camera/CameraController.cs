using UnityEngine;


namespace miao
{
    public class CameraController : MonoBehaviour
    {
        [Header("���Ŀ��")]
        public Transform player;

        [Header("�������")]
        public Vector3 followOffset = new Vector3(0, 5, -8);
        public float followSmooth = 5f;

        [Header("��ת����")]
        public float mouseSensitivity = 2.5f;
        public float minPitch = -30f;
        public float maxPitch = 70f;

        [Header("�ڵ�����")]
        public LayerMask obstacleMask;
        public float minDistance = 2f;
        public float maxDistance = 8f;
        public float zoomSmooth = 10f;

        [Header("ģʽ")]
        public bool isPerspectiveMode = false;

        private float yaw;
        private float pitch;
        private float currentDistance;

        [SerializeField] private GameObject camWater;
        public static CameraController Instance;
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            player = Player.Instance.transform;
            currentDistance = followOffset.magnitude;
        }

        private void LateUpdate()
        {
            if (!player) return;

            // ��������������Ԥ������
            if (!isPerspectiveMode)
            {

                return;
            }

            HandlePerspectiveMode();
        }

        public void setCamWater(bool active)
        {
            if (!active) return;
            camWater.SetActive(active);
        }


        private void HandleOrthographicMode()
        {
            // ���������Ԥ������
        }

        private void HandlePerspectiveMode()
        {
            // ͸�����������ƶ����������ת
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            yaw += mouseX;
            pitch -= mouseY;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            // �������Ŀ�귽��
            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
            Vector3 targetPos = player.position - rotation * Vector3.forward * currentDistance + Vector3.up * 2.0f;

            // ����ڵ�����ߴ���ҵ������
            RaycastHit hit;
            Vector3 direction = (targetPos - player.position).normalized;
            float targetDistance = maxDistance;

            if (Physics.Raycast(player.position + Vector3.up * 1.5f, direction, out hit, maxDistance, obstacleMask))
            {
                targetDistance = Mathf.Clamp(hit.distance * 0.8f, minDistance, maxDistance);
            }

            currentDistance = Mathf.Lerp(currentDistance, targetDistance, Time.deltaTime * zoomSmooth);

            // Ӧ������λ��
            transform.position = player.position - rotation * Vector3.forward * currentDistance + Vector3.up * 2.0f;
            transform.LookAt(player.position + Vector3.up * 1.5f);
        }
    }
}
