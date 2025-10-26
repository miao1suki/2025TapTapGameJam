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
            // TODO: 上下颠倒玩家模型
        }
    }
}

