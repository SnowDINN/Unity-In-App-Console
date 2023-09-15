using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Anonymous
{
    public class ApplicationDebugCanvas : MonoBehaviour, IPointerClickHandler
    {
        [Header("Debug Status")]
        public List<GameObject> PrintObjects;

        private CanvasGroup canvasGroup;
        private Coroutine coroutine;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            coroutine = StartCoroutine(inputDetectorAsync());
        }

        private void OnDestroy()
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnPointerClickCancel();
        }

        public void OnPointerClickDebug()
        {
            canvasGroupActivator(true);
        }

        public void OnPointerClickCancel()
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
            
            PrintObjects.ForEach(print => { print.SetActive(!isActive); });
        }

        private IEnumerator inputDetectorAsync()
        {
            while (ApplicationDebug.isOn)
            {
#if UNITY_EDITOR
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (canvasGroup.alpha == 0.0f)
                        OnPointerClickDebug();
                    else
                        OnPointerClickCancel();
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