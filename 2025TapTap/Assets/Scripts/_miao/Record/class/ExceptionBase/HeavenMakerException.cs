using miao.day_and_night;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miao
{
    [CreateAssetMenu(fileName = "HeavenMakerException", menuName = "Game/Record Exception/Heaven Maker")]
    public class HeavenMakerException : RecordExceptionBase
    {
        public override void OnCollect(GameObject collector, RecordData recordData)
        {
            // TODO: ÌìÌÃÖÆÔì
            DayAndNight.Instance.dayDurationInSeconds = 3.0f;
            StateController.Instance.ExecuteAfter(60.0f,()=> 
            {
                DayAndNight.Instance.dayDurationInSeconds = 600.0f;
            });
        }
    }
}
