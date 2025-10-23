using DG.Tweening;
using TMPro;
using UnityEngine;

namespace ScoreSystem
{
    public class ScoreText : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private float duration;

        [SerializeField] private int value;   //��ǰ��ʾ��ֵ
        [SerializeField] private int finalValue;     //����ֵ
        private Tweener tweener;    //���Ķ���

        public int Value
        {
            get => finalValue;
            set => DOAnimation(value);
        }

        /// <summary>���õ�Ŀ��ֵ���������ֱ䶯�Ķ���</summary>
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