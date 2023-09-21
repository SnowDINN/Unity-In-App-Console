using Anonymous.Systems;
using System.Collections;
using UnityEngine;

namespace Anonymous
{
    public class ApplicationDebugSystemsController : MonoBehaviour, IApplicationDebugSystems
    {
        [Header("Print Debug")]
        public GameObject PrintDebugObject;

        private CanvasGroup canvasGroup;
        private Coroutine coroutine;

        public void Setup()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            coroutine = StartCoroutine(inputDetectorAsync());
        }

        public void Dispose()
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
        }

        public void OnPointerEnter()
        {
            canvasGroupActivator(true);
        }

        public void OnPointerExit()
        {
            canvasGroupActivator(false);
        }

        private void canvasGroupActivator(bool isActive)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = isActive ? 1.0f : 0.0f;
                canvasGroup.interactable = isActive;
                canvasGroup.blocksRaycasts = isActive;
            }
            
            PrintDebugObject.SetActive(!isActive);
            if (ApplicationDebug.isActivate(DebugOptions.DEBUG_STATUS))
                ApplicationDebugSystemsProfilerSystem.Default.Activate(!isActive);
            else
                ApplicationDebugSystemsProfilerSystem.Default.Activate(false);
        }

        private IEnumerator inputDetectorAsync()
        {
            while (ApplicationDebug.isActivate(DebugOptions.DEBUG))
            {
#if UNITY_EDITOR
                if (Input.GetKeyDown(KeyCode.BackQuote))
                {
                    if (canvasGroup.alpha == 0.0f)
                        OnPointerEnter();
                    else
                        OnPointerExit();
                }
#else
                if (Input.touchCount == 3)
                    OnPointerClickDebug();
#endif
                yield return null;
            }
        }
    }
}