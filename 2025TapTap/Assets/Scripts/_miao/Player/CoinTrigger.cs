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
       
        // 检查是否碰撞到了 Coin 类型的物体且金币未被拾取
        Coin coin = other.GetComponent<Coin>();
        if (coin != null && !coin.get)
        {

            // 显示金币
            coin.enabled = true;  // 启用金币脚本
        }
    }


    private void OnTriggerExit(Collider other)
    {
        // 离开触发器时禁用金币显示
        Coin coin = other.GetComponent<Coin>();
        if (coin != null && !coin.get)
        {
            coin.enabled = false;  // 禁用金币脚本
        }
    }
}
