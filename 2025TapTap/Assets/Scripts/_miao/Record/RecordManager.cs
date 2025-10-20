using System.Collections.Generic;
using UnityEngine;

namespace miao
{
    public class RecordManager : MonoBehaviour
    {
        public static RecordManager Instance;

        // �洢�Ѿ��ռ���ID
        private HashSet<string> collectedRecords = new HashSet<string>();

        private void Awake()
        {
            Instance = this;
            LoadRecords();
        }

        // �ռ�һ����Ƭ
        public void CollectRecord(RecordData data)
        {
            if (!collectedRecords.Contains(data.recordID))
            {
                collectedRecords.Add(data.recordID);
                data.isCollected = true; // ����SO״̬
                SaveRecords();
            }
        }

        // �������ռ�����
        public int CollectedCount => collectedRecords.Count;

        // ��������������Ҫ����UI�ﴫ������RecordData����
        public int TotalCount(RecordData[] allData) => allData.Length;

        // ���浽����
        private void SaveRecords()
        {
            PlayerPrefs.SetString("CollectedRecords", string.Join(",", collectedRecords));
            PlayerPrefs.Save();
        }

        // �ӱ��ؼ���
        private void LoadRecords()
        {
            collectedRecords.Clear();
            string saved = PlayerPrefs.GetString("CollectedRecords", "");
            if (!string.IsNullOrEmpty(saved))
            {
                foreach (string id in saved.Split(','))
                    collectedRecords.Add(id);
            }
        }

        // ͬ��SO״̬
        public void SyncRecordData(RecordData[] allData)
        {
            foreach (var data in allData)
                data.isCollected = collectedRecords.Contains(data.recordID);
        }
    }
}
