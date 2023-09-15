using UnityEngine;
using UnityEngine.UI;

namespace Anonymous
{
    public class ApplicationDebugToggle : MonoBehaviour
    {
        private readonly Color offColor = Color.HSVToRGB(0, 75, 65);
        private readonly Color onColor = Color.HSVToRGB(120, 100, 50);

        private void Awake()
        {
            var toggle = GetComponent<Toggle>();
            if (toggle == null)
                return;
            
            toggle.onValueChanged.AddListener(evt =>
            {
                toggle.targetGraphic.color = evt ? onColor : offColor;
            });
        }
    }
}