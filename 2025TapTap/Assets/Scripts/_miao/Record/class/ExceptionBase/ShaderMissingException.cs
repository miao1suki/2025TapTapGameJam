using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miao
{
    [CreateAssetMenu(fileName = "ShaderMissingException", menuName = "Game/Record Exception/Shader Missing")]
    public class ShaderMissingException : RecordExceptionBase
    {
        // 记录被替换的物体和原始材质
        private Dictionary<GameObject, Material[]> originalMaterials = new Dictionary<GameObject, Material[]>();

        // 你准备好的错误材质（可在 Inspector 手动指定）
        public Material errorMaterial;

        // 替换持续时间（多少秒后恢复）
        public float effectDuration = 120f;

        public override void OnCollect(GameObject collector, RecordData recordData)
        {
            // 找到所有带 Tag "Error" 的物体
            GameObject[] errorObjects = GameObject.FindGameObjectsWithTag("Error");

            if (errorObjects.Length == 0)
            {
                Debug.LogWarning(" 没有找到 Tag 为 'Error' 的物体。");
                return;
            }

            if (errorMaterial == null)
            {
                Debug.LogError(" 未指定错误材质（errorMaterial）！");
                return;
            }

            originalMaterials.Clear();

            // 替换所有物体材质
            foreach (GameObject obj in errorObjects)
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    // 保存原始材质
                    originalMaterials[obj] = renderer.sharedMaterials;

                    // 替换为错误材质
                    Material[] newMats = new Material[renderer.sharedMaterials.Length];
                    for (int i = 0; i < newMats.Length; i++)
                        newMats[i] = errorMaterial;

                    renderer.materials = newMats;
                }
            }

            //Debug.Log($"已替换 {originalMaterials.Count} 个物体的材质为错误材质。");

            // 一段时间后恢复
            StateController.Instance.StartCoroutine(RestoreAfterDelay(effectDuration));
        }

        private IEnumerator RestoreAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            foreach (var kvp in originalMaterials)
            {
                if (kvp.Key != null)
                {
                    Renderer renderer = kvp.Key.GetComponent<Renderer>();
                    if (renderer != null)
                        renderer.materials = kvp.Value;
                }
            }

            //Debug.Log("已恢复所有被替换的材质。");

            // 清空记录
            originalMaterials.Clear();
        }
    }
}
