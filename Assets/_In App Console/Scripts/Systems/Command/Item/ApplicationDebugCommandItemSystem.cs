using TMPro;
using UnityEngine;

namespace Anonymous.Systems
{
    public class ApplicationDebugCommandItemSystem : MonoBehaviour
    {
        [SerializeField] public TextMeshProUGUI UIText;
        private ApplicationDebugSystemsCommandSystem systemsCommandSystem;

        public void Setup(ApplicationDebugSystemsCommandSystem systemsCommandSystem) => this.systemsCommandSystem = systemsCommandSystem;

        public void OnClicked()
        {
            systemsCommandSystem.UIInputCommand.text = UIText.text;
            systemsCommandSystem.UIInputCommand.ActivateInputField();
            systemsCommandSystem.UIInputCommand.MoveTextEnd(false);

            systemsCommandSystem.CommandSupporterObject.SetActive(false);
        }
    }
}