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
            // TODO: �ı����ش�С��Shader������
        }
    }
}