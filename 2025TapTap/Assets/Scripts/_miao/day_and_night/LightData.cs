using UnityEngine;

namespace miao.day_and_night
{
    [System.Serializable]
    public class LightData
    {
        [Tooltip("��Դ����")]
        public string lightName;

        [Tooltip("��Դ����")]
        public string category;

        [Tooltip("ʵ������")]
        public Light light;

        [Tooltip("ҹ��ǿ��")]
        public float nightIntensity = 0.5f;
    }
}