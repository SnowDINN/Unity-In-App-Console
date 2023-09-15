using TMPro;
using UnityEngine;

namespace Anonymous.Command
{
    public class ApplicationDebugCommandItem : MonoBehaviour
    {
        public TextMeshProUGUI UIText;
        private ApplicationDebugCommand command;

        public void Initialize(ApplicationDebugCommand command) => this.command = command;

        public void OnClicked()
        {
            command.UIInputCommand.text = UIText.text;
            command.UIInputCommand.ActivateInputField();
            command.UIInputCommand.MoveTextEnd(false);

            command.CommandSupporterObject.SetActive(false);
        }
    }
}