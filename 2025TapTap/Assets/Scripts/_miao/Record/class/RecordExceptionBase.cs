using miao;
using UnityEngine;
namespace miao
{
    public abstract class RecordExceptionBase : ScriptableObject
    {
        public abstract void OnCollect(GameObject collector, RecordData recordData);
    }
}


