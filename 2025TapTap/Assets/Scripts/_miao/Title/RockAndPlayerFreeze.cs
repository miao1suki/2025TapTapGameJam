using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miao
{
    public class RockSpawner : MonoBehaviour
    {
        [Header("��ʯ����")]
        public GameObject[] rockPrefabs;        // 6����ʯԤ����
        [Header("���ɵ�")]
        public GameObject spawnPoint;           // ��ʯ�������ĵ�
        [Header("���ɷ�Χ")]
        public Vector3 spawnRange = new Vector3(3f, 0f, 3f); // XZ��ƫ�Ʒ�Χ
        [Header("������ŷ�Χ")]
        public Vector2 scaleRange = new Vector2(0.8f, 1.5f);
        [Header("ÿ��������ʯ������Χ")]
        public Vector2 quantityRange = new Vector2(1, 5);
        [Header("���ɼ��ʱ��")]
        public Vector2 spawnIntervalRange = new Vector2(2f, 5f);
        [Header("��ʯ����ʱ��")]
        public float lifeTime = 5f;

        private List<GameObject> spawnedRocks = new List<GameObject>();

        private void Start()
        {
            if (spawnPoint == null)
                Debug.LogWarning("RockSpawner: spawnPoint δ���ã����ɽ�ʹ������λ����Ϊ����");

            StartCoroutine(SpawnLoop());
        }

        private void Update()
        {
            Player.Instance.transform.position = new Vector3(0, 2.4f, 0);
        }
        private IEnumerator SpawnLoop()
        {
            while (true)
            {
                float waitTime = Random.Range(spawnIntervalRange.x, spawnIntervalRange.y);
                yield return new WaitForSeconds(waitTime);

                SpawnRocks();
            }
        }

        private void SpawnRocks()
        {
            if (rockPrefabs.Length == 0 || spawnPoint == null) return;

            int numRocks = Random.Range((int)quantityRange.x, (int)quantityRange.y + 1);

            for (int i = 0; i < numRocks; i++)
            {
                // ���ѡ����ʯ����
                GameObject prefab = rockPrefabs[Random.Range(0, rockPrefabs.Length)];
                Vector3 basePos = spawnPoint.transform.position;

                // ���ƫ��λ��
                Vector3 offset = new Vector3(
                    Random.Range(-spawnRange.x, spawnRange.x),
                    Random.Range(-spawnRange.y, spawnRange.y),
                    Random.Range(-spawnRange.z, spawnRange.z)
                );

                Vector3 spawnPos = basePos + offset;

                // ʵ������ʯ
                GameObject rock = Instantiate(prefab, spawnPos, Random.rotation);

                // �������
                float scale = Random.Range(scaleRange.x, scaleRange.y);
                rock.transform.localScale = Vector3.one * scale;

                // ��ȡ PhysicsBody ��ע��
                PhysicsBody body = rock.GetComponent<PhysicsBody>();
                if (body != null)
                {
                    PhysicsSystem.Instance.RegisterBody(body);
                }

                // ��ӵ��б����㶨ʱ���
                spawnedRocks.Add(rock);

                // �Զ�����
                StartCoroutine(DestroyRockAfterTime(rock, lifeTime));
            }

            spawnedRocks.RemoveAll(r => r == null);
        }

        private IEnumerator DestroyRockAfterTime(GameObject rock, float time)
        {
            yield return new WaitForSeconds(time);

            if (rock != null)
            {
                PhysicsBody body = rock.GetComponent<PhysicsBody>();
                if (body != null)
                {
                    PhysicsSystem.Instance.UnregisterBody(body);
                }

                spawnedRocks.Remove(rock);
                Destroy(rock);
            }
        }

        // -------------------- ���ӻ����ɷ�Χ --------------------
        private void OnDrawGizmosSelected()
        {
            if (spawnPoint == null) return;

            Gizmos.color = Color.yellow;

            Vector3 center = spawnPoint.transform.position;
            Vector3 size = new Vector3(spawnRange.x * 2, spawnRange.y * 2, spawnRange.z * 2);
            Gizmos.DrawWireCube(center, size);
        }
    }
}
