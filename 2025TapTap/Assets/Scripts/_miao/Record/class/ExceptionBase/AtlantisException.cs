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
            StateController.Instance.camWater = GameObject.FindGameObjectWithTag("CamWater");

            if(StateController.Instance.camWater)
            {
                StateController.Instance.camWater.SetActive(true);
            }

            StateController.Instance.ExecuteAfterCoroutine(60.0f, () => 
            {
                if (StateController.Instance.camWater)
                {
                    StateController.Instance.camWater.SetActive(false);
                }
            });
        }
    }
}