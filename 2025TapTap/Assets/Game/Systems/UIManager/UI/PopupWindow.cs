using TMPro;
using UnityEngine;

namespace AchievementSystem
{
    public class PopupWindow : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text descriptionText;

        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();

            // 设置锚点与中心点都在中间
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);

            // 设置 anchoredPosition 为 (0,0)，表示居中
            rectTransform.anchoredPosition = Vector2.zero;
        }

        internal void SetAchievement(Achievement achievement)
        {
            nameText.text = achievement.achievementName;
            descriptionText.text = achievement.achievementDescription;
        }

        public void Close() => Invoke("CloseDelay", 1);
        private void CloseDelay() => Destroy(gameObject);

    }
}