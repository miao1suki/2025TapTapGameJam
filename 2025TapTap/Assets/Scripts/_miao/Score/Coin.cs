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
        startPos = transform.position; // Awake ʱ��¼λ�ã���֤�� Instantiate ��
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
            ScoreTrigger.Instance.AddScore("��ý�ң�",100);

            AudioManager.Instance.PlayAudio("��Ҷ�",transform.position,false,0.8f);

            this.gameObject.SetActive(false);       

        }
    }
}
