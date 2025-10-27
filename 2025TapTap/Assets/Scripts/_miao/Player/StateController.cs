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
    /// 每隔 interval 秒执行一次 action，总共执行 totalDuration 秒后停止
    /// </summary>
    /// <param name="interval">重复间隔时间</param>
    /// <param name="totalDuration">总持续时间</param>
    /// <param name="action">要执行的方法</param>
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
