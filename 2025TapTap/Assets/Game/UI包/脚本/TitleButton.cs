using DG.Tweening;
using System;
using System.Collections;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class TitleButton : Button
{
    [SerializeField] private Vector2 shadowOffset;
    [SerializeField] private RectTransform shadow;
    [SerializeField, Range(0, 1)] private float downColorMultiplier;
    private Color startColor;
    private float startX;
    private static readonly float hoverScale = 1.05f;
    private static readonly float hoverAnimDuration = 0.35f;
    private static readonly float dehoverAnimDuration = 0.25f;

    private void Start()
    {
        startX = transform.localPosition.x;
        startColor = targetImage.color;
    }

    protected override void OnHover()
    {
        shadow.DOKill();
        shadow.DOLocalMove(shadowOffset, hoverAnimDuration);
        transform.DOKill();
        transform.DOScale(hoverScale, hoverAnimDuration);
        transform.DOLocalMoveX(startX - 100, hoverAnimDuration);
    }
    protected override void OnDehover()
    {
        shadow.DOKill();
        shadow.DOLocalMove(Vector2.zero, dehoverAnimDuration);
        transform.DOKill();
        transform.DOScale(1f, hoverAnimDuration);
        transform.DOLocalMoveX(startX, dehoverAnimDuration);
    }
    internal override void Submit()
    {
        targetImage.DOKill();
        targetImage.DOColor(startColor * downColorMultiplier, 0.1f).onComplete +=
            () => targetImage.DOColor(startColor, 0.1f);
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        targetImage.DOKill();
        targetImage.DOColor(startColor*downColorMultiplier, 0.1f);
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        targetImage.DOKill();
        targetImage.DOColor(startColor, 0.1f);
    }
}
