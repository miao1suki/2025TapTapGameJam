using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace miao
{
    [RequireComponent(typeof(Collider))]
    public class UpForce : MonoBehaviour
    {
        [Header("施加力")]
        public Vector3 force = new Vector3(0, 10f, 0); // 默认向上

        private void Awake()
        {
            // 确保触发器开启
            Collider col = GetComponent<Collider>();
            col.isTrigger = true;
        }

        // 当有物体进入触发器
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



