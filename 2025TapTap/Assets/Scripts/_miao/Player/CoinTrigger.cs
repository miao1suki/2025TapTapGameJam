using miao;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinTrigger : MonoBehaviour
{

    private void Start()
    {
        this.transform.position = Player.Instance.transform.position;
    }

    private void Update()
    {
        this.transform.position = Player.Instance.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
       
        // ����Ƿ���ײ���� Coin ���͵������ҽ��δ��ʰȡ
        Coin coin = other.GetComponent<Coin>();
        if (coin != null && !coin.get)
        {

            // ��ʾ���
            coin.enabled = true;  // ���ý�ҽű�
        }
    }


    private void OnTriggerExit(Collider other)
    {
        // �뿪������ʱ���ý����ʾ
        Coin coin = other.GetComponent<Coin>();
        if (coin != null && !coin.get)
        {
            coin.enabled = false;  // ���ý�ҽű�
        }
    }
}
