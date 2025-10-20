using miao;
using System.Collections;
using UnityEngine;
namespace miao 
{
    public class RecordCollectible : MonoBehaviour
    {
        public RecordData recordData;
        public float rotateSpeed = 60f;
        public float floatAmplitude = 0.25f;
        public float floatSpeed = 2f;

        private Vector3 startPos;

        private void Awake()
        {
            startPos = transform.position; // Awake ʱ��¼λ�ã���֤�� Instantiate ��
        }
        private IEnumerator Start()
        {
            // �ȴ� RecordManager ��ʼ��
            yield return new WaitUntil(() => RecordManager.Instance != null);

            startPos = transform.position;

            if (recordData == null)
            {
                Debug.LogError($"[RecordCollectible] {name} �� recordData δ���ã�");
                yield break;
            }

            // Debug����������ռ�Ʒ����
            Debug.Log($"��ǰ�����ռ�Ʒ����: {FindObjectsOfType<RecordCollectible>().Length}");

            // ���ռ���������
            if (recordData.isCollected)
            {
                gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
            transform.position = startPos + Vector3.up * Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("222");
                Collect(other.gameObject);
            }
        }

        void Collect(GameObject player)
        {
            RecordManager.Instance.CollectRecord(recordData);
            recordData.exceptionBehavior?.OnCollect(gameObject, recordData);
            recordData.isCollected = true;


            // UI������Ӧ����
            if (RecordUIController.Instance != null)
                RecordUIController.Instance.UnlockRecord(recordData);

            gameObject.SetActive(false);
            Debug.Log("333");
        }
    }
}

