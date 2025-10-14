using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using miao;

public class GameManager : MonoBehaviour
{
    [ExecuteAlways] // 编辑器和运行时都执行

    [SerializeField] private RenderSettingsSO _renderseter;
    //外部访问 RenderSettingsSO 的属性
    public RenderSettingsSO Renderseter
    {
        get => _renderseter;
        set => _renderseter = value;
    }

    [SerializeField] private int fps = 15;
    public Camera current_RT_cam;

    [Header("自动控制 RT_Pixel")]
    [Tooltip("开启后 RT_Pixel 会随 RT_Size 自动变化")]
    public bool autoPixelControl = true;


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
        setCamSize();
    }

    // Update is called once per frame
    void Update()
    {
        //setCamSize();
    }
    public void setCamSize()
    {
        //设置画面大小
        if (current_RT_cam == null) 
        {
            Debug.LogError("没有设置当前RT_cam");
            return; 
        }
        this.current_RT_cam.GetComponent<Camera>().orthographicSize = _renderseter.RT_Size;
        auto_Pixelset();
        _renderseter.ApplySettings();
        UpdateClipPlanes();
    }

    private void auto_Pixelset()
    {
        // 自动根据 RT_Size 调整 RT_Pixel
        if (autoPixelControl)
        {
            // 每个整数范围对应一个 RT_Pixel
            // RT_Size 0.1~1 -> 1，1.1~2 -> 2，2.1~3 -> 3 ...
            int newPixel = Mathf.Clamp(Mathf.CeilToInt(_renderseter.RT_Size), 1, 24);
            if (_renderseter.RT_Pixel != newPixel)
            {
                _renderseter.RT_Pixel = newPixel;
            }
        }
    }

    private void UpdateClipPlanes()
    {
        if (current_RT_cam == null) return;

        float minSize = 0.1f;   // 最近 RT_Size
        float maxSize = 24f;    // 最远 RT_Size

        // 归一化 RT_Size 到 0~1
        float t = Mathf.InverseLerp(minSize, maxSize, _renderseter.RT_Size);

        // 根据拉近拉远线性插值裁切平面
        float near = Mathf.Lerp(-1f, -35f, t);
        float far = Mathf.Lerp(15f, 54f, t);

        current_RT_cam.nearClipPlane = near;
        current_RT_cam.farClipPlane = far;
    }


}
