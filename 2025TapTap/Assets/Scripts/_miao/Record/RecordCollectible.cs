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
            yield return new WaitUntil(() => RecordManager.Instance != null);

            if (recordData == null)
            {
                Debug.LogError($"[RecordCollectible] {name} �� recordData δ���ã�");
                yield break;
            }


            if (RecordManager.Instance.IsCollected(recordData.recordID))
                gameObject.SetActive(false);
            else
                gameObject.SetActive(true);
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
                Collect(other.gameObject);

                AudioManager.Instance.PlayAudio("�����쳣ʱ����Ч", transform.position, false, 0.8f);
            }
        }

        void Collect(GameObject player)
        {
            RecordManager.Instance.CollectRecord(recordData);
            recordData.exceptionBehavior?.OnCollect(gameObject, recordData);


            // UI������Ӧ����
            if (RecordUIController.Instance != null)
                RecordUIController.Instance.UnlockRecord(recordData);

            ScoreTrigger.Instance.AddScore("��ý���!!!",114514);

            gameObject.SetActive(false);
        }
    }
}

