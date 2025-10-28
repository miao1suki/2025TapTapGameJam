using miao;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Checker.Instance.Done(Checker.Instance.achievement5);
            this.enabled = false;
        }
    }
}
