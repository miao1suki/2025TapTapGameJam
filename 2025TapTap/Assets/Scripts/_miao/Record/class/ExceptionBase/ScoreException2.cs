using UnityEngine;
namespace miao
{
    [CreateAssetMenu(fileName = "ScoreException2", menuName = "Game/Record Exception/ScoreException2")]
    public class ScoreException2 : RecordExceptionBase
    {
        public override void OnCollect(GameObject collector, RecordData recordData)
        {
            //�����쳣��������ҷ���Ϊ9999999
            GameManager.Instance.SetScoreText(9999999);
        }
    }
}



