using TMPro;
using UnityEngine;

namespace AchievementSystem
{
    public class PopupWindow : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text descriptionText;

        internal void SetAchievement(Achievement achievement)
        {
            nameText.text = achievement.achievementName;
            descriptionText.text = achievement.achievementDescription;
        }

        public void Close() => Invoke("CloseDelay", 1);
        private void CloseDelay() => Destroy(gameObject);

    }
}