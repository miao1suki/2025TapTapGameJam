using System.Collections;
using UnityEngine;

namespace miao
{
    [CreateAssetMenu(fileName = "ForceWindowException", menuName = "Game/Record Exception/Force Window Mode")]
    public class ForceWindowException : RecordExceptionBase
    {
        public override void OnCollect(GameObject collector, RecordData recordData)
        {
            // ��¼��ǰ�ֱ�����ȫ��ģʽ
            int originalWidth = Screen.width;
            int originalHeight = Screen.height;
            FullScreenMode originalMode = Screen.fullScreenMode;

            // ���޸ķֱ��ʣ����л�Ϊ����ģʽ
            Screen.SetResolution(320, 180, false);
            Screen.fullScreenMode = FullScreenMode.Windowed;

            //Debug.Log("[ForceWindowException] �л�Ϊ����ģʽ (320��180)");

            // һ��ʱ���ָ�
            StateController.Instance.ExecuteAfter(60, () =>
            {
                Screen.SetResolution(originalWidth, originalHeight, true);
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;

                //Debug.Log("[ForceWindowException] �ָ�Ϊȫ������ģʽ");
            });
        }
    }
}
