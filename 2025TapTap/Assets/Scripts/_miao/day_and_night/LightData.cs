using UnityEngine;

namespace miao.day_and_night
{
    [System.Serializable]
    public class LightData
    {
        [Tooltip("光源名称")]
        public string lightName;

        [Tooltip("光源分类")]
        public string category;

        [Tooltip("实际引用")]
        public Light light;

        [Tooltip("夜光强度")]
        public float nightIntensity = 0.5f;
    }
}