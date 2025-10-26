using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace miao
{
    [CreateAssetMenu(fileName = "PlanarizePlayerException", menuName = "Game/Record Exception/Planarize Player")]
    public class PlanarizePlayerException : RecordExceptionBase
    {
        public override void OnCollect(GameObject collector, RecordData recordData)
        {
            // TODO: 让玩家模型变成平面
        }
    }
}
