using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using miao;

public class GameManager : MonoBehaviour
{
    [ExecuteAlways] // �༭��������ʱ��ִ��
    [SerializeField] private RenderSettingsSO _renderseter;
    [SerializeField] private int fps = 15;
    public Camera current_RT_cam;

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
        
    }

    // Update is called once per frame
    void Update()
    {
        setCamSize();
    }
    private void setCamSize()
    {
        //���û����С
        if (current_RT_cam == null) 
        {
            Debug.LogError("û�����õ�ǰRT_cam");
            return; 
        }
        this.current_RT_cam.GetComponent<Camera>().orthographicSize = _renderseter.RT_Size;
    }
}
