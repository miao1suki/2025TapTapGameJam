using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AchievementSystem
{
    public class SingleAchievement : MonoBehaviour
    {
        [SerializeField] private Image borderImage;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image background;
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text achievementName;
        [SerializeField] private TMP_Text achievementDescription;
        [SerializeField] private Color goldColor;
        [SerializeField] private Color sliverColor;
        [SerializeField] private Color bronzeColor;
        [SerializeField] private Color goldBackgroundColor;
        [SerializeField] private Color sliverBackgroundColor;
        [SerializeField] private Color bronzeBackgroundColor;

        public void SetData(AchievementData data, AchievementLevel level)
        {
            if (!data.isCompleted) canvasGroup.alpha = 0.4f;
            icon.sprite = data.achievement.icon;
            borderImage.color = level switch
            {
                AchievementLevel.Gold => goldColor,
                AchievementLevel.Silver => sliverColor,
                AchievementLevel.Bronze => bronzeColor,
                _ => new(1, 1, 1, 0)
            };
            background.color = level switch
            {
                AchievementLevel.Gold => goldBackgroundColor,
                AchievementLevel.Silver => sliverBackgroundColor,
                AchievementLevel.Bronze => bronzeBackgroundColor,
                _ => new(1, 1, 1, 0)
            };
            achievementName.text = data.achievement.achievementName;
            achievementDescription.text = data.achievement.achievementDescription;
        }
    }
}