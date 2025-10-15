using UnityEngine;

namespace miao
{
    public class ScoreManager : MonoBehaviour
    {
        [Header("����")]
        [SerializeField] private ScoreboardData scoreboardData;
        [SerializeField] private PlayerScoreData playerData;
        public static ScoreManager Instance;
        private void Awake()
        {
            Instance = this;
        }


        void Start()
        {
            RefreshRank();
        }

        public void AddScore(int score)
        {
            playerData.UpdateScore(score);
            RefreshRank();
        }
        public void RefreshRank()
        {
            if (scoreboardData != null && playerData != null)
            {
                scoreboardData.Generate(playerData);
                PrintRank();
            }
        }

        void PrintRank()
        {
            Debug.Log("��ǰ���а�");
            int i = 1;
            foreach (var e in scoreboardData.currentRank)
            {
                Debug.Log($"{i}. {e.name} : {e.score}");
                i++;
            }
        }
    }
}
