using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DotsClone
{
    /// <summary>
    /// Allows dots to manage incoming IO
    /// </summary>
    [RequireComponent(typeof(Dot))]
    public class DotTouchIO : MonoBehaviour, IPointerDownHandler,
                                             IPointerEnterHandler,
                                             IPointerUpHandler
    {
        static bool hasSelection;

        Dot dot;

        private void Awake()
        {
            dot = GetComponent<Dot>();
        }

        public delegate void OnSelectionStarted();
        public static event OnSelectionStarted SelectionStarted;
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!hasSelection)
            {
                hasSelection = true;
                //print("OnSelectionStarted");
                if (SelectionStarted != null)
                {
                    SelectionStarted();
                }
                OnPointerEnter(eventData);
            }
        }

        public delegate void OnDotSelected(Dot dot);
        public static event OnDotSelected DotSelected;
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (hasSelection)
            {
                //print("OnDotSelected " + dot.name);
                if (DotSelected != null)
                {
                    DotSelected(dot);
                }
            }
        }

        public delegate void OnSelectionEnded();
        public static event OnSelectionEnded SelectionEnded;
        public void OnPointerUp(PointerEventData eventData)
        {
            if (hasSelection)
            {
                hasSelection = false;
                //print("OnSelectionEnded");
                if (SelectionEnded != null)
                {
                    SelectionEnded();
                }
            }
        }
    }
}