using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miao {
    [CreateAssetMenu(fileName = "ScoreException1", menuName = "Game/Record Exception/Score -114514")]
    public class ScoreException1 : RecordExceptionBase
    {
        public override void OnCollect(GameObject collector, RecordData recordData)
        {
            // TODO: 设置分数为 -114514
            //分数异常，设置玩家分数为-114514
            GameManager.Instance.SetScoreText(-114514);
        }
    }
}
