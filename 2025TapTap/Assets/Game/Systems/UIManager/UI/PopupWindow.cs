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

            // ����ê�������ĵ㶼���м�
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);

            // ���� anchoredPosition Ϊ (0,0)����ʾ����
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