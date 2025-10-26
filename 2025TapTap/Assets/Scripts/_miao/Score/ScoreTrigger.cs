using System.Collections.Generic;
using UnityEngine;
using UI;
using TMPro; // ���� ScorePanel �������ռ�

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
                // �� Trigger �������
                transform.position = Player.Instance.transform.position;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // �ų���ң�ֻ���� PhysicsBody ������
            if (other != null && other.gameObject.GetComponent<PhysicsBody>() && other.gameObject != Player.Instance.gameObject)
            {

                // ��ӵ��б�
                objectsInTrigger.Add(other.gameObject);

                // ���ӱ���
                if (scorePanel != null)
                {
                    AddMultiplier();

                    // �����б��������ӷ�����ÿ�������ֵ���Թ̶�
                    int scorePerObject = 50;
                    int totalScore = objectsInTrigger.Count * scorePerObject;
                    scorePanel.IncreaseScore(totalScore);
                    AddScore("��ײ����",totalScore);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // �뿪 Trigger ��������б��Ƴ�
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
