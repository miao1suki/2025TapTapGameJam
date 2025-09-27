using UnityEngine;

[CreateAssetMenu(fileName = "RenderSettingsSO", menuName = "Game Settings/Render Settings")]
public class RenderSettingsSO : ScriptableObject
{
    [Header("渲染设置")]
    [Tooltip("调整画面大小")]
    [Range(0.1f, 40f)]
    public float RT_Size = 1f;
}
