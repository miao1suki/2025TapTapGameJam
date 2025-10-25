using UnityEngine;
using AchievementSystem;

namespace space
{
    public class Activate : MonoBehaviour
    {
        public void Active(Achievement achievement)
        {
            AchievementPanel.Enable(achievement);
        }
    }
}