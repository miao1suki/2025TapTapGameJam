using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace miao
{
    [CreateAssetMenu(fileName = "FlattenPlayerException", menuName = "Game/Record Exception/Flatten Player")]
    public class FlattenPlayerException : RecordExceptionBase
    {
        public override void OnCollect(GameObject collector, RecordData recordData)
        {
            // TODO: ´ò±âÍæ¼Ò
            Player.Instance.transform.localScale = new Vector3(2, 0.01f, 2);

            StateController.Instance.ExecuteAfter(60.0f, () =>
            {
                Player.Instance.transform.localScale = new Vector3(1, 1, 1);
            });
        }
    }
}