using UnityEngine;
using System.IO;

namespace miao
{
    [CreateAssetMenu(fileName = "PlayerScoreData", menuName = "miao/Player Score Data")]
    public class PlayerScoreData : ScriptableObject
    {
        [Header("玩家信息")]
        public string playerName = "Player";

        [Header("分数")]
        public int currentScore = 0;
        public int highScore = 0;

        private string savePath => Path.Combine(Application.persistentDataPath, "player_score.json");

        [System.Serializable]
        private class SaveData
        {
            public int highScore;
        }

        /// <summary>
        /// 游戏开始时读取 JSON
        /// </summary>
        public void Load()
        {
            if (File.Exists(savePath))
            {
                string json = File.ReadAllText(savePath);
                SaveData data = JsonUtility.FromJson<SaveData>(json);
                highScore = data.highScore;
                Debug.Log($"读取最高分: {highScore}");
            }
            else
            {
                highScore = 0;
            }
            currentScore = 0; // 游戏开始时重置当前分数
        }

        /// <summary>
        /// 增加当前分数，并尝试更新最高分
        /// </summary>
        public void AddScore(int score)
        {
            currentScore += score;
            if (currentScore > highScore)
            {
                highScore = currentScore;
                Debug.Log($"新纪录！{playerName} 的最高分更新为 {highScore}");
            }
        }

        /// <summary>
        /// 游戏退出或保存时调用
        /// </summary>
        public void Save()
        {
            SaveData data = new SaveData() { highScore = highScore };
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(savePath, json);
            Debug.Log($"已保存最高分: {highScore}");
        }

        /// <summary>
        /// 获取当前分数
        /// </summary>
        public int GetCurrentScore() => currentScore;

        /// <summary>
        /// 获取最高分
        /// </summary>
        public int GetHighScore() => highScore;
    }
}
