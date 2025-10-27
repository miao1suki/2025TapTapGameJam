using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miao {
    [CreateAssetMenu(fileName = "DisableJumpException", menuName = "Game/Record Exception/Disable Jump")]
    public class DisableJumpException : RecordExceptionBase
    {
        public override void OnCollect(GameObject collector, RecordData recordData)
        {
            // TODO: 修改背景音乐为无人区
            
        }
    }
}