using System.Collections.Generic;
using UnityEngine;
using UI; // ���� ScorePanel �������ռ�

namespace miao
{
    public class ScoreTrigger : MonoBehaviour
    {
        [SerializeField] private ScorePanel scorePanel; // ��Ҫ��Inspector����ScorePanel
        private List<GameObject> objectsInTrigger = new List<GameObject>();

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
                    scorePanel.IncreaseMultiplier();

                    // �����б��������ӷ�����ÿ�������ֵ���Թ̶�
                    int scorePerObject = 100;
                    int totalScore = objectsInTrigger.Count * scorePerObject;
                    scorePanel.IncreaseScore(totalScore);
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
    }
}
