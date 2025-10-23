using System.Collections.Generic;
using UnityEngine;

namespace miao
{
    public class ScoreManager : MonoBehaviour
    {
        [Header("����")]
        [SerializeField] private PlayerScoreData playerData;
        [SerializeField] private ScoreboardData scoreboardData;

        public static ScoreManager Instance;

        // �������а��б��ⲿֻ��
        public List<ScoreboardData.Entry> rankList { get; private set; } = new List<ScoreboardData.Entry>();

        private void Awake()
        {
            Instance = this;
            playerData.Load(); // �� JSON ��ȡ�����ʷ��߷�
            GenerateRankList(); // �������а�
        }

        /// <summary>
        /// ���ӷ���
        /// </summary>
        public void AddScore(int score)
        {
            if (playerData == null) return;

            playerData.AddScore(score);

            // �����ǰ����������߷֣��򱣴�
            if (playerData.GetCurrentScore() > playerData.GetHighScore())
            {
                playerData.Save();
            }

            // ˢ�����а�
            GenerateRankList();
        }

        /// <summary>
        /// ��ȡ��ǰ����
        /// </summary>
        public int GetCurrentScore() => playerData != null ? playerData.GetCurrentScore() : 0;

        /// <summary>
        /// ��ȡ��߷�
        /// </summary>
        public int GetHighScore() => playerData != null ? playerData.GetHighScore() : 0;

        /// <summary>
        /// ��Ϸ�˳�ʱ�������
        /// </summary>
        public void SaveScore()
        {
            playerData?.Save();
        }

        /// <summary>
        /// �������а��б����������ʷ��߷֣�������
        /// </summary>
        private void GenerateRankList()
        {
            rankList.Clear();

            if (scoreboardData == null || playerData == null) return;

            // ���ɼ���Ҳ����������ʷ��߷�
            scoreboardData.Generate(playerData);

            // �����ɵ����а�ŵ� rankList
            rankList.AddRange(scoreboardData.currentRank);
        }
    }
}
