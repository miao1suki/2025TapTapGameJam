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
            public float nightIntensity = 0.3f; // 夜间亮度
        }


        [Header("渐变(Gradients)")]
        [Tooltip("雾颜色渐变")]
        [SerializeField] private Gradient fogGradient;//雾颜色渐变
        [Tooltip("环境光渐变")]
        [SerializeField] private Gradient ambientGradient;//环境光渐变
        [Tooltip("主光颜色渐变")]
        [SerializeField] private Gradient directionLightGradient;//主光颜色渐变
        [Tooltip("天空盒颜色渐变")]
        [SerializeField] private Gradient skyboxTintGradient;//天空盒颜色渐变

        [Header("环境资源(Enviromental Assets)")]
        [SerializeField] private Light directionalLight;
        [SerializeField] private Material skyboxMaterial;

        [Header("变量(Variables)")]
        [SerializeField] public float dayDurationInSeconds = 60f;//一天持续时间（秒）
        [SerializeField] private float rotationSpeed = 1f;

        [Header("光源自动开关控制")]
        [Tooltip("受控灯光列表")]
        [SerializeField] private List<LightData> lights = new List<LightData>();
        [Tooltip("缓慢开启光源")]
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
            //更新时间
            UpdateTime();
            UpdateDayNightCycle();

            //事件操作
            //////////////////////////////////////////////////////
            // 夜晚开始
            if (currentTime >= 0.25f && !nightLightsTriggered)
            {
                nightLightsTriggered = true;
                dayLightsTriggered = false;
                UpdateLight(); // 启动夜间光源
            }

            // 白天开始
            if (currentTime >= 0.75f && !dayLightsTriggered)
            {
                dayLightsTriggered = true;
                nightLightsTriggered = false;
                UpdateLight(); // 启动白天光源
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

                // 启动协程逐步改变光强
                StopCoroutine(ChangeLightIntensity(lightData.light, targetIntensity));
                StartCoroutine(ChangeLightIntensity(lightData.light, targetIntensity));
            }

        }

        //实际用于更新光源的协程
        private IEnumerator ChangeLightIntensity(Light light, float target)
        {
            while (!Mathf.Approximately(light.intensity, target))
            {
                float step = LightSpeed * 0.1f * Time.deltaTime;
                light.intensity = Mathf.MoveTowards(light.intensity, target, step);
                yield return null; // 等待下一帧
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
                        nightIntensity = l.intensity // 默认保存当前亮度为夜间亮度
                    };
                    lights.Add(data);
                }
            }

        }
    }

}

