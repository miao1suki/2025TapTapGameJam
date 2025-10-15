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

        [Header("��ѡ���֣�ȫ��α��ҳأ�")]
        public List<string> allPossibleNames = new List<string>();

        [Header("���а������ѡȡ������Χ")]
        public Vector2Int randomCountRange = new Vector2Int(5, 10);

        [Header("������Χ")]
        public Vector2Int scoreRange = new Vector2Int(500, 2000);

        [Header("��ǰ���а񣨰�����ң�")]
        public List<Entry> currentRank = new List<Entry>();

        /// <summary>
        /// ����������а�
        /// </summary>
        public void Generate(PlayerScoreData playerData)
        {
            currentRank.Clear();

            if (allPossibleNames.Count == 0)
            {
                Debug.LogWarning("ScoreboardData: ��ѡ�����б�Ϊ�գ�");
                return;
            }

            // ����������ּ���α���
            int count = Random.Range(randomCountRange.x, randomCountRange.y + 1);
            count = Mathf.Min(count, allPossibleNames.Count);

            // �����ѡ���ظ�����
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

            // �������
            if (playerData != null)
            {
                currentRank.Add(new Entry
                {
                    name = playerData.playerName,
                    score = playerData.highScore
                });
            }

            // ����
            currentRank.Sort((a, b) => b.score.CompareTo(a.score));
        }
    }
}
