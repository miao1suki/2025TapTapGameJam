using System.Collections.Generic;
using UnityEngine;

namespace miao
{
    public class ScoreManager : MonoBehaviour
    {
        [Header("引用")]
        [SerializeField] private PlayerScoreData playerData;
        [SerializeField] private ScoreboardData scoreboardData;

        public static ScoreManager Instance;

        // 最终排行榜列表，外部只读
        public List<ScoreboardData.Entry> rankList { get; private set; } = new List<ScoreboardData.Entry>();

        private void Awake()
        {
            Instance = this;
            playerData.Load(); // 从 JSON 读取玩家历史最高分
            GenerateRankList(); // 生成排行榜
        }

        /// <summary>
        /// 增加分数
        /// </summary>
        public void AddScore(int score)
        {
            if (playerData == null) return;

            playerData.AddScore(score);

            // 如果当前分数超过最高分，则保存
            if (playerData.GetCurrentScore() > playerData.GetHighScore())
            {
                playerData.Save();
            }

            // 刷新排行榜
            GenerateRankList();
        }

        /// <summary>
        /// 获取当前分数
        /// </summary>
        public int GetCurrentScore() => playerData != null ? playerData.GetCurrentScore() : 0;

        /// <summary>
        /// 获取最高分
        /// </summary>
        public int GetHighScore() => playerData != null ? playerData.GetHighScore() : 0;

        /// <summary>
        /// 游戏退出时保存分数
        /// </summary>
        public void SaveScore()
        {
            playerData?.Save();
        }

        /// <summary>
        /// 生成排行榜列表（包含玩家历史最高分）并排序
        /// </summary>
        private void GenerateRankList()
        {
            rankList.Clear();

            if (scoreboardData == null || playerData == null) return;

            // 生成假玩家并加入玩家历史最高分
            scoreboardData.Generate(playerData);

            // 把生成的排行榜放到 rankList
            rankList.AddRange(scoreboardData.currentRank);
        }
    }
}
