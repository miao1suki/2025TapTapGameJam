using UnityEngine;
using System.Collections.Generic;

namespace miao
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;

        private Dictionary<string, AudioClip> audioClips;

        // 对象池
        private Queue<AudioSource> audioSourcePool = new Queue<AudioSource>();
        private int poolSize = 10; // 默认池大小

        private void Awake()
        {
            Instance = this;

            // 加载音频
            audioClips = new Dictionary<string, AudioClip>();
            AudioClip[] clips = Resources.LoadAll<AudioClip>("Ignore/Audio");
            foreach (var clip in clips)
                audioClips[clip.name] = clip;

            // 预创建对象池
            for (int i = 0; i < poolSize; i++)
            {
                var temp = gameObject.AddComponent<AudioSource>();
                temp.playOnAwake = false;
                temp.spatialBlend = 1f; // 3D 音频
                audioSourcePool.Enqueue(temp);
            }
        }

        private void Update()
        {
            if (Player.Instance != null)
                transform.position = Player.Instance.transform.position;
        }

        /// <summary>
        /// 播放音效，可自动使用对象池创建临时 AudioSource
        /// </summary>
        public void PlayAudio(string audioName, Vector3 position, bool isLoop = false, float volume = 1f)
        {
            if (!audioClips.TryGetValue(audioName, out AudioClip clip))
            {
                Debug.LogWarning("音频文件未找到：" + audioName);
                return;
            }

            AudioSource source = GetAudioSourceFromPool();
            source.transform.position = position;
            source.clip = clip;
            source.loop = isLoop;
            source.volume = volume;
            source.Play();

            if (!isLoop)
            {
                StartCoroutine(RecycleAfterPlay(source));
            }
        }

        /// <summary>
        /// 停止播放并回收
        /// </summary>
        public void StopAudio(AudioSource source)
        {
            source.Stop();
            source.loop = false;
            RecycleAudioSource(source);
        }

        // 对象池获取
        private AudioSource GetAudioSourceFromPool()
        {
            if (audioSourcePool.Count > 0)
                return audioSourcePool.Dequeue();
            else
            {
                var temp = gameObject.AddComponent<AudioSource>();
                temp.playOnAwake = false;
                temp.spatialBlend = 1f;
                return temp;
            }
        }

        // 播放结束后回收
        private System.Collections.IEnumerator RecycleAfterPlay(AudioSource source)
        {
            yield return new WaitWhile(() => source.isPlaying);
            RecycleAudioSource(source);
        }

        private void RecycleAudioSource(AudioSource source)
        {
            source.clip = null;
            source.loop = false;
            audioSourcePool.Enqueue(source);
        }
    }
}
