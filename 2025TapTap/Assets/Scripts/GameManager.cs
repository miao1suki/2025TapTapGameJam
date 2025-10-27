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
    [ExecuteAlways] // �༭��������ʱ��ִ��

    [SerializeField]
    private List<GameObject> dontDestroyObjects = new List<GameObject>();
    [SerializeField] private RenderSettingsSO _renderseter;
    [SerializeField] private GameObject GameGlobalMenu;
    [SerializeField] private GameObject ScoreTexe;
    //�ⲿ���� RenderSettingsSO ������
    public RenderSettingsSO Renderseter
    {
        get => _renderseter;
        set => _renderseter = value;
    }

    private float lastRTSize = -1f; // ��һ֡�� RT_Size

    [SerializeField] private int fps = 15;
    public Camera current_RT_cam;

    [Header("�Զ����� RT_Pixel")]
    [Tooltip("������ RT_Pixel ���� RT_Size �Զ��仯")]
    public bool autoPixelControl = true;

    public ScoreText text;

    public static GameManager Instance;//����
    private void OnValidate()
    {
        //Inspector�޸Ĳ���ʱ����
        setCamSize();

    }
    private void Awake()
    {
        //�����л�ʱ���ظ�ִ��
        Application.targetFrameRate = fps;
        Instance = this;

        _renderseter.RT_Size = 3.0f;
        setCamSize();

        ScoreTexe.SetActive(false);

    }
    void Start()
    {
        //�����л�ʱ�ظ�ִ��
        current_RT_cam = Camera.main.transform.GetChild(0).gameObject.GetComponent<Camera>();
        Player.Instance.GetComponent<PhysicsBody>().useGravity = false;
        SetConstraints(true, false);//  ����Player��rb������λ�ã���ת��

        Instance = this;
        DontDestroyOnLoad(gameObject);
        AddDontDestroyObject();

        // ��ʾ���
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
            Debug.LogError("δ�ҵ�GameGlobalMenu");
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
        //���û����С
            if (current_RT_cam == null)
            {
                Debug.LogError("û�����õ�ǰRT_cam");
                return;
            }


        // ��� RT_Size �Ƿ�仯
        if (Mathf.Approximately(lastRTSize, _renderseter.RT_Size))
        {
            return; 
        }

        // ���� ���� / ͸��
        if (current_RT_cam.orthographic)
        {
            current_RT_cam.orthographicSize = _renderseter.RT_Size;
        }
        else
        {
            // ͸��ʱ�� FOV ӳ�� RT_Size�����Ʋ��ԣ�
            // RT_Size 0.1 -> FOV 30, RT_Size 24 -> FOV 90
            current_RT_cam.fieldOfView = Mathf.Lerp(30f, 90f, Mathf.InverseLerp(0.1f, 24f, _renderseter.RT_Size));
        }

        auto_Pixelset();
        _renderseter.ApplySettings();
        UpdateClipPlanes();

        // ������һ֡ RT_Size
        lastRTSize = _renderseter.RT_Size;
    }

    private void auto_Pixelset()
    {
        // �Զ����� RT_Size ���� RT_Pixel
        if (autoPixelControl)
        {
            // ÿ��������Χ��Ӧһ�� RT_Pixel
            // RT_Size 0.1~1 -> 1��1.1~2 -> 2��2.1~3 -> 3 ...
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
            float minSize = 0.1f;   // ��� RT_Size
            float maxSize = 24f;    // ��Զ RT_Size

            // ��һ�� RT_Size �� 0~1
            float t = Mathf.InverseLerp(minSize, maxSize, _renderseter.RT_Size);

            // ����������Զ���Բ�ֵ����ƽ��
            near = Mathf.Lerp(-1f, -35f, t);
            far = Mathf.Lerp(15f, 54f, t);
        }
        else
        {
            // ͸����������̶ֹ���Χ�����ⱻ��̬����
            near = 0.01f;
            far = 1000f;
        }
       

        current_RT_cam.nearClipPlane = near;
        current_RT_cam.farClipPlane = far;
    }

    //  ����Player��rb������λ�ã���ת��
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
        //�˴��趨Ӧ�ڳ����л�ǰ��Ч
        Cursor.visible = false;
        Player.Instance.GetComponent<PhysicsBody>().useGravity = true;
        SetConstraints(false,true);


        // �첽���س���
        StartCoroutine(LoadSceneAsync("miao_testScene1"));



    }
    public void GameExit()
    {
        // ���������߷�
        ScoreManager.Instance?.SaveScore();

        PhysicsSystem.Instance.GameExit();

        AchievementManager.Ins.SaveAchievements();

#if UNITY_EDITOR
        EditorApplication.isPlaying = false; // �ڱ༭��ģʽ��ֹͣ����
#else
            Application.Quit(); // ��ʵ������ʱ�˳�
#endif
    }


    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);


        while (!asyncOperation.isDone)
        {
            yield return null; // �ȴ�ֱ�������������
        }

        current_RT_cam = Camera.main.transform.GetChild(0).gameObject.GetComponent<Camera>();
        var allData = Resources.LoadAll<RecordData>("RecordData");

        if (current_RT_cam != null)
        {
            // �� RTCam ��Ϊ͸�Ӳ���������
            current_RT_cam.orthographic = false;
            current_RT_cam.fieldOfView = Mathf.Lerp(30f, 90f, Mathf.InverseLerp(0.1f, 24f, _renderseter.RT_Size));
        }

        //�˴��趨Ӧ�ڳ���������ɺ���Ч
        PhysicsSystem.Instance.gravityDirection = new Vector3(0, -1, 0);

        Camera.main.GetComponent<CameraController>().isPerspectiveMode = true;


        _renderseter.RT_Size = 7.0f;
        setCamSize();

        ScoreTexe.SetActive(true);

        Player.Instance.transform.position = new Vector3(0,5,0);
        Player.Instance.transform.rotation = Quaternion.Euler(0, -60, 0);


    }
   


}
