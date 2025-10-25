using System.Collections.Generic;
using UnityEngine;

namespace AchievementSystem
{
    public class AchievementPanel : MonoBehaviour
    {
        public static AchievementPanel Ins { get; private set; }

        [SerializeField] private PopupWindow popup;
        [SerializeField] private Canvas canvas;

        [SerializeField] private RectTransform contentContainer;
        [SerializeField] private SingleAchievement template;

        [SerializeField] private List<AchievementData> goldAchievements;
        [SerializeField] private List<AchievementData> silverAchievements;
        [SerializeField] private List<AchievementData> bronzeAchievements;

        private void Awake() { Ins = this; gameObject.SetActive(false); }
       


        private void OnEnable()
        {
            foreach (Transform child in contentContainer) Destroy(child.gameObject);
            Display();
        }
        public static void Enable(Achievement achievement) => Ins.Find(achievement)?.Enable();

        private AchievementData Find(Achievement achievement)
        {
            AchievementData target;
            target = goldAchievements.Find(data => data.achievement == achievement);
            if (target != null) return target;
            target = silverAchievements.Find(data => data.achievement == achievement);
            if (target != null) return target;
            target = bronzeAchievements.Find(data => data.achievement == achievement);
            if (target != null) return target;
            return null;

        }
        private void Display()
        {
            CreateAchiv(goldAchievements, AchievementLevel.Gold);
            CreateAchiv(silverAchievements, AchievementLevel.Silver);
            CreateAchiv(bronzeAchievements, AchievementLevel.Bronze);

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
        internal void PopupAchievement(Achievement achievement)
        {
            PopupWindow p = Instantiate(popup);  //pouupwindow
            p.SetAchievement(achievement);
            p.transform.SetParent(canvas.transform);
        }
    }
}