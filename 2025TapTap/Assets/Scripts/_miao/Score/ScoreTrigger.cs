using System.Collections.Generic;
using UnityEngine;
using UI;
using TMPro; // 引用 ScorePanel 的命名空间

namespace miao
{
    public class ScoreTrigger : MonoBehaviour
    {
        [SerializeField] private ScorePanel scorePanel;
        [SerializeField] private TextMeshProUGUI text;
        private List<GameObject> objectsInTrigger = new List<GameObject>();

        public static ScoreTrigger Instance;

        private void Awake()
        {
            Instance = this;
        }
        private void Update()
        {
            if (Player.Instance != null)
            {
                // 让 Trigger 跟随玩家
                transform.position = Player.Instance.transform.position;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // 排除玩家，只处理 PhysicsBody 的物体
            if (other != null && other.gameObject.GetComponent<PhysicsBody>() && other.gameObject != Player.Instance.gameObject)
            {

                // 添加到列表
                objectsInTrigger.Add(other.gameObject);

                // 增加倍率
                if (scorePanel != null)
                {
                    AddMultiplier();

                    // 根据列表数量增加分数，每个物体分值可以固定
                    int scorePerObject = 50;
                    int totalScore = objectsInTrigger.Count * scorePerObject;
                    scorePanel.IncreaseScore(totalScore);
                    AddScore("碰撞物体",totalScore);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // 离开 Trigger 的物体从列表移除
            if (objectsInTrigger.Contains(other.gameObject))
            {
                objectsInTrigger.Remove(other.gameObject);
            }
        }

        public void AddScore(string name,int score)
        {
            text.SetText(name);
            scorePanel.IncreaseScore(score);
        }

        public void AddMultiplier()
        {
            scorePanel.IncreaseMultiplier();
        }
    }
}
