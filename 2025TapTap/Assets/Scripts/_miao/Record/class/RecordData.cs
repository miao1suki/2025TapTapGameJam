using UnityEngine;
namespace miao
{
    [CreateAssetMenu(fileName = "RecordData", menuName = "Game/Record Data")]
    public class RecordData : ScriptableObject
    {
        public string recordID;       // 唯一ID，比如 "Record_01"
        public string recordName;     // 唱片名
        public AudioClip musicClip;   // 对应音乐
        public bool isCollected;      // 是否已收集（编辑器中仅查看）

        [Tooltip("自定义异常脚本，继承自 RecordExceptionBase")]
        public RecordExceptionBase exceptionBehavior;
    }
}



