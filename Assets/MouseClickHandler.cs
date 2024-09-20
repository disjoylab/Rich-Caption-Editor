using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

public class MouseClickHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // UnityEvent for mouse down and mouse up
    public UnityEvent onMouseDown;
    public Action onMouseUp;

    // IPointerDownHandler implementation
    public void OnPointerDown(PointerEventData eventData)
    {
        onMouseDown?.Invoke();
    }

    // IPointerUpHandler implementation
    public void OnPointerUp(PointerEventData eventData)
    {
        onMouseUp?.Invoke();
    }
}
