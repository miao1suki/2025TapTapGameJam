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
        }
    }
}