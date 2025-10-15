using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace miao
{
    public class PhysicsActivator : MonoBehaviour
    {
        void Update()
        {
            this.transform.position = Player.Instance.transform.position;
        }
        private void OnTriggerEnter(Collider other)
        {
            var body = other.GetComponent<PhysicsBody>();
            if (body != null)
            {
                body.EnableSimulation(true);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            var body = other.GetComponent<PhysicsBody>();
            if (body != null && !body.isActive)
            {
                body.EnableSimulation(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var body = other.GetComponent<PhysicsBody>();
            if (body != null)
            {
                body.EnableSimulation(false);
            }
        }
    }

}


