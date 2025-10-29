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

            // �ҵ������������
            Transform nearestRespawn = FindNearestRespawnPoint();

            if (nearestRespawn != null)
            {
                // �������
                Player.Instance.transform.position =  new Vector3(nearestRespawn.position.x, nearestRespawn.position.y + 2, nearestRespawn.position.z);
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
}
