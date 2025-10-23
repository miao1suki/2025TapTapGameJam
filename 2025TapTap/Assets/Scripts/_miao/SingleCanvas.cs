using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miao
{
    public class SingleCanvas : MonoBehaviour
    {
        public static SingleCanvas Ins { get; private set; }
        public Canvas canvas {  get; private set; }

        private void Awake()
        {
            Ins = this;
            canvas = GetComponent<Canvas>();
        }
    }
}
