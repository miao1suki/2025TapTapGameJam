using System.Collections.Generic;
using UnityEngine;

namespace miao
{
    [CreateAssetMenu(fileName = "ScoreboardData", menuName = "miao/Scoreboard Data")]
    public class ScoreboardData : ScriptableObject
    {
        [System.Serializable]
        public class Entry
        {
            public string name;
            public int score;
        }

        [Header("候选名字（全部伪玩家池）")]
        public List<string> allPossibleNames = new List<string>();

        [Header("排行榜中随机选取数量范围")]
        public Vector2Int randomCountRange = new Vector2Int(5, 10);

        [Header("分数范围")]
        public Vector2Int scoreRange = new Vector2Int(500, 2000);

        [Header("当前排行榜（包含玩家）")]
        public List<Entry> currentRank = new List<Entry>();

        /// <summary>
        /// 随机生成排行榜
        /// </summary>
        public void Generate(PlayerScoreData playerData)
        {
            currentRank.Clear();

            if (allPossibleNames.Count == 0)
            {
                Debug.LogWarning("ScoreboardData: 候选名字列表为空！");
                return;
            }

            // 随机决定出现几个伪玩家
            int count = Random.Range(randomCountRange.x, randomCountRange.y + 1);
            count = Mathf.Min(count, allPossibleNames.Count);

            // 随机挑选不重复名字
            List<string> tempNames = new List<string>(allPossibleNames);
            for (int i = 0; i < count; i++)
            {
                int index = Random.Range(0, tempNames.Count);
                string name = tempNames[index];
                tempNames.RemoveAt(index);

                currentRank.Add(new Entry
                {
                    name = name,
                    score = Random.Range(scoreRange.x, scoreRange.y)
                });
            }

            // 加入玩家
            if (playerData != null)
            {
                currentRank.Add(new Entry
                {
                    name = playerData.playerName,
                    score = playerData.highScore
                });
            }

            // 排序
            currentRank.Sort((a, b) => b.score.CompareTo(a.score));
        }
    }
}
