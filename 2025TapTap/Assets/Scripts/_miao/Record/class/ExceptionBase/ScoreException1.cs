using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miao {
    [CreateAssetMenu(fileName = "ScoreException1", menuName = "Game/Record Exception/Score -114514")]
    public class ScoreException1 : RecordExceptionBase
    {
        public override void OnCollect(GameObject collector, RecordData recordData)
        {
            // TODO: ���÷���Ϊ -114514
            //�����쳣��������ҷ���Ϊ-114514
            GameManager.Instance.SetScoreText(-114514);

            GameManager.Instance.SetTextCoolor(new Color(255,0,0));

            StateController.Instance.ExecuteAfter(10.0f, () =>
            {
                GameManager.Instance.SetTextCoolor(new Color(255, 255, 255));
            });

        }
    }
}
