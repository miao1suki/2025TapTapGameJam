using System.Collections.Generic;
using UnityEngine;

namespace AchievementSystem
{
    public class AchievementPanel : MonoBehaviour
    {
        [SerializeField] private RectTransform contentContainer;
        [SerializeField] private SingleAchievement template;

        private void OnEnable()
        {
            foreach (Transform child in contentContainer) Destroy(child.gameObject);
            Display();
        }
        private void Display()
        {
            CreateAchiv(AchievementManager.Ins.goldAchievements, AchievementLevel.Gold);
            CreateAchiv(AchievementManager.Ins.silverAchievements, AchievementLevel.Silver);
            CreateAchiv(AchievementManager.Ins.bronzeAchievements, AchievementLevel.Bronze);

            void CreateAchiv(List<AchievementData> achievements, AchievementLevel level)
            {
                foreach (var achiv in achievements)
                {
                    SingleAchievement view = Instantiate(template);
                    view.transform.SetParent(contentContainer);
                    view.SetData(achiv, level);
                }
            }
        }
    }
}