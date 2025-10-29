using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StateController : MonoBehaviour
{
    public static StateController Instance;
    public GameObject camWater;

    private void Awake()
    {
        Instance = this;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDestroy()
    { 
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ����������ɺ�ִ��
        if (scene.name != "Title")
        {
            StartCoroutine(FindCamWaterWhenReady());
        }
    }
    private IEnumerator FindCamWaterWhenReady()
    {
        // ��һ֡ȷ���������嶼��ʼ�����
        yield return null;

        camWater = FindInactiveObjectByTag("CamWater");  
    }


    private GameObject FindInactiveObjectByTag(string tag)
    {
        var rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (var root in rootObjects)
        {
            var found = FindInChildrenByTag(root.transform, tag);
            if (found != null)
                return found.gameObject;
        }
        return null;
    }
    private Transform FindInChildrenByTag(Transform parent, string tag)
    {
        if (parent.CompareTag(tag))
            return parent;

        foreach (Transform child in parent)
        {
            var result = FindInChildrenByTag(child, tag);
            if (result != null)
                return result;
        }
        return null;
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void ExecuteAfter(float delay, Action action)
    {
        StartCoroutine(ExecuteAfterCoroutine(delay, action));
    }
    private IEnumerator ExecuteAfterCoroutine(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }

    /// <summary>
    /// ÿ�� interval ��ִ��һ�� action���ܹ�ִ�� totalDuration ���ֹͣ
    /// </summary>
    /// <param name="interval">�ظ����ʱ��</param>
    /// <param name="totalDuration">�ܳ���ʱ��</param>
    /// <param name="action">Ҫִ�еķ���</param>
    public void ExecuteRepeated(float interval, float totalDuration, Action action)
    {
        StartCoroutine(ExecuteRepeatedCoroutine(interval, totalDuration, action));
    }

    private IEnumerator ExecuteRepeatedCoroutine(float interval, float totalDuration, Action action)
    {
        float elapsed = 0f;
        while (elapsed < totalDuration)
        {
            action?.Invoke();
            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }
    }


}
