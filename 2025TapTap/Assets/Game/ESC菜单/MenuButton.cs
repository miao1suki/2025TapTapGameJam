using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

namespace UI
{
    public class MenuButton : Button
    {
        [SerializeField] private TMPro.TMP_Text targetText;
        [SerializeField] private Color startColor;
        [SerializeField] private Color clickDownColor;
        [SerializeField, Range(0.5f, 1.5f)] private float clickDownScale = 0.95f;
        [SerializeField] private Color hoverColor;
        [SerializeField] private Color textDefaultColor;
        [SerializeField] private Color textHoverColor;
        [SerializeField, Range(0.05f, 1f)] private float animDuration;

        private void Start()
        {
            targetText.color = textDefaultColor;
            targetImage.color = startColor;
        }

        protected override void OnDehover()
        {
            float duration = animDuration * 0.5f;
            targetText.DOKill();
            targetText.DOColor(textDefaultColor, duration).SetUpdate(true);
            targetImage.DOKill();
            targetImage.DOColor(startColor, duration).SetUpdate(true);
            if(targetText.transform.localScale != Vector3.one)
            {
                targetText.transform.DOKill();
                targetText.transform.DOScale(Vector3.one, duration).SetUpdate(true);
            }
        }
        protected override void OnHover()
        {
            targetText.DOKill();
            targetText.DOColor(textHoverColor, animDuration).SetUpdate(true);
            targetImage.DOKill();
            targetImage.DOColor(hoverColor, animDuration).SetUpdate(true);
        }
        public override void OnPointerDown(PointerEventData eventData)
        {
            float duration = animDuration * 0.5f;
            targetImage.DOKill();
            targetImage.DOColor(clickDownColor, animDuration * 0.5f).SetUpdate(true);
            targetText.transform.DOKill();
            targetText.transform.DOScale(clickDownScale, duration).SetUpdate(true);
        }
        public override void OnPointerUp(PointerEventData eventData)
        {
            float duration = animDuration * 0.5f;
            if (!pointerHover) return;
            targetImage.DOKill();
            targetImage.DOColor(hoverColor, duration).SetUpdate(true);
            targetText.transform.DOKill();
            targetText.transform.DOScale(Vector3.one, duration).SetUpdate(true);
        }
    }
}