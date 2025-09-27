using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miao.day_and_night
{
    public class DayAndNight : MonoBehaviour
    {
        [Header("����(Gradients)")]
        [Tooltip("����ɫ����")]
        [SerializeField] private Gradient fogGradient;//����ɫ����
        [Tooltip("�����⽥��")]
        [SerializeField] private Gradient ambientGradient;//�����⽥��
        [Tooltip("������ɫ����")]
        [SerializeField] private Gradient directionLightGradient;//������ɫ����
        [Tooltip("��պ���ɫ����")]
        [SerializeField] private Gradient skyboxTintGradient;//��պ���ɫ����

        [Header("������Դ(Enviromental Assets)")]
        [SerializeField] private Light directionalLight;
        [SerializeField] private Material skyboxMaterial;

        [Header("����(Variables)")]
        [SerializeField] private float dayDurationInSeconds = 60f;//һ�����ʱ�䣨�룩
        [SerializeField] private float rotationSpeed = 1f;

        private float currentTime = 0;

        private void Update()
        {
            UpdateTime();
            UpdateDayNightCycle();
            RotateSkybox();
        }

        private void UpdateTime()
        {
            currentTime += Time.deltaTime / dayDurationInSeconds;
            currentTime = Mathf.Repeat(currentTime, 1f);
        }

        private void UpdateDayNightCycle()
        {
            float sunPosition = Mathf.Repeat(currentTime + 0.25f, 1f);
            directionalLight.transform.rotation = Quaternion.Euler(sunPosition * 360f, 0f, 0f);

            RenderSettings.fogColor = fogGradient.Evaluate(currentTime);
            RenderSettings.ambientLight = ambientGradient.Evaluate(currentTime);

            directionalLight.color = directionLightGradient.Evaluate(currentTime);

            skyboxMaterial.SetColor("_Tint", skyboxTintGradient.Evaluate(currentTime));
        }

        private void RotateSkybox()
        {
            float currentRotation = skyboxMaterial.GetFloat("_Rotation");
            float newRotation = currentRotation + rotationSpeed * Time.deltaTime;
            newRotation = Mathf.Repeat(newRotation, 360f);
            skyboxMaterial.SetFloat("_Rotation", newRotation);
        }

        private void OnApplicationQuit()
        {
            skyboxMaterial.SetColor("_Tint", new Color(0.5f, 0.5f, 0.5f));
        }
    }

}

