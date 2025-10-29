using System.Collections;
using UnityEngine;
using AudioSystem; // �������ȫ�� AudioManager

[RequireComponent(typeof(AudioSource), typeof(Collider))]
public class AmbientAudioTrigger : MonoBehaviour
{
    private AudioSource audioSource;
    private Coroutine fadeCoroutine;
    private Coroutine globalFadeCoroutine;

    [Header("���뵭��ʱ�䣨�룩")]
    [SerializeField] private float fadeDuration = 1.5f;

    [Header("ȫ�ֱ�������")]
    [SerializeField] private float enterGlobalVolume = 0.03f;
    [SerializeField] private float exitGlobalVolume = 0.3f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        // ȷ����ײ���Ǵ�����
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;

        // ��Ƶѭ�������Զ�����
        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // ����������Ч
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeIn(audioSource, fadeDuration));

        // ����ȫ�ֱ�����
        if (globalFadeCoroutine != null) StopCoroutine(globalFadeCoroutine);
        globalFadeCoroutine = StartCoroutine(FadeGlobalVolume(AudioManager.Volume, enterGlobalVolume, fadeDuration));
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // ����������Ч
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeOut(audioSource, fadeDuration));

        // �ָ�ȫ�ֱ�����
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
