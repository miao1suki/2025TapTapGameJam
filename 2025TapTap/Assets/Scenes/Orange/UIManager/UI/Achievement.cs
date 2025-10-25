using System;
using UnityEngine;

namespace AchievementSystem
{
    [CreateAssetMenu(fileName = "新成就", menuName = "创建成就")]
    public class Achievement : ScriptableObject
    {
        [SerializeField] internal Sprite icon;
        [SerializeField] internal string achievementName;
        [SerializeField, TextArea(3, 10)] internal string achievementDescription;
        [SerializeField, TextArea(3, 10)] internal string completionPrompt;
    }

    [Serializable]
    public class AchievementData
    {
        [SerializeField] internal Achievement achievement;
        [SerializeField] internal bool isCompleted;

        internal void Enable()
        {
            if (isCompleted) return;
            isCompleted = true;
            AchievementPanel.Ins.PopupAchievement(achievement);
        }
    }

    public enum AchievementLevel
    {
        Gold,
        Silver,
        Bronze
    }
}