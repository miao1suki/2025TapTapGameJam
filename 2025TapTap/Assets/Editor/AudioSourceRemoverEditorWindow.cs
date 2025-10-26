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
        window.minSize = new Vector2(300, 42);
    }

    private void CreateGUI()
    {
        Button button = new();
        Label label = new();
        label.style.marginTop = 2;
        button.text = "�Ƴ�";
        button.clicked += () =>
        {
            if (EditorApplication.isPlaying)
            {
                label.style.color = Color.yellow;
                label.text = "���˳�����ģʽ��ִ��";
                return;
            }
            var targets = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
            foreach (var target in targets)
            {
                GameObject targetGameobject = target.gameObject;
                DestroyImmediate(target);
                EditorUtility.SetDirty(targetGameobject);
            }
            label.style.color = Color.white;

            label.text = targets.Length > 0 ? $"�ҵ����Ƴ���{targets.Length}��Ŀ��" : "������û���ҵ�AudioSource";
        };
        rootVisualElement.Add(button);
        rootVisualElement.Add(label);
    }
}
