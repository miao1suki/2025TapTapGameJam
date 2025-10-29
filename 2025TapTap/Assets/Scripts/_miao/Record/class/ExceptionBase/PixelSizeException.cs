using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace miao
{
    [CreateAssetMenu(fileName = "PixelSizeException", menuName = "Game/Record Exception/Pixel Size Change")]
    public class PixelSizeException : RecordExceptionBase
    {
        public override void OnCollect(GameObject collector, RecordData recordData)
        {
            // TODO: 改变像素大小（Shader参数）
            GameManager.Instance.autoPixelControl = false;
            GameManager.Instance._renderseter.RT_Pixel = 1;
            GameManager.Instance._renderseter.ApplySettings();

            StateController.Instance.ExecuteAfter(60, () => 
            {
                GameManager.Instance.autoPixelControl = true;
                GameManager.Instance._renderseter.RT_Size = 7.0f;
                GameManager.Instance._renderseter.RT_Pixel = 7;
                GameManager.Instance.setCamSize();
                GameManager.Instance._renderseter.ApplySettings();
            });
        }
    }
}