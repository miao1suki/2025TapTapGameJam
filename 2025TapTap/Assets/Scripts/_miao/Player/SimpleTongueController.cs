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

        [Header("加分冷却（仅针对加分）")]
        public float scoreCooldown = 1.5f;   // 仅加分的冷却（秒）

        private bool isExtending = false;
        private float currentStretch = 0f;
        private float currentForward = 0f;
        private Vector3 baseLocalScale;
        private Vector3 baseLocalPosition;

        // 加分冷却计时器（Only for scoring）
        private float scoreCooldownTimer = 0f;

        private bool hasPlayedSound = false; // 防止在舌头未收回前重复播放
        public string tongueSoundName = "舌头吐出后，粘液粘黏声"; // 音效名

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

            // 更新加分冷却计时（只影响是否能再次加分，不影响舌头动作）
            if (scoreCooldownTimer > 0f)
                scoreCooldownTimer -= Time.deltaTime;

            UpdateTongueRootRotation();
            UpdateTongue();

            // 如果正在伸舌并且冷却到位，则触发一次加分并重置冷却
            if (isExtending && scoreCooldownTimer <= 0f)
            {
                TryScore();
            }
        }

        void HandleInput()
        {
            // 保持舌头伸缩响应立即生效（不受冷却限制）
            isExtending = Input.GetMouseButton(0);
        }

        void TryScore()
        {
            // 只处理存在的 ScoreTrigger 单例
            if (ScoreTrigger.Instance != null)
            {
                ScoreTrigger.Instance.AddScore("伸出舌头", 10);
            }
            // 重置加分冷却（仅影响加分）
            scoreCooldownTimer = scoreCooldown;
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

            HandleTongueSound();
        }

        void HandleTongueSound()
        {
            // 当舌头开始伸出时且音效还没播放
            if (isExtending && !hasPlayedSound && currentStretch <= 0.01f)
            {
                hasPlayedSound = true;
                AudioManager.Instance?.PlayAudio(tongueSoundName, tongueRoot.position);
            }

            // 当舌头完全收回时，允许下次播放
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
