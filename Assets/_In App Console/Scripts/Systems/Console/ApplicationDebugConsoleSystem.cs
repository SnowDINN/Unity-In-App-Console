using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

namespace Anonymous.Systems
{
	public class ApplicationDebugConsoleSystem : MonoBehaviour, IApplicationDebugSystems
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

			Debug.unityLogger.logEnabled = ApplicationDebug.isActivate(ApplicationDebug.PPK_DEBUG);
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
			coroutine = StartCoroutine(LogMessageReceivedAsync(condition, stackTrace, type));
		}

		private IEnumerator LogMessageReceivedAsync(string condition, string stackTrace, LogType type)
		{
			if (string.IsNullOrEmpty(condition))
				yield break;

			var message = condition.Replace("\n", "<br>").Replace("\r", "");
			switch (type)
			{
				case LogType.Exception:
					builder.AppendLine(
						$"[{DateTime.Now:HH:mm:ss}]<indent=5em><color=red>{message}</color><br><color=#a52a2aff>{stackTrace}</color></indent>");
					break;

				case LogType.Error:
					builder.AppendLine($"[{DateTime.Now:HH:mm:ss}]<indent=5em><color=red>{message}</color></indent>");
					break;

				case LogType.Log:
					builder.AppendLine($"[{DateTime.Now:HH:mm:ss}]<indent=5em>{message}</indent>");
					break;
			}

			UITextLogBox.text = builder.ToString();

			const int limitIndex = 50000;
			if (builder.Length <= limitIndex)
				yield break;
			
			builder.Clear();
			builder.Append(UITextLogBox.text[^limitIndex..]);
		}
	}
}