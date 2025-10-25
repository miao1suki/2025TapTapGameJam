using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using AudioSystem;
using UnityEngine.Events;

namespace UI
{
    public class Button : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private AudioClip clickSound;
        [SerializeField] private AudioClip hoverSound;
        [SerializeField] protected Image targetImage;
        //[SerializeField] private UnityEvent clicked;

        private bool isPointerEntered = false;
        private bool pointerHover
        {
            get => isPointerEntered;
            set
            {
                if (isPointerEntered == value) return;

                isPointerEntered = value;
                if (value)
                {
                    OnHover();
                    if(hoverSound)
                        AudioManager.PlaySound(hoverSound, AudioSystem.AudioType.UIEffectSound);
                }
                else OnDehover();
            }
        }

        protected virtual void OnDehover() { }
        protected virtual void OnHover() { }
        protected virtual void OnClick() { }

        public void OnPointerClick(PointerEventData eventData)
        {
            //clicked.Invoke();
            OnClick();
            if(clickSound)
                AudioManager.PlaySound(clickSound, AudioSystem.AudioType.UIEffectSound);
        }
        public void OnPointerEnter(PointerEventData eventData) => pointerHover = true;
        public virtual void OnPointerDown(PointerEventData eventData) { }
        public virtual void OnPointerUp(PointerEventData eventData) { }
        public void OnPointerExit(PointerEventData eventData) => pointerHover = false;
    }
}