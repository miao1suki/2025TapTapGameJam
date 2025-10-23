using UnityEngine;
using UnityEngine.InputSystem;

namespace AudioSystem
{
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager instance;
        public static AudioManager Instance => instance;

        [SerializeField] private AudioSource bgSource;
        [SerializeField] private AudioSource effectSource;
        [SerializeField] private AudioSource uiEffectSource;

        [SerializeField, Range(0, 1)] private float volume = 1;
        [SerializeField, Range(0, 1)] private float bgVolume = 1;
        [SerializeField, Range(0, 1)] private float effectVolume = 1;
        [SerializeField, Range(0, 1)] private float uiEffectVolume = 1;

        public static float Volume
        {
            get => instance.volume;
            set
            {
                instance.volume = value;
                instance.bgSource.volume = instance.bgVolume * instance.volume;
                instance.effectSource.volume = instance.effectVolume * instance.volume;
                instance.uiEffectSource.volume = instance.uiEffectVolume * instance.volume;
            }
        }
        public static float BackgroundMusicVolume
        {
            get => instance.bgVolume;
            set
            {
                instance.bgVolume = value;
                instance.bgSource.volume = instance.bgVolume * instance.volume;
            }
        }
        public static float UIEffectVolume
        {
            get => instance.uiEffectVolume;
            set
            {
                instance.uiEffectVolume = value;
                instance.uiEffectSource.volume = instance.uiEffectVolume * instance.volume;
            }
        }
        public static float EffectSoundVolume
        {
            get => instance.effectVolume;
            set
            {
                instance.effectVolume = value;
                instance.effectSource.volume = instance.effectVolume * instance.volume;
            }
        }

        public static AudioSource BgSource => instance.bgSource;

        private void Awake()
        {
            if (instance)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            Volume = volume;
            DontDestroyOnLoad(gameObject);
        }

        public static void PlaySound(AudioClip audioClip, AudioType type)
        {
            AudioSource targetSource = type switch
            {
                AudioType.BackgoundMusic => instance.bgSource,
                AudioType.EffectSound => instance.effectSource,
                AudioType.UIEffectSound => instance.uiEffectSource,
                _ => null
            };
            targetSource.PlayOneShot(audioClip);
        }
    }

    public enum AudioType
    {
        BackgoundMusic,
        EffectSound,
        UIEffectSound
    }
}