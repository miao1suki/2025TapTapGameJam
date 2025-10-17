using UnityEngine;
using System.Collections.Generic;
namespace miao
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;
        private Dictionary<string, AudioClip> audioClips;

        void Awake()
        {
            Instance = this;
            if (audioClips == null)
            {
                audioClips = new Dictionary<string, AudioClip>();
            }

            // 加载所有音频文件到字典
            AudioClip[] clips = Resources.LoadAll<AudioClip>("Audio");
            foreach (var clip in clips)
            {
                audioClips[clip.name] = clip;
            }
        }

        void Update()
        {
            this.transform.position = Player.Instance.transform.position;
        }
        public static void Play(string audioName, AudioSource source, bool isSinglePlay) => Instance.PlayAudio(audioName, source, isSinglePlay);

        // 根据音频名称播放音效
        public void PlayAudio(string audioName, AudioSource source, bool isSinglePlay)
        {
            if (audioClips.TryGetValue(audioName, out AudioClip clip))
            {
                source.clip = clip;

                if (isSinglePlay)
                {
                    // 使用 PlayOneShot 播放一次音频
                    source.PlayOneShot(clip);
                }
                else
                {
                    // 使用 Play 播放并循环
                    source.loop = true;
                    source.Play();
                }
            }
            else
            {
                Debug.LogWarning("音频文件未找到：" + audioName);
            }
        }

        // 停止音频播放
        public void StopAudio(AudioSource source)
        {
            source.Stop(); 
            source.loop = false; 
        }
    }
}


