using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputSystem
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private InputActionAsset actionAsset;
        private InputActionMap UIMap;

        public static InputManager Instance { get; private set; }
        public static InputAction Navigate { get; private set; }
        public static InputAction Submit { get; private set; }

        private void Awake()
        {
            Instance = this;
            UIMap = actionAsset.FindActionMap("UI");
            Navigate = UIMap.FindAction("NavigateV");
            Submit = UIMap.FindAction("Submit");
        }
        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
            Navigate = null;
            Submit = null;
        }

    }
}