using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace miao
{
    [RequireComponent(typeof(Collider))]
    public class UpForce : MonoBehaviour
    {
        [Header("ʩ����")]
        public Vector3 force = new Vector3(0, 10f, 0); // Ĭ������

        private void Awake()
        {
            // ȷ������������
            Collider col = GetComponent<Collider>();
            col.isTrigger = true;
        }

        // ����������봥����
        private void OnTriggerStay(Collider other)
        {
            PhysicsBody body = other.GetComponent<PhysicsBody>();
            if (body != null)
            {
                body.ApplyForce(force);
            }
        }
    }
}



