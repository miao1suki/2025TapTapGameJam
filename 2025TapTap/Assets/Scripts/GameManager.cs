using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using miao;

public class GameManager : MonoBehaviour
{
    [ExecuteAlways] // �༭��������ʱ��ִ��

    [SerializeField] private RenderSettingsSO _renderseter;
    //�ⲿ���� RenderSettingsSO ������
    public RenderSettingsSO Renderseter
    {
        get => _renderseter;
        set => _renderseter = value;
    }

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
        //���û����С
        if (current_RT_cam == null) 
        {
            Debug.LogError("û�����õ�ǰRT_cam");
            return; 
        }
        this.current_RT_cam.GetComponent<Camera>().orthographicSize = _renderseter.RT_Size;
        auto_Pixelset();
        _renderseter.ApplySettings();
        UpdateClipPlanes();
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


}
