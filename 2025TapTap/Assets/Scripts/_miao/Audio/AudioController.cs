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
                audioTrigger.gameObject.SetActive(true); // 进入触发器区域时启用音频
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var audioTrigger = other.GetComponent<AudioTrigger>();
            if (audioTrigger != null)
            {
                audioTrigger.gameObject.SetActive(false); // 离开触发器区域时禁用音频
            }
        }
    }
}


