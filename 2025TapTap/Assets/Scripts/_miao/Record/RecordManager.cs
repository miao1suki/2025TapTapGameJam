using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

namespace miao
{
    public class RecordManager : MonoBehaviour
    {
        public static RecordManager Instance;
        private HashSet<string> collectedRecords = new HashSet<string>();
        private string savePath;

        private void Awake()
        {
            Instance = this;
            savePath = Path.Combine(Application.persistentDataPath, "records.json");
            LoadRecords();
            UpdateRecordText(); //  ����ʱ��������
        }

        public bool IsCollected(string recordID)
        {
            return collectedRecords.Contains(recordID);
        }

        public void CollectRecord(RecordData data)
        {
            if (!collectedRecords.Contains(data.recordID))
            {
                collectedRecords.Add(data.recordID);
                SaveRecords();
                UpdateRecordText(); //  ÿ���ռ�����Ʒʱ������ʾ
            }
        }

        public void MarkUncollected(RecordData data)
        {
            if (collectedRecords.Contains(data.recordID))
            {
                collectedRecords.Remove(data.recordID);
                SaveRecords();
                UpdateRecordText(); //  ���ȡ���ռ���Ҳ������ʾ
            }
        }

        private void SaveRecords()
        {
            RecordSaveData saveData = new RecordSaveData
            {
                collectedIDs = new List<string>(collectedRecords)
            };
            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(savePath, json);
        }

        private void LoadRecords()
        {
            collectedRecords.Clear();
            if (File.Exists(savePath))
            {
                string json = File.ReadAllText(savePath);
                RecordSaveData saveData = JsonUtility.FromJson<RecordSaveData>(json);
                foreach (var id in saveData.collectedIDs)
                    collectedRecords.Add(id);
            }
        }

        /// <summary>
        /// ���� UI �ϵ��ı�
        /// </summary>
        private void UpdateRecordText()
        {
            if (GameManager.Instance != null && GameManager.Instance.RecordTexe != null)
            {
                int count = collectedRecords.Count;
                GameManager.Instance.RecordTexe.GetComponent<TextMeshProUGUI>().text = $"���ռ��Ľ�����{count}/15";
            }
            else
            {
                Debug.LogWarning("RecordText δ�� GameManager ����ȷ���á�");
            }
        }
    }
}
