using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectCurrentTime : MonoBehaviour,IPointerDownHandler,IDragHandler
{
    private RectTransform rectTransform;

    private float lastClickTime = 0f;
    private float doubleClickThreshold = 0.3f;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>(); 
    } 
      
    public void OnPointerDown(PointerEventData eventData)
    {
        float timeSinceLastClick = Time.time - lastClickTime;
        if (timeSinceLastClick <= doubleClickThreshold)
        { 
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, null, out localPoint);             
            float selectedTimePixel = localPoint.x;
            VideoManager.Instance.SetTime(selectedTimePixel / TimelineManager.PixelsPerSecond());
        }
        lastClickTime = Time.time;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //just consuming the event so we cant drag the time line box in all directions, thats what the scroll bars are for 
    }
}