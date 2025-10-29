using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace miao
{
    [CreateAssetMenu(fileName = "PlayerScaleException", menuName = "Game/Record Exception/Player Scale Change")]
    public class PlayerScaleException : RecordExceptionBase
    {
        public override void OnCollect(GameObject collector, RecordData recordData)
        {
            // TODO: 改变玩家大小
            Player.Instance.transform.localScale = new Vector3(5,2,2);
            StateController.Instance.ExecuteRepeated(0.5f,30.0f, () =>
            {
                bool a = true;
                if (a)
                {
                    Player.Instance.transform.localScale = new Vector3(5, 2, 1);
                    a = !a;
                }
                else
                {
                    Player.Instance.transform.localScale = new Vector3(1, 2, 5);
                    a = !a;
                }
                
            });
            StateController.Instance.ExecuteAfter(32.0f, () => 
            {
                Player.Instance.transform.localScale = new Vector3(1, 1, 1);
            });
        }
    }
}