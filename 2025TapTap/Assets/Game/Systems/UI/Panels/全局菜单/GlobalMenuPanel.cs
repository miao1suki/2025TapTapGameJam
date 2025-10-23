using UnityEngine;
using UI;

namespace UI.Templates
{
    public class GlobalMenuPanel : MonoBehaviour
    {
        public void ExitGame() => GameManager.Instance.GameExit();
        private bool pause;
        void OnEnable()
        {
            if(!pause)
            {
                Time.timeScale = 0f;
                // œ‘ æ Û±Í
                Cursor.visible = true;
            }
        }

        void OnDisable()
        {
            if (!pause)
            {
                Time.timeScale = 1f;
                // “˛≤ÿ Û±Í
                Cursor.visible = false;
            }
        }

        public void Pause(bool pause)
        {
            this.pause = pause;
        }
    }
}