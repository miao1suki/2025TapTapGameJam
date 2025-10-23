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
                // ��ʾ���
                Cursor.visible = true;
            }
        }

        void OnDisable()
        {
            if (!pause)
            {
                Time.timeScale = 1f;
                // �������
                Cursor.visible = false;
            }
        }

        public void Pause(bool pause)
        {
            this.pause = pause;
        }
    }
}