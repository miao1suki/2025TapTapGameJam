using System.Collections;
using UnityEngine;

namespace miao
{
    [CreateAssetMenu(fileName = "ForceWindowException", menuName = "Game/Record Exception/Force Window Mode")]
    public class ForceWindowException : RecordExceptionBase
    {
        public override void OnCollect(GameObject collector, RecordData recordData)
        {
            // 记录当前分辨率与全屏模式
            int originalWidth = Screen.width;
            int originalHeight = Screen.height;
            FullScreenMode originalMode = Screen.fullScreenMode;

            // 先修改分辨率，再切换为窗口模式
            Screen.SetResolution(320, 180, false);
            Screen.fullScreenMode = FullScreenMode.Windowed;

            //Debug.Log("[ForceWindowException] 切换为窗口模式 (320×180)");

            // 一段时间后恢复
            StateController.Instance.ExecuteAfter(60, () =>
            {
                Screen.SetResolution(originalWidth, originalHeight, true);
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;

                //Debug.Log("[ForceWindowException] 恢复为全屏窗口模式");
            });
        }
    }
}
