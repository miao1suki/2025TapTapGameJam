using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using miao;
using UnityEngine.SceneManagement;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    [ExecuteAlways] // �༭��������ʱ��ִ��

    private float deltaTime = 0.0f;
    [SerializeField]
    private List<GameObject> dontDestroyObjects = new List<GameObject>();
    [SerializeField] private RenderSettingsSO _renderseter;
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
        Camera.main.GetComponent<CamSize>().enabled = false;

    }
    void Start()
    {
        //�����л�ʱ�ظ�ִ��
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
        if (Time.frameCount % 30 == 0) // ÿ30֡���һ��
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


        this.current_RT_cam.GetComponent<Camera>().orthographicSize = _renderseter.RT_Size;
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

        float minSize = 0.1f;   // ��� RT_Size
        float maxSize = 24f;    // ��Զ RT_Size

        // ��һ�� RT_Size �� 0~1
        float t = Mathf.InverseLerp(minSize, maxSize, _renderseter.RT_Size);

        // ����������Զ���Բ�ֵ����ƽ��
        float near = Mathf.Lerp(-1f, -35f, t);
        float far = Mathf.Lerp(15f, 54f, t);

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
        if(current_RT_cam.gameObject.transform.parent.GetComponent<CamSize>() != null)
        {
            current_RT_cam.gameObject.transform.parent.GetComponent<CamSize>().useOffscreenIndicator = true;
        }

        Player.Instance.GetComponent<PhysicsBody>().useGravity = true;
        SetConstraints(false,true);


        // �첽���س���
        StartCoroutine(LoadSceneAsync("miao_testScene1"));



    }
    private void GameExit()
    {
        PhysicsSystem.Instance.GameExit();

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

        //�˴��趨Ӧ�ڳ���������ɺ���Ч
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
