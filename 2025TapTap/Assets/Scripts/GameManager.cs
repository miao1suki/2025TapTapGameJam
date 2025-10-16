using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using miao;
using UnityEngine.SceneManagement;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    [ExecuteAlways] // 编辑器和运行时都执行

    private float deltaTime = 0.0f;
    [SerializeField]
    private List<GameObject> dontDestroyObjects = new List<GameObject>();
    [SerializeField] private RenderSettingsSO _renderseter;
    //外部访问 RenderSettingsSO 的属性
    public RenderSettingsSO Renderseter
    {
        get => _renderseter;
        set => _renderseter = value;
    }

    private float lastRTSize = -1f; // 上一帧的 RT_Size

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
        //场景切换时不重复执行
        Application.targetFrameRate = fps;
        Instance = this;

        _renderseter.RT_Size = 3.0f;
        setCamSize();
        Camera.main.GetComponent<CamSize>().enabled = false;

    }
    void Start()
    {
        //场景切换时重复执行
        current_RT_cam = Camera.main.transform.GetChild(0).gameObject.GetComponent<Camera>();
        Camera.main.gameObject.GetComponent<CamSize>().useOffscreenIndicator = false;
        Player.Instance.GetComponent<PhysicsBody>().useGravity = false;
        SetConstraints(true, false);

        Instance = this;
        DontDestroyOnLoad(gameObject);
        AddDontDestroyObject();
        //setCamSize();
    }

    // Update is called once per frame
    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        if (Time.frameCount % 30 == 0) // 每30帧输出一次
        {
            int fps = Mathf.RoundToInt(1.0f / deltaTime);
            Debug.Log("FPS: " + fps);
        }
        //setCamSize();
        if (InputController.Instance.get_Key("Esc"))
        {
            GameExit();
        }

        if(Input.GetKeyDown(KeyCode.L))
        {
            GameStart();
        }
    }
    public void AddDontDestroyObject()
    {
        foreach (GameObject obj in dontDestroyObjects)
        {
            if (obj != null)
            {
                DontDestroyOnLoad(obj);
            }
        }
    }
    public void setCamSize()
    {
        //设置画面大小
        if (current_RT_cam == null) 
        {
            Debug.LogError("没有设置当前RT_cam");
            return; 
        }

        // 检查 RT_Size 是否变化
        if (Mathf.Approximately(lastRTSize, _renderseter.RT_Size))
        {
            return; 
        }


        this.current_RT_cam.GetComponent<Camera>().orthographicSize = _renderseter.RT_Size;
        auto_Pixelset();
        _renderseter.ApplySettings();
        UpdateClipPlanes();

        // 更新上一帧 RT_Size
        lastRTSize = _renderseter.RT_Size;
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

    //  设置Player的rb锁定（位置，旋转）
    public void SetConstraints(bool freezePosition, bool freezeRotation)
    {
        Player.Instance.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

        if (freezePosition)
            Player.Instance.GetComponent<Rigidbody>().constraints |= RigidbodyConstraints.FreezePosition;

        if (freezeRotation)
            Player.Instance.GetComponent<Rigidbody>().constraints |= RigidbodyConstraints.FreezeRotation;
    }

    public void GameStart()
    {
        //此处设定应在场景切换前生效
        if(current_RT_cam.gameObject.transform.parent.GetComponent<CamSize>() != null)
        {
            current_RT_cam.gameObject.transform.parent.GetComponent<CamSize>().useOffscreenIndicator = true;
        }

        Player.Instance.GetComponent<PhysicsBody>().useGravity = true;
        SetConstraints(false,true);


        // 异步加载场景
        StartCoroutine(LoadSceneAsync("miao_testScene1"));



    }
    private void GameExit()
    {
        PhysicsSystem.Instance.GameExit();

#if UNITY_EDITOR
        EditorApplication.isPlaying = false; // 在编辑器模式下停止播放
#else
            Application.Quit(); // 在实际运行时退出
#endif
    }


    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);


        while (!asyncOperation.isDone)
        {
            yield return null; // 等待直到场景加载完成
        }

        //此处设定应在场景加载完成后生效
        PhysicsSystem.Instance.gravityDirection = new Vector3(0, -1, 0);
        current_RT_cam = Camera.main.transform.GetChild(0).gameObject.GetComponent<Camera>();
        Camera.main.GetComponent<CamSize>().enabled = true;
        Camera.main.gameObject.GetComponent<CamSize>().useOffscreenIndicator = true;
        Player.Instance.transform.position = new Vector3(0,5,0);
        Player.Instance.transform.rotation = Quaternion.Euler(0, -60, 0);
        _renderseter.RT_Size = 7.0f;
        setCamSize();
    }
}
