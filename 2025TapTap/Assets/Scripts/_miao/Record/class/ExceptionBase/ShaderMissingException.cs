using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miao
{
    [CreateAssetMenu(fileName = "ShaderMissingException", menuName = "Game/Record Exception/Shader Missing")]
    public class ShaderMissingException : RecordExceptionBase
    {
        // ��¼���滻�������ԭʼ����
        private Dictionary<GameObject, Material[]> originalMaterials = new Dictionary<GameObject, Material[]>();

        // ��׼���õĴ�����ʣ����� Inspector �ֶ�ָ����
        public Material errorMaterial;

        // �滻����ʱ�䣨�������ָ���
        public float effectDuration = 120f;

        public override void OnCollect(GameObject collector, RecordData recordData)
        {
            // �ҵ����д� Tag "Error" ������
            GameObject[] errorObjects = GameObject.FindGameObjectsWithTag("Error");

            if (errorObjects.Length == 0)
            {
                Debug.LogWarning(" û���ҵ� Tag Ϊ 'Error' �����塣");
                return;
            }

            if (errorMaterial == null)
            {
                Debug.LogError(" δָ��������ʣ�errorMaterial����");
                return;
            }

            originalMaterials.Clear();

            // �滻�����������
            foreach (GameObject obj in errorObjects)
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    // ����ԭʼ����
                    originalMaterials[obj] = renderer.sharedMaterials;

                    // �滻Ϊ�������
                    Material[] newMats = new Material[renderer.sharedMaterials.Length];
                    for (int i = 0; i < newMats.Length; i++)
                        newMats[i] = errorMaterial;

                    renderer.materials = newMats;
                }
            }

            //Debug.Log($"���滻 {originalMaterials.Count} ������Ĳ���Ϊ������ʡ�");

            // һ��ʱ���ָ�
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

            //Debug.Log("�ѻָ����б��滻�Ĳ��ʡ�");

            // ��ռ�¼
            originalMaterials.Clear();
        }
    }
}
