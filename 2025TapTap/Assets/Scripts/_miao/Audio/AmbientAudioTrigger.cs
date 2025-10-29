using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(Collider))]
public class AmbientAudioTrigger : MonoBehaviour
{
    private AudioSource audioSource;
    private Coroutine fadeCoroutine;

    [Header("淡入淡出时间（秒）")]
    [SerializeField] private float fadeDuration = 1.5f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        // 确保碰撞体是触发器
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;

        // 音频循环，不自动播放
        audioSource.loop = true;
        audioSource.playOnAwake = false;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeIn(audioSource, fadeDuration));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeOut(audioSource, fadeDuration));
        }
    }

    private IEnumerator FadeIn(AudioSource source, float duration)
    {
        if (!source.isPlaying)
            source.Play();

        float startVolume = 0f;
        float targetVolume = 0.5f;
        source.volume = 0f;

        float time = 0f;
        while (time < duration)
        {
            source.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        source.volume = targetVolume;
    }

    private IEnumerator FadeOut(AudioSource source, float duration)
    {
        float startVolume = source.volume;
        float time = 0f;

        while (time < duration)
        {
            source.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        source.Stop();
        source.volume = startVolume;
    }
}
