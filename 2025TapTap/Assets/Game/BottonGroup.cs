using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using InputSystem;
using System;

namespace UI
{
    public class BottonGroup : MonoBehaviour
    {
        [SerializeField] bool selectFirstOnStart;
        [SerializeField] bool loops;
        protected Button[] buttons { get; private set; }
        private int index;

        public Button current
        {
            get => buttons[index];  //���ܿ���δ���������
            internal set    //�����е�Button���ã���ָ����밴ťʱ��ʹNavigate������Hover����ȷȡ��
            {
                if(current) current.pointerHover = false;
                index = Array.IndexOf(buttons, value);
            }
        }

        private void Awake()
        {
            buttons = GetComponentsInChildren<Button>();
            buttons = Array.FindAll(buttons, b => b != null);
            foreach (Button button in buttons) button.group = this;
        }
        protected virtual void Start()
        {
            if (selectFirstOnStart) HoverTo(0);
        }

        private void OnEnable()
        {
            if (InputManager.Navigate != null)
                InputManager.Navigate.Enable();

            if (InputManager.Submit != null)
                InputManager.Submit.Enable();

            if (InputManager.Navigate != null)
                InputManager.Navigate.performed += Navigate;

            if (InputManager.Submit != null)
                InputManager.Submit.performed += Submit;
        }
        private void OnDisable()
        {
            if (InputManager.Navigate != null)
                InputManager.Navigate.Disable();

            if (InputManager.Submit != null)
                InputManager.Submit.Disable();

            if (InputManager.Navigate != null)
                InputManager.Navigate.performed -= Navigate;

            if (InputManager.Submit != null)
                InputManager.Submit.performed -= Submit;
        }

        private void Submit(InputAction.CallbackContext context)
        {
            UpdateFocusIndex();
            if (index < 0) return;

            current.Submit();
        }
        private void Navigate(InputAction.CallbackContext context)
        {
            float value = context.ReadValue<float>();
            if (value > 0) Next();
            else if (value < 0) Previous();
        }
        private void Next()
        {
            UpdateFocusIndex();
            if (index < 0) HoverTo(0);
            else if (index >= buttons.Length - 1)
            {
                if(loops) HoverTo(0);
                return;
            }
            else HoverTo(index + 1);
        }
        private void Previous()
        {
            UpdateFocusIndex();
            if (index < 0) HoverTo(0);
            else if (index == 0)
            {
                if (loops) HoverTo(buttons.Length - 1);
                return;
            }
            else HoverTo(index - 1);
        }
        private void HoverTo(int to)
        {
            if(index >= 0) current.pointerHover = false;
            index = to;
            current.pointerHover = true;
        }
        private void UpdateFocusIndex()
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i].pointerHover)
                {
                    index = i;
                    return;
                }
            }
            index = -1;
        }
    }
}