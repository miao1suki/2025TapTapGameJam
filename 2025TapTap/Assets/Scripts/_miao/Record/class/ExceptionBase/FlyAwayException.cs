using UnityEngine;
namespace miao
{
    [CreateAssetMenu(fileName = "FlyAwayException", menuName = "Game/Record Exception/Fly Away")]
    public class FlyAwayException : RecordExceptionBase
    {
        public override void OnCollect(GameObject collector, RecordData recordData)
        {
            var rb = Player.Instance.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddForce(Vector3.up * 5000f, ForceMode.Impulse);
        }
    }
}



