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
            Player.Instance.transform.localScale = new Vector3(0.08f, 1, 1);

            StateController.Instance.ExecuteAfterCoroutine(60.0f, () =>
            {
                Player.Instance.transform.localScale = new Vector3(1, 1, 1);
            });
        }
    }
}
