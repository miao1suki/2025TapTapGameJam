using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace miao
{
    public class PlayerMoving : MonoBehaviour
    {

        void Start()
        {

        }


        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                JiMiUnlockManager.Instance.OnPlayerEvent(Player.Instance);
                Debug.Log("¹þÆøÒ»´Î");
                Player.Instance.haQiCount += 1;
                JiMiUnlockManager.Instance.CheckUnlocks(Player.Instance);
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                Player.Instance.SetJiMiType(JiMiType.HaNiuMo);
            }
        }
    }
}
