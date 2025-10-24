using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class RankItem : MonoBehaviour
    {
        [SerializeField] private Image bg;
        [SerializeField] private Color defaultColor;
        [SerializeField] private Color playerColor;
        [SerializeField] private TMP_Text rankText;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text scoreText;

        internal void SetData(int rank, string name, int score)
        {
            rankText.text = (rank + 1).ToString();
            nameText.text = name;
            if(name == "Íæ¼Ò") bg.color = playerColor;
            else bg.color = defaultColor;
            scoreText.text = score.ToString();
        }
    }
}