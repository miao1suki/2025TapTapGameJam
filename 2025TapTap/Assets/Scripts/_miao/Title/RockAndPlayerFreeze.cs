using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miao
{
    public class RockSpawner : MonoBehaviour
    {
        [Header("岩石设置")]
        public GameObject[] rockPrefabs;        // 6种岩石预制体
        [Header("生成点")]
        public GameObject spawnPoint;           // 岩石生成中心点
        [Header("生成范围")]
        public Vector3 spawnRange = new Vector3(3f, 0f, 3f); // XZ轴偏移范围
        [Header("随机缩放范围")]
        public Vector2 scaleRange = new Vector2(0.8f, 1.5f);
        [Header("每次生成岩石数量范围")]
        public Vector2 quantityRange = new Vector2(1, 5);
        [Header("生成间隔时间")]
        public Vector2 spawnIntervalRange = new Vector2(2f, 5f);
        [Header("岩石存在时间")]
        public float lifeTime = 5f;

        private List<GameObject> spawnedRocks = new List<GameObject>();

        private void Start()
        {
            if (spawnPoint == null)
                Debug.LogWarning("RockSpawner: spawnPoint 未设置，生成将使用自身位置作为中心");

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
                // 随机选择岩石类型
                GameObject prefab = rockPrefabs[Random.Range(0, rockPrefabs.Length)];
                Vector3 basePos = spawnPoint.transform.position;

                // 随机偏移位置
                Vector3 offset = new Vector3(
                    Random.Range(-spawnRange.x, spawnRange.x),
                    Random.Range(-spawnRange.y, spawnRange.y),
                    Random.Range(-spawnRange.z, spawnRange.z)
                );

                Vector3 spawnPos = basePos + offset;

                // 实例化岩石
                GameObject rock = Instantiate(prefab, spawnPos, Random.rotation);

                // 随机缩放
                float scale = Random.Range(scaleRange.x, scaleRange.y);
                rock.transform.localScale = Vector3.one * scale;

                // 获取 PhysicsBody 并注册
                PhysicsBody body = rock.GetComponent<PhysicsBody>();
                if (body != null)
                {
                    PhysicsSystem.Instance.RegisterBody(body);
                }

                // 添加到列表，方便定时清除
                spawnedRocks.Add(rock);

                // 自动销毁
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

        // -------------------- 可视化生成范围 --------------------
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
