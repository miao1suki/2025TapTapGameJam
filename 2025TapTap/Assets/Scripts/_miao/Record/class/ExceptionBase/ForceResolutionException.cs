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
            // 记录当前分辨率和全屏模式
            originalResolution = Screen.currentResolution;
            originalMode = Screen.fullScreenMode;

            // 设置新分辨率 720×2560
            Screen.SetResolution(720, 2560, originalMode);

            // 更新 UI 显示（如果 UI 存在）
            windowSetting = Object.FindObjectOfType<WindowSetting>();
            if (windowSetting != null)
            {
                UpdateUIDropdown(720, 2560);
            }

            //Debug.Log($" [ForceResolutionException] 分辨率已强制修改为 720×2560");

            // 延迟恢复
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

            //Debug.Log($" [ForceResolutionException] 分辨率已恢复为 {originalResolution.width}×{originalResolution.height}");
        }

        /// <summary>
        /// 更新 UI 下拉菜单显示当前分辨率
        /// </summary>
        private void UpdateUIDropdown(int width, int height)
        {
            TMP_Dropdown resolutionDropdown = windowSetting.GetComponentInChildren<TMP_Dropdown>();
            if (resolutionDropdown == null) return;

            string resText = $"{width}×{height}";
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
                // 不存在则添加一项
                resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(resText));
                index = resolutionDropdown.options.Count - 1;
            }

            resolutionDropdown.value = index;
            resolutionDropdown.RefreshShownValue();
        }
    }
}
