using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Profiling;

namespace Anonymous.Status
{
    public class ApplicationDebugFrame : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI uiText;

        private Coroutine coroutine;
        private float time;

        private void OnEnable()
        {
            InvokeRepeating(nameof(refreshStatus), 0, 0.1f);
            coroutine = StartCoroutine(deltaTimeAsync());
        }

        private void OnDisable()
        {
            CancelInvoke(nameof(refreshStatus));
            if (coroutine != null)
                StopCoroutine(coroutine);
        }

        private IEnumerator deltaTimeAsync()
        {
            while (ApplicationDebug.isOn)
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
    }
}