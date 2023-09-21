using Anonymous.Systems;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

namespace Anonymous
{
    public enum DebugOptions
    {
        DEBUG,
        DEBUG_STATUS,
    }
    
    public class ApplicationDebug
    {
        private static GameObject go;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void RuntimeInitializeLoad()
        {
            StartedApplicationDebug();
            
            ApplicationDebugSystemsCommandSystem.AddCommand("status", command =>
            {
                Debug.Log($"Status : {Profiler.logFile}");
            });
            
            ApplicationDebugSystemsCommandSystem.AddCommand("profiler", command =>
            {
                var isOn = switchActivate(DebugOptions.DEBUG_STATUS);
                Debug.Log($"Profiler : " + $"{(isOn ? "on" : "off")}");
            });
        }
        
        public static void StartedApplicationDebug()
        {
            if (go != null)
                return;
            
            go = Object.Instantiate(Resources.Load<GameObject>("Debug"));
            Object.DontDestroyOnLoad(go);
            onActivate(DebugOptions.DEBUG);

            var components = go.GetComponentsInChildren<IApplicationDebugSystems>(true).ToArray();
            foreach (var component in components)
                component.Setup();
        }

        public static void EndedApplicationDebug()
        {
            if (go == null)
                return;

            Object.Destroy(go);
            offActivate(DebugOptions.DEBUG);
            
            var components = go.GetComponentsInChildren<IApplicationDebugSystems>(true).ToArray();
            foreach (var component in components)
                component.Dispose();
        }

        public static void onActivate(DebugOptions type)
        {
            PlayerPrefs.SetInt($"{type}", 0);
        }
        
        public static void offActivate(DebugOptions type)
        {
            PlayerPrefs.DeleteKey($"{type}");
        }

        public static bool isActivate(DebugOptions type)
        {
            return PlayerPrefs.HasKey($"{type}");
        }
        
        public static bool switchActivate(DebugOptions type)
        {
            if (PlayerPrefs.HasKey($"{type}"))
                PlayerPrefs.DeleteKey($"{type}");
            else
                PlayerPrefs.SetInt($"{type}", 0);
            
            return PlayerPrefs.HasKey($"{type}");
        }
    }
}