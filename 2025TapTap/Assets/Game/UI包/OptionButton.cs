using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class OptionButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image targetImage;
        [SerializeField] private TMP_Text text;
        [SerializeField] private Color startColor;
        [SerializeField] private Color selectedColor;
        [SerializeField] private Color startTextColor;
        [SerializeField] private Color selectedTextColor;
        [SerializeField] private UnityEvent onSelect;
        [SerializeField] private UnityEvent onDeselect;

        public void OnPointerClick(PointerEventData eventData) => Select();

        public void Select()
        {
            targetImage.color = selectedColor;
            text.color = selectedTextColor;
            onSelect?.Invoke();
        }
        public void Deselect()
        {
            targetImage.color = startColor;
            text.color = startTextColor;
            onDeselect?.Invoke();
        }
    }
}