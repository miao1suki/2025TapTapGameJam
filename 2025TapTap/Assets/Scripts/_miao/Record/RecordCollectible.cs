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
            startPos = transform.position; // Awake 时记录位置，保证在 Instantiate 后
        }
        private IEnumerator Start()
        {
            yield return new WaitUntil(() => RecordManager.Instance != null);

            if (recordData == null)
            {
                Debug.LogError($"[RecordCollectible] {name} 的 recordData 未设置！");
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

                AudioManager.Instance.PlayAudio("出现异常时的音效", transform.position, false, 0.8f);
            }
        }

        void Collect(GameObject player)
        {
            RecordManager.Instance.CollectRecord(recordData);
            recordData.exceptionBehavior?.OnCollect(gameObject, recordData);


            // UI解锁对应音乐
            if (RecordUIController.Instance != null)
                RecordUIController.Instance.UnlockRecord(recordData);

            ScoreTrigger.Instance.AddScore("获得奖杯!!!",114514);

            gameObject.SetActive(false);
        }
    }
}

