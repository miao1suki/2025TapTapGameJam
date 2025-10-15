using UnityEngine;

namespace miao
{
    [CreateAssetMenu(fileName = "PlayerScoreData", menuName = "miao/Player Score Data")]
    public class PlayerScoreData : ScriptableObject
    {
        [Header("�����Ϣ")]
        public string playerName = "Player";

        [Header("��߷�")]
        public int highScore = 0;

        /// <summary>
        /// ���Ը�����߷�
        /// </summary>
        public void UpdateScore(int newScore)
        {
            if (newScore > highScore)
            {
                highScore = newScore;
                Debug.Log($"�¼�¼��{playerName} ����߷ָ���Ϊ {highScore}");
            }
        }
    }
}
