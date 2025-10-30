using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miao
{
    [CreateAssetMenu(fileName = "AtlantisException", menuName = "Game/Record Exception/Atlantis")]
    public class AtlantisException : RecordExceptionBase
    {
        public override void OnCollect(GameObject collector, RecordData recordData)
        {
            // TODO: 亚特兰蒂斯水圈
            StateController.Instance.camWater = Camera.main.gameObject.transform.GetChild(0).GetChild(0).gameObject;

            if(StateController.Instance.camWater)
            {
                Debug.Log("触发");
                StateController.Instance.camWater.SetActive(true);
            }

            StateController.Instance.ExecuteAfter(60.0f, () => 
            {
                if (StateController.Instance.camWater)
                {
                    Debug.Log("清除");
                    StateController.Instance.camWater.SetActive(false);
                }
            });
        }
    }
}