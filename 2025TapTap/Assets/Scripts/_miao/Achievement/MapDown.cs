using miao;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDown : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Checker.Instance.Done(Checker.Instance.achievement1);

            // 找到最近的重生点
            Transform nearestRespawn = FindNearestRespawnPoint();

            if (nearestRespawn != null)
            {
                // 传送玩家
                Player.Instance.transform.position =  new Vector3(nearestRespawn.position.x, nearestRespawn.position.y + 2, nearestRespawn.position.z);
                Player.Instance.transform.rotation = nearestRespawn.rotation; //同步朝向
            }
            else
            {
                Debug.LogWarning("没有找到任何重生点！");
            }
        }

        

    }
    private Transform FindNearestRespawnPoint()
    {
        // 获取所有带 Tag 的重生点
        GameObject[] respawnPoints = GameObject.FindGameObjectsWithTag("RespawnPoint");
        if (respawnPoints.Length == 0) return null;

        Transform nearest = respawnPoints[0].transform;
        float nearestDist = Vector3.Distance(Player.Instance.transform.position, nearest.position);

        // 遍历找到最近的
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
}
