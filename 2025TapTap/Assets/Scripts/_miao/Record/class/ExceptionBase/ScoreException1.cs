using UnityEngine;
namespace miao
{
    [CreateAssetMenu(fileName = "ScoreException1", menuName = "Game/Record Exception/ScoreException1")]
    public class ScoreException1 : RecordExceptionBase
    {
        public override void OnCollect(GameObject collector, RecordData recordData)
        {
            //�����쳣��������ҷ���Ϊ-114514
            GameManager.Instance.SetScoreText(-114514);
        }
    }
}



