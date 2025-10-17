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

            // ����������Ƶ�ļ����ֵ�
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

        // ������Ƶ���Ʋ�����Ч
        public void PlayAudio(string audioName, AudioSource source, bool isSinglePlay)
        {
            if (audioClips.TryGetValue(audioName, out AudioClip clip))
            {
                source.clip = clip;

                if (isSinglePlay)
                {
                    // ʹ�� PlayOneShot ����һ����Ƶ
                    source.PlayOneShot(clip);
                }
                else
                {
                    // ʹ�� Play ���Ų�ѭ��
                    source.loop = true;
                    source.Play();
                }
            }
            else
            {
                Debug.LogWarning("��Ƶ�ļ�δ�ҵ���" + audioName);
            }
        }

        // ֹͣ��Ƶ����
        public void StopAudio(AudioSource source)
        {
            source.Stop(); 
            source.loop = false; 
        }
    }
}


