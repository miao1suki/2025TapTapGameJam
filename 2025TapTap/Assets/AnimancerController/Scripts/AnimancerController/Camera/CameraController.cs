using Cinemachine;
using System;
using UnityEngine;
using UnityEngine.Playables;
/**************************************************************************
����: HuHu
����: 3112891874@qq.com
����: �����������������ƽ�������������
**************************************************************************/

public class CameraController : MonoBehaviour
{
    public float defaultDistance;
    [Range(0.5f,3)]public float minDistance;
    [Range(3,10)] public float maxDistance;
    private float currentDistance;
    public float sensitivity;
    public float smoothness;

    private CinemachineFramingTransposer virtualCamera;
    private PlayableDirector playableDirector;
    private InputService inputService;

    private void Awake()
    {
        inputService = InputService.Instance;

        virtualCamera = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>();
        playableDirector = transform.GetComponent<PlayableDirector>();
        currentDistance = defaultDistance;
        virtualCamera.m_CameraDistance = currentDistance;
    }
 
  
    private void Update()
    {
        GetMouseScroll();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void LateUpdate()
    {
        UpdateCameraDistance();
    }

    private void GetMouseScroll()
    {
        currentDistance -= inputService.inputMap.Player.Scroll.ReadValue<Vector2>().y * Time.deltaTime* sensitivity;
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
    }
    private void UpdateCameraDistance()
    {
        if (playableDirector!=null)
        {
            if (playableDirector?.state == PlayState.Playing)
            {
                // ������ڲ��ţ��������� m_CameraDistance
                return;
            }
        }
        virtualCamera.m_CameraDistance = Mathf.Lerp(virtualCamera.m_CameraDistance, currentDistance,Time.deltaTime* smoothness) ;
    }

  
}
