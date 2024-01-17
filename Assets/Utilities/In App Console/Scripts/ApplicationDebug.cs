using System.Collections.Generic;
using System.Linq;
using Anonymous.Systems;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Anonymous
{
	public class ApplicationDebug
	{
		private const string ERROR_COMMAND = "<color=red>The command you entered does not exist.</color>";
		public const string PPK_PROFILER = "APPLICATION_PROFILER_ACTIVATOR";
		public const string PPK_DEBUG = "APPLICATION_DEBUG_ACTIVATOR";

		private static Canvas canvas;
		private static GameObject go;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		public static void RuntimeInitializeLoad()
		{
			StartedApplicationDebug();

			ApplicationDebugCommandSystem.AddCommand("Profiler")
				.AddComment("It shows the performance status of your device.")
				.AddParameters(new List<string> { "on", "off" })
				.AddParameters(new List<string> { "active", "unactive" })
				.AddExecute(command =>
				{
					var split = command.Split(' ');
					switch (split.Length)
					{
						case 1:
						{
							var isOn = SwitchActivate();

							Debug.Log("Profiler : " + $"{(isOn ? "on" : "off")}");
							break;
						}
						default:
						{
							Debug.Log(ERROR_COMMAND);
							break;
						}
					}
				});
		}

		public static void StartedApplicationDebug()
		{
			if (go != null)
				return;

			go = Object.Instantiate(Resources.Load<GameObject>("Debug"));
			go.name = "DEBUGGER";
			SetWorldCamera();

			Object.DontDestroyOnLoad(go);
			OnActivate();

			var components = go.GetComponentsInChildren<IApplicationDebugSystems>(true).ToArray();
			foreach (var component in components)
				component.Setup();
			
			SceneManager.activeSceneChanged += OnChangeScene;
		}

		public static void EndedApplicationDebug()
		{
			if (go == null)
				return;

			Object.Destroy(go);
			OffActivate();

			var components = go.GetComponentsInChildren<IApplicationDebugSystems>(true).ToArray();
			foreach (var component in components)
				component.Dispose();
			
			SceneManager.activeSceneChanged -= OnChangeScene;
		}

		public static bool isActivate(string key)
		{
			return PlayerPrefs.HasKey(key);
		}

		public static void OnActivate()
		{
			PlayerPrefs.SetInt(PPK_DEBUG, 0);
		}

		public static void OffActivate()
		{
			PlayerPrefs.DeleteKey(PPK_DEBUG);
		}

		public static bool SwitchActivate()
		{
			if (PlayerPrefs.HasKey(PPK_PROFILER))
				PlayerPrefs.DeleteKey(PPK_PROFILER);
			else
				PlayerPrefs.SetInt(PPK_PROFILER, 0);

			return PlayerPrefs.HasKey(PPK_PROFILER);
		}
		
		private static void OnChangeScene(Scene arg0, Scene arg1)
		{
			SetWorldCamera();
		}

		private static void SetWorldCamera()
		{
			if (canvas == null)
				canvas = go.GetComponent<Canvas>();
			canvas.worldCamera = Camera.main;
		}
	}
}