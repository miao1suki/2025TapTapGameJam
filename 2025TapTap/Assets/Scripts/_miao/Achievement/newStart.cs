using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newStart : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Checker.Instance.Done(Checker.Instance.achievement6);
            this.gameObject.SetActive(false);
        }
    }
}
