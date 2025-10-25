using UnityEngine;
using DG.Tweening;

namespace UI
{
    public class MenuButton : Button
    {
        [SerializeField] private TMPro.TMP_Text targetText;
        [SerializeField] private Color hoverColor;
        [SerializeField] private Color textHoverColor;
        [SerializeField, Range(0.05f, 1f)] private float animDuration;

        protected override void OnDehover()
        {
            float duration = animDuration * 0.5f;
            targetText.DOKill();
            targetText.DOColor(new(1, 1, 1, 0.9f), duration);
            targetImage.DOKill();
            targetImage.DOColor(new Color(1,1,1,0), duration);
        }
        protected override void OnHover()
        {
            targetText.DOKill();
            targetText.DOColor(textHoverColor, animDuration);
            targetImage.DOKill();
            targetImage.DOColor(hoverColor, animDuration);
        }
    }
}