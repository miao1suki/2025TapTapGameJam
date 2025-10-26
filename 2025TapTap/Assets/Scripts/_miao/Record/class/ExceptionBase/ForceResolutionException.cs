using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miao
{
    [CreateAssetMenu(fileName = "ForceResolutionException", menuName = "Game/Record Exception/Force Resolution")]
    public class ForceResolutionException : RecordExceptionBase
    {
        public override void OnCollect(GameObject collector, RecordData recordData)
        {
            // TODO: 修改游戏分辨率
        }
    }
}
