using Anonymous.Systems;
using System.Collections;
using UnityEngine;

namespace Anonymous
{
    public class ApplicationDebugSystemsController : MonoBehaviour, IApplicationDebugSystems
    {
        [Header("Print Debug")]
        public CanvasGroup profileGroup;

        private CanvasGroup canvasGroup;
        private Coroutine coroutine;

        public void Setup()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            coroutine = StartCoroutine(inputDetectorAsync());
            
            GroupController(true);
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
            
            GroupController(!isActive);
            if (ApplicationDebug.isActivate(DebugOptions.DEBUG_STATUS))
                ApplicationDebugProfilerSystem.Default.Activate(!isActive);
            else
                ApplicationDebugProfilerSystem.Default.Activate(false);
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
        
        private void GroupController(bool isActive)
        {
            profileGroup.alpha = isActive ? 1.0f : 0.0f;

#if UNITY_EDITOR
            profileGroup.interactable = isActive;
            profileGroup.blocksRaycasts = isActive;      
#endif
        }
    }
}