using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

namespace Anonymous.Systems
{
    public class ApplicationDebugSystemsConsoleSystem : MonoBehaviour, IApplicationDebugSystems
    {
        [Header("Debug Information")]
        public TextMeshProUGUI UITextInformation;

        [Header("Debug Log")]
        public TextMeshProUGUI UITextLogBox;

        private readonly StringBuilder builder = new();
        private Coroutine coroutine;

        public void Setup()
        {
            UITextInformation.text = $"Build Version : {Application.version}\nBuild  Number : {Application.version}";

            Debug.unityLogger.logEnabled = ApplicationDebug.isActivate(DebugOptions.DEBUG);
            if (Debug.unityLogger.logEnabled)
                Application.logMessageReceived += LogMessageReceived;
        }

        public void Dispose()
        {
            if (Debug.unityLogger.logEnabled)
                Application.logMessageReceived -= LogMessageReceived;

            if (coroutine != null)
                StopCoroutine(coroutine);
        }

        private void LogMessageReceived(string condition, string stackTrace, LogType type)
        {
            coroutine = StartCoroutine(logMessageReceivedAsync(condition, stackTrace, type));
        }

        private IEnumerator logMessageReceivedAsync(string condition, string stackTrace, LogType type)
        {
            if (string.IsNullOrEmpty(condition))
                yield break;

            var message = condition.Replace("\n", "").Replace("\r", "");
            switch (type)
            {
                case LogType.Exception:
                    builder.AppendLine($"<color=red>{message}</color><br><color=#a52a2aff>{stackTrace}</color>" +
                                       $"{(message.Contains("<color=#ffa500ff>") ? "" : "<br>")}");
                    break;

                case LogType.Error:
                    builder.AppendLine($"<color=red>{message}</color>" +
                                       $"{(message.Contains("<color=#ffa500ff>") ? "" : "<br>")}");
                    break;

                case LogType.Log:
                    builder.AppendLine($"{message}" +
                                       $"{(message.Contains("<color=#ffa500ff>") ? "" : "<br>")}");
                    break;

                default:
                    builder.AppendLine($"<color=yellow>{message}</color>" +
                                       $"{(message.Contains("<color=#ffa500ff>") ? "" : "<br>")}");
                    break;
            }

            UITextLogBox.text = builder.ToString();

            var limitIndex = 50000;
            if (builder.Length > limitIndex)
            {
                builder.Clear();
                builder.Append(UITextLogBox.text[^limitIndex..]);
            }
        }
    }
}