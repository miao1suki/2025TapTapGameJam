using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using miao;
using UnityEngine.SceneManagement;
using UnityEditor;
using UI;
using ScoreSystem;
using AchievementSystem;


public class GameManager : MonoBehaviour
{
    [ExecuteAlways] // 编辑器和运行时都执行

    [SerializeField]
    private List<GameObject> dontDestroyObjects = new List<GameObject>();
    [SerializeField] private RenderSettingsSO _renderseter;
    [SerializeField] private GameObject GameGlobalMenu;
    [SerializeField] private GameObject ScoreTexe;
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

    public ScoreText text;

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

        ScoreTexe.SetActive(false);

    }
    void Start()
    {
        //场景切换时重复执行
        current_RT_cam = Camera.main.transform.GetChild(0).gameObject.GetComponent<Camera>();
        Player.Instance.GetComponent<PhysicsBody>().useGravity = false;
        SetConstraints(true, false);//  设置Player的rb锁定（位置，旋转）

        Instance = this;
        DontDestroyOnLoad(gameObject);
        AddDontDestroyObject();

        // 显示鼠标
        Cursor.visible = true;

        //setCamSize();
    }

    // Update is called once per frame
    void Update()
    {
        //setCamSize();
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name != "Title")
        {
            ShowUI();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name == "Title")
        {
            GameExit();
        }
    }
    public void ShowUI()
    {
        if (!GameGlobalMenu)
        {
            Debug.LogError("未找到GameGlobalMenu");
        }

        GameGlobalMenu.SetActive(!GameGlobalMenu.activeSelf);
    }
    public void SetScoreText(int score)
    {
        text.Value = score;
    }

    public void ResetPos()
    {
        Player.Instance.transform.position = new Vector3(Player.Instance.transform.position.x, Player.Instance.transform.position.y + 10.0f, Player.Instance.transform.position.z);
    }

    public void CursorON()
    {
        Cursor.visible = true;
    }

    public void CursorOFF()
    {
        Cursor.visible = false;
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

        // 区分 正交 / 透视
        if (current_RT_cam.orthographic)
        {
            current_RT_cam.orthographicSize = _renderseter.RT_Size;
        }
        else
        {
            // 透视时用 FOV 映射 RT_Size（近似策略）
            // RT_Size 0.1 -> FOV 30, RT_Size 24 -> FOV 90
            current_RT_cam.fieldOfView = Mathf.Lerp(30f, 90f, Mathf.InverseLerp(0.1f, 24f, _renderseter.RT_Size));
        }

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

        float near = 0.01f;
        float far = 1000.0f;

        if (current_RT_cam.orthographic)
        {
            float minSize = 0.1f;   // 最近 RT_Size
            float maxSize = 24f;    // 最远 RT_Size

            // 归一化 RT_Size 到 0~1
            float t = Mathf.InverseLerp(minSize, maxSize, _renderseter.RT_Size);

            // 根据拉近拉远线性插值裁切平面
            near = Mathf.Lerp(-1f, -35f, t);
            far = Mathf.Lerp(15f, 54f, t);
        }
        else
        {
            // 透视相机：保持固定范围，避免被动态调整
            near = 0.01f;
            far = 1000f;
        }
       

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
        Cursor.visible = false;
        Player.Instance.GetComponent<PhysicsBody>().useGravity = true;
        SetConstraints(false,true);


        // 异步加载场景
        StartCoroutine(LoadSceneAsync("miao_testScene1"));



    }
    public void GameExit()
    {
        // 保存玩家最高分
        ScoreManager.Instance?.SaveScore();

        PhysicsSystem.Instance.GameExit();

        AchievementManager.Ins.SaveAchievements();

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

        current_RT_cam = Camera.main.transform.GetChild(0).gameObject.GetComponent<Camera>();
        var allData = Resources.LoadAll<RecordData>("RecordData");

        if (current_RT_cam != null)
        {
            // 把 RTCam 设为透视并调整参数
            current_RT_cam.orthographic = false;
            current_RT_cam.fieldOfView = Mathf.Lerp(30f, 90f, Mathf.InverseLerp(0.1f, 24f, _renderseter.RT_Size));
        }

        //此处设定应在场景加载完成后生效
        PhysicsSystem.Instance.gravityDirection = new Vector3(0, -1, 0);

        Camera.main.GetComponent<CameraController>().isPerspectiveMode = true;


        _renderseter.RT_Size = 7.0f;
        setCamSize();

        ScoreTexe.SetActive(true);

        Player.Instance.transform.position = new Vector3(0,5,0);
        Player.Instance.transform.rotation = Quaternion.Euler(0, -60, 0);


    }
   


}
