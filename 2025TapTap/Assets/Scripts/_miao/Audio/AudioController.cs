using UnityEngine;
namespace miao
{
    public class AudioController : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            var audioTrigger = other.GetComponent<AudioTrigger>();
            if (audioTrigger != null)
            {
                audioTrigger.enabled = true; // ���봥��������ʱ������Ƶ
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var audioTrigger = other.GetComponent<AudioTrigger>();
            if (audioTrigger != null)
            {
                audioTrigger.enabled = false; // �뿪����������ʱ������Ƶ
            }
        }
    }
}


