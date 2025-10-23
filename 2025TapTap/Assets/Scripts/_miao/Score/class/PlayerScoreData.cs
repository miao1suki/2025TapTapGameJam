using UnityEngine;
using System.IO;

namespace miao
{
    [CreateAssetMenu(fileName = "PlayerScoreData", menuName = "miao/Player Score Data")]
    public class PlayerScoreData : ScriptableObject
    {
        [Header("�����Ϣ")]
        public string playerName = "Player";

        [Header("����")]
        public int currentScore = 0;
        public int highScore = 0;

        private string savePath => Path.Combine(Application.persistentDataPath, "player_score.json");

        [System.Serializable]
        private class SaveData
        {
            public int highScore;
        }

        /// <summary>
        /// ��Ϸ��ʼʱ��ȡ JSON
        /// </summary>
        public void Load()
        {
            if (File.Exists(savePath))
            {
                string json = File.ReadAllText(savePath);
                SaveData data = JsonUtility.FromJson<SaveData>(json);
                highScore = data.highScore;
                Debug.Log($"��ȡ��߷�: {highScore}");
            }
            else
            {
                highScore = 0;
            }
            currentScore = 0; // ��Ϸ��ʼʱ���õ�ǰ����
        }

        /// <summary>
        /// ���ӵ�ǰ�����������Ը�����߷�
        /// </summary>
        public void AddScore(int score)
        {
            currentScore += score;
            if (currentScore > highScore)
            {
                highScore = currentScore;
                Debug.Log($"�¼�¼��{playerName} ����߷ָ���Ϊ {highScore}");
            }
        }

        /// <summary>
        /// ��Ϸ�˳��򱣴�ʱ����
        /// </summary>
        public void Save()
        {
            SaveData data = new SaveData() { highScore = highScore };
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(savePath, json);
            Debug.Log($"�ѱ�����߷�: {highScore}");
        }

        /// <summary>
        /// ��ȡ��ǰ����
        /// </summary>
        public int GetCurrentScore() => currentScore;

        /// <summary>
        /// ��ȡ��߷�
        /// </summary>
        public int GetHighScore() => highScore;
    }
}
