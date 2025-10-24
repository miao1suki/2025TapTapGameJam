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
            Resolution[] resolutions = Screen.resolutions;
            int currentResolusionIndex = 0;
            Resolution current = Screen.currentResolution;

            List<int> temp = new();

            for (int i = resolutions.Length - 1; i >= 0; i--)
            {
                var target = resolutions[i];
                {
                    int m = target.width * target.height;
                    if (temp.Contains(m)) continue;
                    else temp.Add(m);
                }
                if (current.width == target.width && current.height == target.height) currentResolusionIndex = options.Count;
                dataResolutions.Add(new(target.width, target.height));
                options.Add(new($"{target.width}×{target.height}"));
            }

            resolutionDropdown.options = options;
            resolutionDropdown.value = currentResolusionIndex;

            {
                fullScreenModeOptions.Add(new("独占全屏"));
                fullScreenModeOptions.Add(new("无边框窗口（全屏）"));
                fullScreenModeOptions.Add(new("窗口"));
            }
            fullScreenModeDropdown.options = fullScreenModeOptions;
            fullScreenModeDropdown.value = Screen.fullScreenMode switch
            {
                FullScreenMode.ExclusiveFullScreen => 0,
                FullScreenMode.FullScreenWindow => 1,
                FullScreenMode.Windowed => 2,
                _ => 1,
            };

            resolutionDropdown.onValueChanged.AddListener(index =>
            {
                var resolution = dataResolutions[index];
                Screen.SetResolution(resolution.x, resolution.y, Screen.fullScreenMode);
            });
            fullScreenModeDropdown.onValueChanged.AddListener(index =>
            {
                Screen.fullScreenMode = fullScreenModeDropdown.value switch
                {
                    0 => FullScreenMode.ExclusiveFullScreen,
                    2 => FullScreenMode.Windowed,
                    _ => FullScreenMode.FullScreenWindow
                };
            });
        }
    }
}