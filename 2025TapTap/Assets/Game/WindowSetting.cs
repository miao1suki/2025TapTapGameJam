using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI
{
    public class WindowSetting : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private TMP_Dropdown fullScreenModeDropdown;

        private List<TMP_Dropdown.OptionData> options = new();
        private List<TMP_Dropdown.OptionData> fullScreenModeOptions = new();
        private List<Vector2Int> dataResolutions = new();

        private void Start()
        {
            // ��ǿ��ȫ������
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;

            InitializeResolutionDropdown();
            InitializeFullScreenDropdown();
        }

        private void InitializeResolutionDropdown()
        {
            Resolution[] resolutions = Screen.resolutions;
            int currentResolutionIndex = 0;
            Resolution current = Screen.currentResolution;

            List<int> added = new();

            // ���������ȥ��
            for (int i = resolutions.Length - 1; i >= 0; i--)
            {
                var r = resolutions[i];
                int m = r.width * r.height;
                if (added.Contains(m)) continue;
                added.Add(m);

                dataResolutions.Add(new Vector2Int(r.width, r.height));
                options.Add(new TMP_Dropdown.OptionData($"{r.width}��{r.height}"));

                if (r.width == current.width && r.height == current.height)
                    currentResolutionIndex = options.Count - 1;
            }

            resolutionDropdown.options = options;

            // ����¼���������Ĭ��ֵ�����ⴥ��
            resolutionDropdown.onValueChanged.RemoveAllListeners();
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();

            // ���¼�
            resolutionDropdown.onValueChanged.AddListener(index =>
            {
                var resolution = dataResolutions[index];
                Screen.SetResolution(resolution.x, resolution.y, Screen.fullScreenMode);
            });
        }

        private void InitializeFullScreenDropdown()
        {
            fullScreenModeOptions.Clear();
            fullScreenModeOptions.Add(new TMP_Dropdown.OptionData("��ռȫ��"));
            fullScreenModeOptions.Add(new TMP_Dropdown.OptionData("�ޱ߿򴰿ڣ�ȫ����"));
            fullScreenModeOptions.Add(new TMP_Dropdown.OptionData("����"));

            fullScreenModeDropdown.options = fullScreenModeOptions;

            // ����¼���������Ĭ��ֵ
            fullScreenModeDropdown.onValueChanged.RemoveAllListeners();
            fullScreenModeDropdown.value = 0; // Ĭ�϶�ռȫ��
            fullScreenModeDropdown.RefreshShownValue();

            // ���¼�
            fullScreenModeDropdown.onValueChanged.AddListener(index =>
            {
                Screen.fullScreenMode = fullScreenModeDropdown.value switch
                {
                    0 => FullScreenMode.ExclusiveFullScreen,
                    1 => FullScreenMode.FullScreenWindow,
                    2 => FullScreenMode.Windowed,
                    _ => FullScreenMode.ExclusiveFullScreen
                };
            });
        }
    }
}
