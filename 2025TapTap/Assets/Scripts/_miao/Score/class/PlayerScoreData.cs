using UnityEngine;

namespace miao
{
    [CreateAssetMenu(fileName = "PlayerScoreData", menuName = "miao/Player Score Data")]
    public class PlayerScoreData : ScriptableObject
    {
        [Header("玩家信息")]
        public string playerName = "Player";

        [Header("最高分")]
        public int highScore = 0;

        /// <summary>
        /// 尝试更新最高分
        /// </summary>
        public void UpdateScore(int newScore)
        {
            if (newScore > highScore)
            {
                highScore = newScore;
                Debug.Log($"新纪录！{playerName} 的最高分更新为 {highScore}");
            }
        }
    }
}
