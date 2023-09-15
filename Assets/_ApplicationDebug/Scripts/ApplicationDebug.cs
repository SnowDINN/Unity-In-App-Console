using UnityEngine;

namespace Anonymous
{
    public class ApplicationDebug
    {
        public const string PPS_Debug = "DEBUG_ACTIVATE";
        public const string PPS_DebugLog = "DEBUG_LOG";
        public const string PPS_DebugStatus = "DEBUG_STATUS";

        public static bool isOn = true;

        private static GameObject go;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void RuntimeInitializeLoad()
        {
            Setup();
        }
        
        public static void Setup()
        {
            if (go != null)
                return;

            var resource = Resources.Load<GameObject>("Debug");
            go = Object.Instantiate(resource);
            Object.DontDestroyOnLoad(go);
            PlayerPrefs.SetInt(PPS_Debug, 0);
        }

        public static void Dispose()
        {
            if (go == null)
                return;

            Object.Destroy(go);
            PlayerPrefs.DeleteKey(PPS_Debug);
        }
    }
}