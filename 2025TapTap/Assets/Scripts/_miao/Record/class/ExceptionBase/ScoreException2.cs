using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miao
{
    [CreateAssetMenu(fileName = "ScoreException2", menuName = "Game/Record Exception/Score 9999999")]
    public class ScoreException2 : RecordExceptionBase
    {
        public override void OnCollect(GameObject collector, RecordData recordData)
        {
            // TODO: ���÷���Ϊ 9999999
            //�����쳣��������ҷ���Ϊ9999999
            GameManager.Instance.SetScoreText(9999999);

            GameManager.Instance.SetTextCoolor(new Color(255, 0, 0));

            StateController.Instance.ExecuteAfter(10.0f, () =>
            {
                GameManager.Instance.SetTextCoolor(new Color(255, 255, 255));
            });
        }
    }
}