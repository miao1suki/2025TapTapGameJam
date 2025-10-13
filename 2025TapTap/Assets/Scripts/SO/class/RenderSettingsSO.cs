using UnityEngine;

[CreateAssetMenu(fileName = "RenderSettingsSO", menuName = "Game Settings/Render Settings")]
public class RenderSettingsSO : ScriptableObject
{
    [Header("渲染设置")]
    [Tooltip("调整画面大小")]
    [Range(0.1f, 24f)]
    public float RT_Size = 1f;

    [Tooltip("调整像素密度")]
    [Range(1, 24)]
    public int RT_Pixel = 3;

    [Header("基础分辨率")]
    [Tooltip("基础分辨率，保持16:9比例")]
    public int BaseWidth = 160;
    public int BaseHeight = 90;

    [Header("Pixel Rendering 设置")]
    public PixelRendering.PixelRenderingFeature.PixelSettings pixelSettings;

    public void ApplySettings()
    {
        if (pixelSettings == null)
        {
            Debug.LogWarning("PixelSettings 未赋值！");
            return;
        }

        // 使用 RT_Pixel 控制最终宽高
        int finalWidth = BaseWidth * RT_Pixel;
        int finalHeight = BaseHeight * RT_Pixel;

        // 设置 PixelSettings 的宽高（通过反射访问私有字段）
        var widthField = typeof(PixelRendering.PixelRenderingFeature.PixelSettings)
            .GetField("_width", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var heightField = typeof(PixelRendering.PixelRenderingFeature.PixelSettings)
            .GetField("_height", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (widthField != null && heightField != null)
        {
            widthField.SetValue(pixelSettings, finalWidth);
            heightField.SetValue(pixelSettings, finalHeight);
        }

        // 更新 PixelSettings 的 RenderTexture
        var rtField = typeof(PixelRendering.PixelRenderingFeature.PixelSettings)
            .GetField("_rt", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (rtField != null)
        {
            RenderTexture rt = rtField.GetValue(pixelSettings) as RenderTexture;
            if (rt != null)
            {
                rt.Release();
                rt.width = finalWidth;
                rt.height = finalHeight;
                rt.Create();
            }
        }

        Debug.Log($"应用分辨率（像素密度控制）：{finalWidth}x{finalHeight}，RT_Pixel={RT_Pixel}");
    }
}
