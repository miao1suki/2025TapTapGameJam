using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miao.day_and_night
{
    public class DayAndNight : MonoBehaviour
    {
        [System.Serializable]
        public class LightData
        {
            public Light light;
            public float nightIntensity = 0.3f; // ҹ������
        }


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
        [SerializeField] public float dayDurationInSeconds = 60f;//һ�����ʱ�䣨�룩
        [SerializeField] private float rotationSpeed = 1f;

        [Header("��Դ�Զ����ؿ���")]
        [Tooltip("�ܿصƹ��б�")]
        [SerializeField] private List<LightData> lights = new List<LightData>();
        [Tooltip("����������Դ")]
        [Range(0.01f, 2f)]
        [SerializeField] private float LightSpeed = 0.1f;

        private float currentTime = 0;

        private bool nightLightsTriggered = false;
        private bool dayLightsTriggered = false;
        private float fpsTimer = 0f;
        private int frameCount = 0;

        public static DayAndNight Instance;
        private void Awake()
        {
            Instance = this;
        }
        private void Start()
        {
            CollectSceneLights();
        }


        private void Update()
        {
            //����ʱ��
            UpdateTime();
            UpdateDayNightCycle();

            //�¼�����
            //////////////////////////////////////////////////////
            // ҹ��ʼ
            if (currentTime >= 0.25f && !nightLightsTriggered)
            {
                nightLightsTriggered = true;
                dayLightsTriggered = false;
                UpdateLight(); // ����ҹ���Դ
            }

            // ���쿪ʼ
            if (currentTime >= 0.75f && !dayLightsTriggered)
            {
                dayLightsTriggered = true;
                nightLightsTriggered = false;
                UpdateLight(); // ���������Դ
            }
            //////////////////////////////////////////////////////
            RotateSkybox();
        }

        private void UpdateTime()
        {
            currentTime += Time.deltaTime / dayDurationInSeconds;
            currentTime = Mathf.Repeat(currentTime, 1f);
            //Debug.Log(currentTime);
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

        private void UpdateLight()
        {
            bool isNight = currentTime >= 0.25f && currentTime <= 0.75f;

            foreach (var lightData in lights)
            {
                if (lightData.light == null) continue;

                float targetIntensity = isNight ? lightData.nightIntensity : 0f;

                // ����Э���𲽸ı��ǿ
                StopCoroutine(ChangeLightIntensity(lightData.light, targetIntensity));
                StartCoroutine(ChangeLightIntensity(lightData.light, targetIntensity));
            }

        }

        //ʵ�����ڸ��¹�Դ��Э��
        private IEnumerator ChangeLightIntensity(Light light, float target)
        {
            while (!Mathf.Approximately(light.intensity, target))
            {
                float step = LightSpeed * 0.1f * Time.deltaTime;
                light.intensity = Mathf.MoveTowards(light.intensity, target, step);
                yield return null; // �ȴ���һ֡
            }
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

        private void CollectSceneLights()
        {
            lights.Clear();

            Light[] allLights = FindObjectsOfType<Light>();

            foreach (var l in allLights)
            {
                if (l == null) continue;
                if (l.type == LightType.Point || l.type == LightType.Spot)
                {
                    LightData data = new LightData
                    {
                        light = l,
                        nightIntensity = l.intensity // Ĭ�ϱ��浱ǰ����Ϊҹ������
                    };
                    lights.Add(data);
                }
            }

        }
    }

}

