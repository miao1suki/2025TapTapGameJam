using UnityEngine;
using System.Collections.Generic;

namespace miao
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;

        private Dictionary<string, AudioClip> audioClips;

        // �����
        private Queue<AudioSource> audioSourcePool = new Queue<AudioSource>();
        private int poolSize = 10; // Ĭ�ϳش�С

        private void Awake()
        {
            Instance = this;

            // ������Ƶ
            audioClips = new Dictionary<string, AudioClip>();
            AudioClip[] clips = Resources.LoadAll<AudioClip>("Ignore/Audio");
            foreach (var clip in clips)
                audioClips[clip.name] = clip;

            // Ԥ���������
            for (int i = 0; i < poolSize; i++)
            {
                var temp = gameObject.AddComponent<AudioSource>();
                temp.playOnAwake = false;
                temp.spatialBlend = 1f; // 3D ��Ƶ
                audioSourcePool.Enqueue(temp);
            }
        }

        private void Update()
        {
            if (Player.Instance != null)
                transform.position = Player.Instance.transform.position;
        }

        /// <summary>
        /// ������Ч�����Զ�ʹ�ö���ش�����ʱ AudioSource
        /// </summary>
        public void PlayAudio(string audioName, Vector3 position, bool isLoop = false, float volume = 1f)
        {
            if (!audioClips.TryGetValue(audioName, out AudioClip clip))
            {
                Debug.LogWarning("��Ƶ�ļ�δ�ҵ���" + audioName);
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
        /// ֹͣ���Ų�����
        /// </summary>
        public void StopAudio(AudioSource source)
        {
            source.Stop();
            source.loop = false;
            RecycleAudioSource(source);
        }

        // ����ػ�ȡ
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

        // ���Ž��������
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
