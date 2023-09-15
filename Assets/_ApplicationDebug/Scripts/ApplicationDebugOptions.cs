using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

namespace Anonymous
{
    public class ApplicationDebugOptions : MonoBehaviour
    {
        public TextMeshProUGUI UITextInformation;

        [Header("Debug Log")]
        public GameObject LogObject;
        public TextMeshProUGUI UITextLogBox;

        [Header("Debug Status")]
        public GameObject StatusObject;

        private readonly StringBuilder builder = new();
        private Coroutine coroutine;

        private void Awake()
        {
            UITextInformation.text = $"Build Version : {Application.version}\nBuild  Number : {Application.version}";
            
            Debug.unityLogger.logEnabled = ApplicationDebug.isOn;
            if (Debug.unityLogger.logEnabled)
                Application.logMessageReceived += LogMessageReceived;

            LogObject.SetActive(false);
            StatusObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (Debug.unityLogger.logEnabled)
                Application.logMessageReceived -= LogMessageReceived;
            
            if (coroutine != null)
                StopCoroutine(coroutine);
        }

        public void DebugLogActivator()
        {
            LogObject.SetActive(!ApplicationDebug.isOn);
            if (!ApplicationDebug.isOn)
                Application.logMessageReceived += LogMessageReceived;
            else
                Application.logMessageReceived -= LogMessageReceived;

            UITextLogBox.text = string.Empty;
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
                    builder.AppendLine($"<color=red>{message}</color><br><color=#a52a2aff>{stackTrace}</color><br>");
                    break;

                case LogType.Error:
                    builder.AppendLine($"<color=red>{message}</color><br>");
                    break;

                case LogType.Log:
                    builder.AppendLine($"{message}<br>");
                    break;

                default:
                    builder.AppendLine($"<color=yellow>{message}</color><br>");
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