using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI; 
using TMPro;

namespace miao
{
    [CreateAssetMenu(fileName = "ForceResolutionException", menuName = "Game/Record Exception/Force Resolution")]
    public class ForceResolutionException : RecordExceptionBase
    {
        private Resolution originalResolution;
        private FullScreenMode originalMode;
        private WindowSetting windowSetting;

        public override void OnCollect(GameObject collector, RecordData recordData)
        {
            // ��¼��ǰ�ֱ��ʺ�ȫ��ģʽ
            originalResolution = Screen.currentResolution;
            originalMode = Screen.fullScreenMode;

            // �����·ֱ��� 720��2560
            Screen.SetResolution(720, 2560, originalMode);

            // ���� UI ��ʾ����� UI ���ڣ�
            windowSetting = Object.FindObjectOfType<WindowSetting>();
            if (windowSetting != null)
            {
                UpdateUIDropdown(720, 2560);
            }

            //Debug.Log($" [ForceResolutionException] �ֱ�����ǿ���޸�Ϊ 720��2560");

            // �ӳٻָ�
            StateController.Instance.StartCoroutine(
                StateController.Instance.ExecuteAfterCoroutine(60, () =>
                {
                    RestoreResolution();
                })
            );
        }

        private void RestoreResolution()
        {
            Screen.SetResolution(originalResolution.width, originalResolution.height, originalMode);
            if (windowSetting != null)
            {
                UpdateUIDropdown(originalResolution.width, originalResolution.height);
            }

            //Debug.Log($" [ForceResolutionException] �ֱ����ѻָ�Ϊ {originalResolution.width}��{originalResolution.height}");
        }

        /// <summary>
        /// ���� UI �����˵���ʾ��ǰ�ֱ���
        /// </summary>
        private void UpdateUIDropdown(int width, int height)
        {
            TMP_Dropdown resolutionDropdown = windowSetting.GetComponentInChildren<TMP_Dropdown>();
            if (resolutionDropdown == null) return;

            string resText = $"{width}��{height}";
            int index = -1;
            for (int i = 0; i < resolutionDropdown.options.Count; i++)
            {
                if (resolutionDropdown.options[i].text == resText)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
            {
                // �����������һ��
                resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(resText));
                index = resolutionDropdown.options.Count - 1;
            }

            resolutionDropdown.value = index;
            resolutionDropdown.RefreshShownValue();
        }
    }
}
