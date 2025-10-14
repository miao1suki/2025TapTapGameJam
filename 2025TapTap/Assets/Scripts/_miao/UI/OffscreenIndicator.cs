using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace miao
{
    public class OffscreenIndicator : MonoBehaviour
    {
        public static OffscreenIndicator Instance;
        private void Awake()
        {
            Instance = this;
        }
    }
}



