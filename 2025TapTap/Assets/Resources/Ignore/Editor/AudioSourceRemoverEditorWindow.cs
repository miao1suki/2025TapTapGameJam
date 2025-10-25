using InventorySystem.Editor;
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
        window.minSize = new Vector2(300, 30);
        window.maxSize = new Vector2(300, 30);
    }

    private void CreateGUI()
    {
        Button button = new();
        button.text = "移除";
        button.clicked += () =>
        {
            if (EditorApplication.isPlaying)
            {
                Debug.LogWarning("请退出播放模式后执行");
                return;
            }
            var targets = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
            foreach (var target in targets) DestroyImmediate(target);
            Debug.Log($"找到并移除了{targets.Length}个目标");
        };
        rootVisualElement.Add(button);
    }
}
