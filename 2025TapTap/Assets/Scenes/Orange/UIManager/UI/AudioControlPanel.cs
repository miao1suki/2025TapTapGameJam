using UnityEngine;

namespace AudioSystem
{
    public class AudioControlPanel : MonoBehaviour
    {
        public AudioClip clip;
        private void Start()
        {
            AudioManager.PlaySound(clip, AudioType.BackgoundMusic);
        }

        public void AdjustVolume(float volume) => AudioManager.Volume = volume;
        public void AdjustBgVolume(float volume) => AudioManager.BackgroundMusicVolume = volume;
        public void AdjustEffectVolume(float volume) => AudioManager.EffectSoundVolume = volume;
        public void AdjustUIVolume(float volume) => AudioManager.UIEffectVolume = volume;
    }
}