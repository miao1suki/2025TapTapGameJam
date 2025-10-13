using UnityEngine;

[CreateAssetMenu(fileName = "RenderSettingsSO", menuName = "Game Settings/Render Settings")]
public class RenderSettingsSO : ScriptableObject
{
    [Header("��Ⱦ����")]
    [Tooltip("���������С")]
    [Range(0.1f, 24f)]
    public float RT_Size = 1f;

    [Tooltip("���������ܶ�")]
    [Range(1, 24)]
    public int RT_Pixel = 3;

    [Header("�����ֱ���")]
    [Tooltip("�����ֱ��ʣ�����16:9����")]
    public int BaseWidth = 160;
    public int BaseHeight = 90;

    [Header("Pixel Rendering ����")]
    public PixelRendering.PixelRenderingFeature.PixelSettings pixelSettings;

    public void ApplySettings()
    {
        if (pixelSettings == null)
        {
            Debug.LogWarning("PixelSettings δ��ֵ��");
            return;
        }

        // ʹ�� RT_Pixel �������տ��
        int finalWidth = BaseWidth * RT_Pixel;
        int finalHeight = BaseHeight * RT_Pixel;

        // ���� PixelSettings �Ŀ�ߣ�ͨ���������˽���ֶΣ�
        var widthField = typeof(PixelRendering.PixelRenderingFeature.PixelSettings)
            .GetField("_width", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var heightField = typeof(PixelRendering.PixelRenderingFeature.PixelSettings)
            .GetField("_height", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (widthField != null && heightField != null)
        {
            widthField.SetValue(pixelSettings, finalWidth);
            heightField.SetValue(pixelSettings, finalHeight);
        }

        // ���� PixelSettings �� RenderTexture
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

        Debug.Log($"Ӧ�÷ֱ��ʣ������ܶȿ��ƣ���{finalWidth}x{finalHeight}��RT_Pixel={RT_Pixel}");
    }
}
