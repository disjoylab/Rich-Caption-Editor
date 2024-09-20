using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IDragHandler
{ 
    
   // private RectTransform rectTransform;

    private void Start()
    {
     //   rectTransform = GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 dragDirection = eventData.delta;
        float dragAmount = dragDirection.x + dragDirection.y;
        MenuManager.TargetVideoScale -= (dragAmount / 1000); 
    }
}
