using UnityEngine;
namespace miao
{
    [CreateAssetMenu(fileName = "RecordData", menuName = "Game/Record Data")]
    public class RecordData : ScriptableObject
    {
        public string recordID;       // ΨһID������ "Record_01"
        public string recordName;     // ��Ƭ��
        public AudioClip musicClip;   // ��Ӧ����
        public bool isCollected;      // �Ƿ����ռ����༭���н��鿴��

        [Tooltip("�Զ����쳣�ű����̳��� RecordExceptionBase")]
        public RecordExceptionBase exceptionBehavior;
    }
}



