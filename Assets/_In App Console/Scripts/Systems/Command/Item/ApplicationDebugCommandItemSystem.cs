using TMPro;
using UnityEngine;

namespace Anonymous.Systems
{
    public class ApplicationDebugCommandItemSystem : MonoBehaviour
    {
        [SerializeField] public TextMeshProUGUI UIText;
        private ApplicationDebugCommandSystem commandSystem;

        public void Setup(ApplicationDebugCommandSystem commandSystem) => this.commandSystem = commandSystem;

        public void OnClicked()
        {
            commandSystem.UIInputCommand.text = UIText.text;
            commandSystem.UIInputCommand.ActivateInputField();
            commandSystem.UIInputCommand.MoveTextEnd(false);

            ApplicationDebugCommandSystem.SupporterActive(false);
        }
    }
}