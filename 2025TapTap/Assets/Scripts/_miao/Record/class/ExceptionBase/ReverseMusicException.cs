using System.Collections;
using UnityEngine;
using AudioSystem; 

namespace miao
{
    [CreateAssetMenu(fileName = "ReverseMusicException", menuName = "Game/Record Exception/Reverse Music")]
    public class ReverseMusicException : RecordExceptionBase
    {
        public override void OnCollect(GameObject collector, RecordData recordData)
        {
            AudioSystem.AudioManager.Volume = 0f;
            miao.AudioManager.Instance.PlayAudio("¹þÑ©´óÃ±ÏÕµ¹·Å",Player.Instance.transform.position,false,0.7f);

            StateController.Instance.ExecuteAfter(45.0f, () => 
            {
                if(AudioSystem.AudioManager.Volume < 0.03f)
                {
                    AudioSystem.AudioManager.Volume = 0.3f;
                }
            });
        }

       
    }
}
