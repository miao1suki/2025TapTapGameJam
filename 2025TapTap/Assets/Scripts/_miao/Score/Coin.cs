using miao;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public float rotateSpeed = 60f;
    public float floatAmplitude = 0.25f;
    public float floatSpeed = 2f;

    private Vector3 startPos;
    private void Awake()
    {
        startPos = transform.position; // Awake 时记录位置，保证在 Instantiate 后
    }
    private void Update()
    {
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
        transform.position = startPos + Vector3.up * Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            ScoreTrigger.Instance.AddScore("获得金币！",100);

            AudioManager.Instance.PlayAudio("金币短",transform.position,false,0.8f);

            this.gameObject.SetActive(false);       

        }
    }
}
