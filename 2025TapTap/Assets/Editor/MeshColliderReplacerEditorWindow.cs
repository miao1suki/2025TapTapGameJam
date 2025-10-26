using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class MeshColliderReplacerEditorWindow : EditorWindow
{
    [MenuItem("Tools/MeshCollider替换器")]
    public static void MakeWindow()
    {
        MeshColliderReplacerEditorWindow window = GetWindow<MeshColliderReplacerEditorWindow>();
        window.titleContent = new("MeshCollider替换器");
        window.minSize = new Vector2(300, 42);
    }

    private void CreateGUI()
    {
        Button button = new();
        Label label = new();
        label.style.marginTop = 2;
        button.text = "替换 MeshCollider → BoxCollider";
        button.clicked += () =>
        {
            if (EditorApplication.isPlaying)
            {
                label.style.color = Color.yellow;
                label.text = "请退出播放模式后执行";
                return;
            }

            // 找出场景中所有 MeshCollider，包括禁用的和子物体的
            var targets = FindObjectsByType<MeshCollider>(FindObjectsSortMode.None);

            int replacedCount = 0;
            foreach (var target in targets)
            {
                GameObject go = target.gameObject;

                // 记录当前 Collider 的一些信息（可选）
                bool isTrigger = target.isTrigger;
                PhysicMaterial mat = target.sharedMaterial;

                // 添加 BoxCollider
                BoxCollider box = Undo.AddComponent<BoxCollider>(go);
                box.isTrigger = isTrigger;
                box.sharedMaterial = mat;

                // 移除 MeshCollider
                DestroyImmediate(target);

               
                // 标记修改
                EditorUtility.SetDirty(go);
                replacedCount++;
            }

            label.style.color = Color.white;
            label.text = replacedCount > 0
                ? $"找到并替换了 {replacedCount} 个 MeshCollider"
                : "场景中未找到 MeshCollider";
        };

        rootVisualElement.Add(button);
        rootVisualElement.Add(label);
    }
}
