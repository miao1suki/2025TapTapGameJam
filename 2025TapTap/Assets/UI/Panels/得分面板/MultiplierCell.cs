using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI
{
    public class MultiplierCell : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        public List<Color> scoreLevel = new();
        private int multiplier = 0;
        [SerializeField] private int maxMultiplier = 10;

        //µÃ·Ö±¶ÂÊ
        public int Multiplier
        {
            get => multiplier;
            set
            {
                if (value == multiplier) return;

                if(value <= 1)
                {
                    multiplier = value;
                    Text = "";
                }
                else
                {
                    if (value > maxMultiplier) return;
                    multiplier = value;
                    Text = "¡Á" + value;
                    text.fontSize = 30 + value * 2.5f;
                    if(value - 1 < scoreLevel.Count)
                    {
                        text.color = scoreLevel[value - 1];
                    }
                }
            }
        }

        private void Start() => Init();

        internal string Text
        {
            get => text.text;
            set => text.text = value;
        }
        internal void Init()
        {
            text.text = "";
            if (scoreLevel.Count > 0) text.color = scoreLevel[0];
        }
    }
}

