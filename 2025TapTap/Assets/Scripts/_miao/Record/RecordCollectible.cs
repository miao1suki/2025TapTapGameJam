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
            // 等待 RecordManager 初始化
            yield return new WaitUntil(() => RecordManager.Instance != null);

            startPos = transform.position;

            if (recordData == null)
            {
                Debug.LogError($"[RecordCollectible] {name} 的 recordData 未设置！");
                yield break;
            }

            // Debug：输出所有收集品数量
            Debug.Log($"当前场景收集品数量: {FindObjectsOfType<RecordCollectible>().Length}");

            // 已收集过则隐藏
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


            // UI解锁对应音乐
            if (RecordUIController.Instance != null)
                RecordUIController.Instance.UnlockRecord(recordData);

            gameObject.SetActive(false);
            Debug.Log("333");
        }
    }
}

