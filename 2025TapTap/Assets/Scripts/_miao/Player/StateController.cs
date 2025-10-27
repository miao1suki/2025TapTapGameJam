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
    }
    private void Start()
    {
        if(SceneManager.GetActiveScene().name != "Title")
        {
            camWater = GameObject.FindGameObjectWithTag("CamWater");
        }
    }

    public IEnumerator ExecuteAfterCoroutine(float delay, Action action)
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
