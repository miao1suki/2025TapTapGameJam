using TMPro;
using UnityEngine;

namespace AudioSystem
{
    public class AudioControlPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private TextMeshProUGUI Bg_text;
        [SerializeField] private TextMeshProUGUI Effect_text;
        [SerializeField] private TextMeshProUGUI UI_text;
        public void AdjustVolume(float volume)
        {
            AudioManager.Volume = volume / 100;
            text.text = $"{volume}";
        }

        public void AdjustBgVolume(float volume)
        {
            AudioManager.BackgroundMusicVolume = volume / 100;
            Bg_text.text = $"{volume}";
        }

        public void AdjustEffectVolume(float volume)
        {
            AudioManager.EffectSoundVolume = volume / 100;
            Effect_text.text = $"{volume}";
        }

        public void AdjustUIVolume(float volume)
        {
            AudioManager.UIEffectVolume = volume / 100;
            UI_text.text = $"{volume}";
        }

    }
}