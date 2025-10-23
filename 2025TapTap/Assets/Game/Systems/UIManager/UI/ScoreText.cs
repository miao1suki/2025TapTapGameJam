using DG.Tweening;
using TMPro;
using UnityEngine;

namespace ScoreSystem
{
    public class ScoreText : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private float duration;

        [SerializeField] private int value;   //当前显示的值
        [SerializeField] private int finalValue;     //最终值
        private Tweener tweener;    //它的动画

        public int Value
        {
            get => finalValue;
            set => DOAnimation(value);
        }

        /// <summary>设置到目标值，进行数字变动的动画</summary>
        private void DOAnimation(int target)
        {
            finalValue = target;
            tweener?.Kill();
            tweener = DOTween.To(() => value, TextSetter, Value, duration);
        }
        private void TextSetter(int value)
        {
            this.value = value;
            text.text = value.ToString();
        }
        public void Clear()
        {
            tweener?.Kill();
            TextSetter(0);
            finalValue = 0;
        }
    }
}