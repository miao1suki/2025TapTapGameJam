using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class AudioSourceRemoverEditorWindow : EditorWindow
{
    [MenuItem("Tools/AudioSource移除器")]
    public static void MakeWindow()
    {
        AudioSourceRemoverEditorWindow window = GetWindow<AudioSourceRemoverEditorWindow>();

        window.titleContent = new("AudioSource移除器");
        window.minSize = new Vector2(300, 42);
    }

    private void CreateGUI()
    {
        Button button = new();
        Label label = new();
        label.style.marginTop = 2;
        button.text = "移除";
        button.clicked += () =>
        {
            if (EditorApplication.isPlaying)
            {
                label.style.color = Color.yellow;
                label.text = "请退出播放模式后执行";
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

            label.text = targets.Length > 0 ? $"找到并移除了{targets.Length}个目标" : "场景中没有找到AudioSource";
        };
        rootVisualElement.Add(button);
        rootVisualElement.Add(label);
    }
}
