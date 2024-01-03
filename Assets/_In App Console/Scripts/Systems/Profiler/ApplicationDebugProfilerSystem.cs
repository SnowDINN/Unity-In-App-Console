using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Profiling;

namespace Anonymous.Systems
{
    public class ApplicationDebugProfilerSystem : MonoBehaviour, IApplicationDebugSystems
    {
        public static ApplicationDebugProfilerSystem Default;
        
        [Header("Profiler GameObject")]
        [SerializeField] private GameObject ProfilerObject;

        [Header("Print Profiler Text")]
        [SerializeField] private TextMeshProUGUI uiText;

        private Coroutine coroutine;
        private float time;
        
        public void Setup()
        {
            Default = this;
            Activate(ApplicationDebug.isActivate(DebugOptions.DEBUG_STATUS));
            
            InvokeRepeating(nameof(refreshStatus), 0, 0.1f);
            coroutine = StartCoroutine(deltaTimeAsync());
        }

        public void Dispose()
        {
            CancelInvoke(nameof(refreshStatus));
            if (coroutine != null)
                StopCoroutine(coroutine);
        }

        private IEnumerator deltaTimeAsync()
        {
            while (ApplicationDebug.isActivate(DebugOptions.DEBUG))
            {
                time += (Time.deltaTime - time) * 0.1f;
                yield return null;
            }
        }

        private void refreshStatus()
        {
            var latency = time * 1000.0f;
            var fps = 1.0f / time;

            uiText.text = $"FPS : {fps:N0} <size=15>[{latency:N1} ms]</size>";
            uiText.text +=
                $"\nRAM : {Profiler.GetTotalReservedMemoryLong() / 1048576:N0}MB <size=15>[{Math.Round((double)Profiler.GetTotalAllocatedMemoryLong() * 100 / Profiler.GetTotalReservedMemoryLong(), 1):N1}%]</size>";
        }

        public void Activate(bool isActivate)
        {
            ProfilerObject.SetActive(isActivate);
        }
    }
}