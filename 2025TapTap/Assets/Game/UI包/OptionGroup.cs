using UnityEngine;

namespace UI
{
    public class OptionGroup : MonoBehaviour
    {
        [SerializeField] private OptionButton defaultSelect;
        private OptionButton ob;


        private void Start()
        {
            defaultSelect.OnPointerClick(null);
        }

        public void Select(OptionButton other)
        {
            if (!other || ob == other) return;

            if(ob) ob.Deselect();
            ob = other;
        }
    }
}