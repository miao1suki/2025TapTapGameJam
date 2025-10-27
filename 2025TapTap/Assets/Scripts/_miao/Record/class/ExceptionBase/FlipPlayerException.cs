using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miao
{
    [CreateAssetMenu(fileName = "FlipPlayerException", menuName = "Game/Record Exception/Flip Player Upside Down")]
    public class FlipPlayerException : RecordExceptionBase
    {
        public override void OnCollect(GameObject collector, RecordData recordData)
        {
            Player.Instance.transform.rotation = Quaternion.Euler(-180,0,0);
            // TODO: 上下颠倒玩家模型
            StateController.Instance.ExecuteRepeated(0.5f, 60.0f, () => 
            {
                Player.Instance.transform.rotation = Quaternion.Euler(-180, 0, 0);
            });
        }
    }
}

