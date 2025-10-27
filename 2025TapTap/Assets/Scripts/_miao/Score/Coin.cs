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
            ScoreTrigger.Instance.AddScore("»ñµÃ½ð±Ò£¡",100);
            ScoreTrigger.Instance.AddMultiplier();
        }
    }
}
