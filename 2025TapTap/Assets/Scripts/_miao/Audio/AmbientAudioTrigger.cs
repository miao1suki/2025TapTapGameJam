using System.Collections;
using UnityEngine;
using AudioSystem; // 引入你的全局 AudioManager

[RequireComponent(typeof(AudioSource), typeof(Collider))]
public class AmbientAudioTrigger : MonoBehaviour
{
    private AudioSource audioSource;
    private Coroutine fadeCoroutine;
    private Coroutine globalFadeCoroutine;

    [Header("淡入淡出时间（秒）")]
    [SerializeField] private float fadeDuration = 1.5f;

    [Header("全局背景音量")]
    [SerializeField] private float enterGlobalVolume = 0.03f;
    [SerializeField] private float exitGlobalVolume = 0.3f;

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
        if (!other.CompareTag("Player")) return;

        // 淡入自身音效
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeIn(audioSource, fadeDuration));

        // 淡出全局背景音
        if (globalFadeCoroutine != null) StopCoroutine(globalFadeCoroutine);
        globalFadeCoroutine = StartCoroutine(FadeGlobalVolume(AudioManager.Volume, enterGlobalVolume, fadeDuration));
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // 淡出自身音效
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeOut(audioSource, fadeDuration));

        // 恢复全局背景音
        if (globalFadeCoroutine != null) StopCoroutine(globalFadeCoroutine);
        globalFadeCoroutine = StartCoroutine(FadeGlobalVolume(AudioManager.Volume, exitGlobalVolume, fadeDuration));
    }

    private IEnumerator FadeIn(AudioSource source, float duration)
    {
        if (!source.isPlaying) source.Play();

        float startVolume = 0f;
        float targetVolume = 1.0f;
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

    private IEnumerator FadeGlobalVolume(float start, float target, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            AudioManager.Volume = Mathf.Lerp(start, target, time / duration);
            yield return null;
        }
        AudioManager.Volume = target;
    }
}
