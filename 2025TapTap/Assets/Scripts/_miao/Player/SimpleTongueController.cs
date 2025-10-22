using UnityEngine;

namespace miao
{
    public class SimpleTongueController : MonoBehaviour
    {
        [Header("舌头引用")]
        public Transform tongueModel;  // 舌头模型
        public Transform tongueRoot;   // 舌头根部（通常挂在玩家嘴巴）

        [Header("参数设置")]
        public float extendSpeed = 5f;       // 缩放速度
        public float moveSpeed = 2.5f;       // 位置移动速度
        public float maxStretch = 1f;        // 最大 z 缩放增量
        public float maxMoveForward = 0.5f;  // 最大前移距离

        private bool isExtending = false;
        private float currentStretch = 0f;
        private float currentForward = 0f;
        private Vector3 baseLocalScale;
        private Vector3 baseLocalPosition;

        void Start()
        {
            if (tongueModel == null || tongueRoot == null)
            {
                Debug.LogError("请确保 tongueModel 和 tongueRoot 已经赋值");
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

            // 舌头父物体朝向与主摄像机一致
            tongueRoot.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Vector3.up);
        }

        void UpdateTongue()
        {
            // 目标值
            float targetStretch = isExtending ? maxStretch : 0f;
            float targetForward = isExtending ? maxMoveForward : 0f;

            // 平滑插值
            currentStretch = Mathf.MoveTowards(currentStretch, targetStretch, extendSpeed * Time.deltaTime);
            currentForward = Mathf.MoveTowards(currentForward, targetForward, moveSpeed * Time.deltaTime);

            // 更新局部缩放 z
            Vector3 newScale = baseLocalScale;
            newScale.z = baseLocalScale.z + currentStretch;
            tongueModel.localScale = newScale;

            // 更新局部位置 z（沿 tongueRoot 本地 Z 轴）
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
