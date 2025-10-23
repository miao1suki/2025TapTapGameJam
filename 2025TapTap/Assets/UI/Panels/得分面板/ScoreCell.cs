using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UI
{
    public class ScoreCell : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private Color color;
        [SerializeField] private float scale;
        [SerializeField] private float enterDuration;
        [SerializeField] private float delay;
        [SerializeField] private float leaveDuration;

        internal string Text
        {
            get => text.text;
            set => text.text = value;
        }
        internal void Init()
        {
            text.color = color;
            transform.localPosition = Vector2.zero;
            transform.localScale = Vector3.one * scale;
        }
        internal void OnEnable() => DOAnimation();
        private void OnDisable() => Kill();
        public void DOAnimation()
        {
            Init();
            Kill();
            transform.DOScale(1, enterDuration).OnComplete(() =>
            {
                transform.DOLocalMoveY(10, leaveDuration).SetDelay(delay);
                text.DOColor(color * new Vector4(1, 1, 1, 0), leaveDuration).SetDelay(delay);
            });
        }
        private void Kill()
        {
            transform.DOKill();
            text.DOKill();
        }
    }

}