using miao;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace AchievementSystem
{

    public class AchievementManager : MonoBehaviour
    {
        public static AchievementManager Ins { get; private set; }

        [SerializeField] private PopupWindow popup;

        [SerializeField] internal List<AchievementData> goldAchievements;
        [SerializeField] internal List<AchievementData> silverAchievements;
        [SerializeField] internal List<AchievementData> bronzeAchievements;

        private void Awake()
        {
            Ins = this;
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

        internal void PopupAchievement(Achievement achievement)
        {
            PopupWindow p = Instantiate(popup);  //pouupwindow
            p.SetAchievement(achievement);
            p.transform.SetParent(SingleCanvas.Ins.canvas.transform);
        }
    }

}