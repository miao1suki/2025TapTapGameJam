using InventorySystem.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class AudioSourceRemoverEditorWindow : EditorWindow
{
    [MenuItem("Tools/AudioSource�Ƴ���")]
    public static void MakeWindow()
    {
        AudioSourceRemoverEditorWindow window = GetWindow<AudioSourceRemoverEditorWindow>();

        window.titleContent = new("AudioSource�Ƴ���");
        window.minSize = new Vector2(300, 30);
        window.maxSize = new Vector2(300, 30);
    }

    private void CreateGUI()
    {
        Button button = new();
        button.text = "�Ƴ�";
        button.clicked += () =>
        {
            if (EditorApplication.isPlaying)
            {
                Debug.LogWarning("���˳�����ģʽ��ִ��");
                return;
            }
            var targets = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
            foreach (var target in targets) DestroyImmediate(target);
            Debug.Log($"�ҵ����Ƴ���{targets.Length}��Ŀ��");
        };
        rootVisualElement.Add(button);
    }
}
