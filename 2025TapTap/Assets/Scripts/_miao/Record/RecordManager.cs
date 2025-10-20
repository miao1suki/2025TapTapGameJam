using System.Collections.Generic;
using UnityEngine;

namespace miao
{
    public class RecordManager : MonoBehaviour
    {
        public static RecordManager Instance;

        // 存储已经收集的ID
        private HashSet<string> collectedRecords = new HashSet<string>();

        private void Awake()
        {
            Instance = this;
            LoadRecords();
        }

        // 收集一个唱片
        public void CollectRecord(RecordData data)
        {
            if (!collectedRecords.Contains(data.recordID))
            {
                collectedRecords.Add(data.recordID);
                data.isCollected = true; // 更新SO状态
                SaveRecords();
            }
        }

        // 返回已收集数量
        public int CollectedCount => collectedRecords.Count;

        // 返回总数量，需要你在UI里传入所有RecordData数组
        public int TotalCount(RecordData[] allData) => allData.Length;

        // 保存到本地
        private void SaveRecords()
        {
            PlayerPrefs.SetString("CollectedRecords", string.Join(",", collectedRecords));
            PlayerPrefs.Save();
        }

        // 从本地加载
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

        // 同步SO状态
        public void SyncRecordData(RecordData[] allData)
        {
            foreach (var data in allData)
                data.isCollected = collectedRecords.Contains(data.recordID);
        }
    }
}
