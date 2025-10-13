using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace miao
{
    public class CamSize : MonoBehaviour
    {
        [Header("关联 GameManager 和摄像机")]
        public Camera targetCamera;
        private Camera main_Cam;

        [Header("滚轮控制参数")]
        [Tooltip("每次滚轮增量对应的 RT_Size 改变值")]
        public float scrollStep = 0.1f;

        [Header("玩家引用")]
        public Transform player;  // 玩家Transform

        [Header("UI指引")]
        public RectTransform offscreenIndicator; // UI箭头（Canvas内）

        [Header("参数设置")]
        public float autoAdjustSpeed = 2f; // 摄像机RT_Size增长速度
        public float maxRTSize = 24f;      // 最大视野大小
        public float topThreshold = 0.95f; // 玩家在屏幕中y值超过此阈值触发拉远

        [Header("屏幕边缘偏移")]
        public float edgeOffset = 50f;

        // 按键检测 Flag
        private bool wheelUp_Flag;
        private bool wheelDown_Flag;

        private void Start()
        {
            wheelUp_Flag = false;
            wheelDown_Flag = false;

            if (offscreenIndicator != null)
                offscreenIndicator.gameObject.SetActive(false);

            main_Cam = this.gameObject.GetComponent<Camera>();
        }

        void Update()
        {
            // ------------------ 滚轮控制逻辑 ------------------
            // 每帧重置标志
            ResetFlags();

            // 检测按下
            DetectWheelDown();

            // 检测持续
            DetectWheelKeep();
            if (GameManager.Instance == null || targetCamera == null) return;
            if (!GameManager.Instance.autoPixelControl) return;

            // 根据 Flag 调整 RT_Size
            if (wheelUp_Flag)
            {
                GameManager.Instance.Renderseter.RT_Size -= scrollStep;
            }
            if (wheelDown_Flag)
            {
                GameManager.Instance.Renderseter.RT_Size += scrollStep;
            }

            // 限制范围
            GameManager.Instance.Renderseter.RT_Size = Mathf.Clamp(GameManager.Instance.Renderseter.RT_Size, 0.1f, 24f);

            // ------------------ 自动调整逻辑 ------------------
            AutoAdjustSize();

            // 应用设置
            GameManager.Instance.setCamSize();
        }

        private void AutoAdjustSize()
        {
            if (player == null)
            {
                Debug.LogWarning("未绑定Player");
                return;
            }

            // 将玩家位置转换到视口空间（0~1）
            Vector3 viewPos = main_Cam.WorldToViewportPoint(player.position);
            float currentSize = GameManager.Instance.Renderseter.RT_Size;

            // 判断玩家是否超出屏幕（任意方向）
            bool isOutOfView =
                viewPos.x < 0f || viewPos.x > 1f ||
                viewPos.y < 0f || viewPos.y > 1f ||
                viewPos.z < 0f; // 在摄像机背后也算出屏

            if (isOutOfView)
            {
                //  尝试自动拉远视野（任意方向）
                if (currentSize < maxRTSize)
                {
                    GameManager.Instance.Renderseter.RT_Size = Mathf.Lerp(
                        currentSize,
                        Mathf.Min(maxRTSize, currentSize + 0.5f),
                        Time.deltaTime * autoAdjustSpeed
                    );
                }

                //  显示UI箭头
                if (offscreenIndicator != null)
                {
                    offscreenIndicator.gameObject.SetActive(true);
                    UpdateOffscreenIndicator(viewPos);
                }
            }
            else
            {
                // 玩家在画面内则隐藏UI箭头
                if (offscreenIndicator != null)
                    offscreenIndicator.gameObject.SetActive(false);
            }
        }


        private void UpdateOffscreenIndicator(Vector3 viewPos)
        {
            if (offscreenIndicator == null) return;

            // 视口坐标转成屏幕坐标
            Vector2 screenPos = new Vector2(
                (viewPos.x - 0.5f) * Screen.width,
                (viewPos.y - 0.5f) * Screen.height
            );

            // 限制箭头在屏幕边缘范围内
            float halfW = Screen.width * 0.5f - edgeOffset;
            float halfH = Screen.height * 0.5f - edgeOffset;
            Vector2 clampedPos = screenPos;
            clampedPos.x = Mathf.Clamp(clampedPos.x, -halfW, halfW);
            clampedPos.y = Mathf.Clamp(clampedPos.y, -halfH, halfH);

            offscreenIndicator.anchoredPosition = clampedPos;

            // 箭头旋转指向玩家方向
            Vector2 dir = screenPos.normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            offscreenIndicator.rotation = Quaternion.Euler(0, 0, angle - 90);
        }



        #region 鼠标滚轮检测方法

        private void ResetFlags()
        {
            wheelUp_Flag = false;
            wheelDown_Flag = false;
        }

        private void DetectWheelDown()
        {
            float scroll = Input.mouseScrollDelta.y;
            if (scroll > 0f) wheelUp_Flag = true;
            if (scroll < 0f) wheelDown_Flag = true;
        }

        private void DetectWheelKeep()
        {
            // 滚轮不需要像键盘那样持续按住，但保留接口以防以后拓展
        }

        #endregion
    }
}

