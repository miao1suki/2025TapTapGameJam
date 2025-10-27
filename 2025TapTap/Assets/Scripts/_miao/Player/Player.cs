using AchievementSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
namespace miao
{
    public class Player : MonoBehaviour
    {
        [HideInInspector]
        public Rigidbody rb;
        public static Player Instance;//����


        [Header("��������")]
        public int _PlayerHealth = 100;//��ҵ�ǰѪ��
        [SerializeField]
        private int _PlayerMaxHealth = 100;//������Ѫ��


        [Header("��ǰ����������")]
        public JiMiType currentJiMiType = JiMiType.Normal;

        [Header("��ǰ����������")]
        public JiMiAbility currentAbility;

        [Header("���ֹ�������")]
        public int haQiCount = 0;



        private void Awake()
        {
            Instance = this;
            haQiCount = 0;//���ù�������
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogError(gameObject.name + " û�л�ȡ�� Rigidbody");
            }
        }

        public void ResetPlayerHealth()
        {
            _PlayerHealth = _PlayerMaxHealth;
        }

        public void ChangePlayerHealth(int health)
        {
            _PlayerHealth += health;
        }

        public void SetPlayerHealth(int health)
        {
            _PlayerHealth += health;
        }

        private void OnCollisionEnter(Collision collision)
        {

            if (collision.gameObject.CompareTag("Water"))
            {
                // �ӷ�
                ScoreTrigger.Instance.AddScore("����ˮ��", 500);
                if (!AchievementManager.Ins.IsCompleted(Checker.Instance.achievement4))
                {
                    Checker.Instance.Done(Checker.Instance.achievement4);
                }

                // �ҵ������������
                Transform nearestRespawn = FindNearestRespawnPoint();

                if (nearestRespawn != null)
                {
                    // �������
                    Player.Instance.transform.position = nearestRespawn.position;
                    Player.Instance.transform.rotation = nearestRespawn.rotation; //ͬ������
                }
                else
                {
                    Debug.LogWarning("û���ҵ��κ������㣡");
                }
            }
        }

        private Transform FindNearestRespawnPoint()
        {
            // ��ȡ���д� Tag ��������
            GameObject[] respawnPoints = GameObject.FindGameObjectsWithTag("RespawnPoint");
            if (respawnPoints.Length == 0) return null;

            Transform nearest = respawnPoints[0].transform;
            float nearestDist = Vector3.Distance(Player.Instance.transform.position, nearest.position);

            // �����ҵ������
            foreach (GameObject point in respawnPoints)
            {
                float dist = Vector3.Distance(Player.Instance.transform.position, point.transform.position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = point.transform;
                }
            }

            return nearest;
        }

        public void SetJiMiType(JiMiType newType)
        {
            // �ж��Ƿ����
            string typeName = newType.ToString(); // ʹ��ö������Ϊ JiMiDataSO ����
            if (!JiMiUnlockManager.Instance.IsUnlocked(typeName))
            {
                Debug.LogWarning($"������ {typeName} δ�������޷��л���");
                return; // δ������ֱ�ӷ��أ����л�
            }
            // ���������
            if (currentAbility != null)
            {
                currentAbility.OnDeactivate(this);
                Destroy(currentAbility);
            }


            currentJiMiType = newType;

            // �����������������
            switch (newType)
            {
                case JiMiType.Normal:
                    currentAbility = gameObject.AddComponent<Normal_JiMiAbility>();
                    break;
                case JiMiType.ZhiShengJi:
                    currentAbility = gameObject.AddComponent<ZhiShengJi_JiMiAbility>();
                    break;
                case JiMiType.DingDongJi:
                    currentAbility = gameObject.AddComponent<DingDongJi_JiMiAbility>();
                    break;
                case JiMiType.HaNiuMo:
                    currentAbility = gameObject.AddComponent<HaNiuMo_JiMiAbility>();
                    break;
                default:
                    currentAbility = null;
                    break;
            }

            // ����������
            if (currentAbility != null)
                currentAbility.OnActivate(this);

            Debug.Log("��ǰ�������л�Ϊ��" + currentJiMiType);
        }
    }
}
