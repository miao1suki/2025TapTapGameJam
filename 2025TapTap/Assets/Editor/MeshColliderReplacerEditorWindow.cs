using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class MeshColliderReplacerEditorWindow : EditorWindow
{
    [MenuItem("Tools/MeshCollider�滻��")]
    public static void MakeWindow()
    {
        MeshColliderReplacerEditorWindow window = GetWindow<MeshColliderReplacerEditorWindow>();
        window.titleContent = new("MeshCollider�滻��");
        window.minSize = new Vector2(300, 42);
    }

    private void CreateGUI()
    {
        Button button = new();
        Label label = new();
        label.style.marginTop = 2;
        button.text = "�滻 MeshCollider �� BoxCollider";
        button.clicked += () =>
        {
            if (EditorApplication.isPlaying)
            {
                label.style.color = Color.yellow;
                label.text = "���˳�����ģʽ��ִ��";
                return;
            }

            // �ҳ����������� MeshCollider���������õĺ��������
            var targets = FindObjectsByType<MeshCollider>(FindObjectsSortMode.None);

            int replacedCount = 0;
            foreach (var target in targets)
            {
                GameObject go = target.gameObject;

                // ��¼��ǰ Collider ��һЩ��Ϣ����ѡ��
                bool isTrigger = target.isTrigger;
                PhysicMaterial mat = target.sharedMaterial;

                // ��� BoxCollider
                BoxCollider box = Undo.AddComponent<BoxCollider>(go);
                box.isTrigger = isTrigger;
                box.sharedMaterial = mat;

                // �Ƴ� MeshCollider
                DestroyImmediate(target);

               
                // ����޸�
                EditorUtility.SetDirty(go);
                replacedCount++;
            }

            label.style.color = Color.white;
            label.text = replacedCount > 0
                ? $"�ҵ����滻�� {replacedCount} �� MeshCollider"
                : "������δ�ҵ� MeshCollider";
        };

        rootVisualElement.Add(button);
        rootVisualElement.Add(label);
    }
}
