using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using miao;

public class GameManager : MonoBehaviour
{
    [ExecuteAlways] // 编辑器和运行时都执行
    [SerializeField] private RenderSettingsSO _renderseter;
    [SerializeField] private int fps = 15;
    public Camera current_RT_cam;

    public static GameManager Instance;//单例
    private void OnValidate()
    {
        //Inspector修改参数时触发
        setCamSize();
    }
    private void Awake()
    {
        Application.targetFrameRate = fps;
        Instance = this;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        setCamSize();
    }
    private void setCamSize()
    {
        //设置画面大小
        if (current_RT_cam == null) 
        {
            Debug.LogError("没有设置当前RT_cam");
            return; 
        }
        this.current_RT_cam.GetComponent<Camera>().orthographicSize = _renderseter.RT_Size;
    }
}
