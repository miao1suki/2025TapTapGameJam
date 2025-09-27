using UnityEngine;

[CreateAssetMenu(fileName = "RenderSettingsSO", menuName = "Game Settings/Render Settings")]
public class RenderSettingsSO : ScriptableObject
{
    [Header("��Ⱦ����")]
    [Tooltip("���������С")]
    [Range(0.1f, 40f)]
    public float RT_Size = 1f;
}
