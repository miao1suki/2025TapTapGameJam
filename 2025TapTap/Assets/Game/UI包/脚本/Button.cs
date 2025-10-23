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
        [SerializeField] private UnityEvent clicked;
        [SerializeField] private UnityEvent hover;
        private bool isPointerEntered;

        internal BottonGroup group { get; set; }
        public UnityEvent Clicked => clicked;
        public bool pointerHover
        {
            get => isPointerEntered;
            internal set
            {
                if (isPointerEntered == value) return;

                isPointerEntered = value;
                if (value)
                {
                    hover.Invoke();
                    OnHover();
                    if (hoverSound) AudioManager.PlaySound(hoverSound, AudioSystem.AudioType.UIEffectSound);
                    if (group && group.current != this) group.current = this;
                }
                else OnDehover();
            }
        }


        protected virtual void OnDehover() { }
        protected virtual void OnHover() { }
        protected virtual void OnClick() { }
        internal virtual void Submit() => OnPointerClick(null);
        protected virtual void OnDisable() => pointerHover = false;
        public void OnPointerClick(PointerEventData eventData)
        {
            clicked.Invoke();
            OnClick();
            if (clickSound) AudioManager.PlaySound(clickSound, AudioSystem.AudioType.UIEffectSound);
        }
        public void OnPointerEnter(PointerEventData eventData) => pointerHover = true;
        public virtual void OnPointerDown(PointerEventData eventData) { }
        public virtual void OnPointerUp(PointerEventData eventData) { }
        public void OnPointerExit(PointerEventData eventData) => pointerHover = false;
    }
}