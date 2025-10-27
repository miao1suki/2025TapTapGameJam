using UnityEngine;


namespace miao
{
    public class CameraController : MonoBehaviour
    {
        [Header("玩家目标")]
        public Transform player;

        [Header("跟随参数")]
        public Vector3 followOffset = new Vector3(0, 5, -8);
        public float followSmooth = 5f;

        [Header("旋转控制")]
        public float mouseSensitivity = 2.5f;
        public float minPitch = -30f;
        public float maxPitch = 70f;

        [Header("遮挡调整")]
        public LayerMask obstacleMask;
        public float minDistance = 2f;
        public float maxDistance = 8f;
        public float zoomSmooth = 10f;

        [Header("模式")]
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

            // 如果是正交相机，预留方法
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
            // 正交相机，预留方法
        }

        private void HandlePerspectiveMode()
        {
            // 透视相机，鼠标移动控制相机旋转
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            yaw += mouseX;
            pitch -= mouseY;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            // 计算相机目标方向
            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
            Vector3 targetPos = player.position - rotation * Vector3.forward * currentDistance + Vector3.up * 2.0f;

            // 检测遮挡物（射线从玩家到相机）
            RaycastHit hit;
            Vector3 direction = (targetPos - player.position).normalized;
            float targetDistance = maxDistance;

            if (Physics.Raycast(player.position + Vector3.up * 1.5f, direction, out hit, maxDistance, obstacleMask))
            {
                targetDistance = Mathf.Clamp(hit.distance * 0.8f, minDistance, maxDistance);
            }

            currentDistance = Mathf.Lerp(currentDistance, targetDistance, Time.deltaTime * zoomSmooth);

            // 应用最终位置
            transform.position = player.position - rotation * Vector3.forward * currentDistance + Vector3.up * 2.0f;
            transform.LookAt(player.position + Vector3.up * 1.5f);
        }
    }
}
