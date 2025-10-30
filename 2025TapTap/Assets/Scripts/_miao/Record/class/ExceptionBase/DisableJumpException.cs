using System.Collections;
using UnityEngine;
using AudioSystem; // 引入AudioManager命名空间

namespace miao
{
    [CreateAssetMenu(fileName = "DisableJumpException", menuName = "Game/Record Exception/Disable Jump")]
    public class DisableJumpException : RecordExceptionBase
    {
        [Header("替换曲目设置")]
        [SerializeField] private string newMusicName = "无基区"; // 要播放的新音乐名（放在Resources/Audio中）
        [SerializeField] private float newMusicVolume = 0.3f;   // 新音乐音量
        [SerializeField] private float revertDelay = 141f;       // 播放多久后换回原曲

        private AudioClip originalClip;
        private float originalTime;
        private float originalVolume;

        public override void OnCollect(GameObject collector, RecordData recordData)
        {
            StateController.Instance.ExecuteAfter(0.1f, () => 
            {
                SwapBackgroundMusic();
            });
        }

        private void SwapBackgroundMusic()
        {
            AudioSource bg = AudioSystem.AudioManager.BgSource;

            // 保存原曲信息
            originalClip = bg.clip;
            originalTime = bg.time;
            originalVolume = AudioSystem.AudioManager.Volume;

            // 加载新曲目
            AudioClip newClip = Resources.Load<AudioClip>($"Audio/{newMusicName}");
            if (newClip == null)
            {
                Debug.LogWarning($"找不到音频：Audio/{newMusicName}");
                return;
            }

            // 切换播放
            bg.clip = newClip;
            bg.time = 0f;
            bg.loop = true;
            bg.Play();

            // 设置音量
            AudioSystem.AudioManager.Volume = newMusicVolume;

            // 在指定时间后换回原曲
            StateController.Instance.ExecuteAfter(revertDelay, () =>
            {
                if (originalClip != null)
                {
                    bg.clip = originalClip;
                    bg.time = originalTime;
                    bg.loop = true;
                    bg.Play();
                    AudioSystem.AudioManager.Volume = originalVolume;
                }
            });
        }
    }
}
