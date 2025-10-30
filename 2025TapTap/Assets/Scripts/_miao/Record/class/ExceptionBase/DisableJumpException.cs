using System.Collections;
using UnityEngine;
using AudioSystem; // ����AudioManager�����ռ�

namespace miao
{
    [CreateAssetMenu(fileName = "DisableJumpException", menuName = "Game/Record Exception/Disable Jump")]
    public class DisableJumpException : RecordExceptionBase
    {
        [Header("�滻��Ŀ����")]
        [SerializeField] private string newMusicName = "�޻���"; // Ҫ���ŵ���������������Resources/Audio�У�
        [SerializeField] private float newMusicVolume = 0.3f;   // ����������
        [SerializeField] private float revertDelay = 141f;       // ���Ŷ�ú󻻻�ԭ��

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

            // ����ԭ����Ϣ
            originalClip = bg.clip;
            originalTime = bg.time;
            originalVolume = AudioSystem.AudioManager.Volume;

            // ��������Ŀ
            AudioClip newClip = Resources.Load<AudioClip>($"Audio/{newMusicName}");
            if (newClip == null)
            {
                Debug.LogWarning($"�Ҳ�����Ƶ��Audio/{newMusicName}");
                return;
            }

            // �л�����
            bg.clip = newClip;
            bg.time = 0f;
            bg.loop = true;
            bg.Play();

            // ��������
            AudioSystem.AudioManager.Volume = newMusicVolume;

            // ��ָ��ʱ��󻻻�ԭ��
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
