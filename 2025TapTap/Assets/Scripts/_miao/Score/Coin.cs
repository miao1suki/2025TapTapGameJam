using miao;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            ScoreTrigger.Instance.AddScore("��ý�ң�",100);
            ScoreTrigger.Instance.AddMultiplier();
        }
    }
}
