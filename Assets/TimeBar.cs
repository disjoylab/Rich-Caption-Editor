 
using UnityEngine;
using UnityEngine.EventSystems;

public class TimeBar : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{     
    RectTransform rectTransform;
    float initialBarPosition;
    float initialMousePosition;
    float currentMousePosition;
    bool isDragging;
         
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        VideoManager.CurrentTimeChanged += OnCurrentTimeChanged;
    }
    
    private void OnDestroy()
    {
        VideoManager.CurrentTimeChanged -= OnCurrentTimeChanged;
    }

    private void OnCurrentTimeChanged(double _currentTime)
    {
        SetPosition();
    }
    
    public void SetPosition()
    {
        if (!isDragging)
        {
            rectTransform.localPosition = new Vector2(TimelineManager.currentTimePixel, 0);
        }
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
           initialMousePosition = eventData.position.x;
        initialBarPosition = rectTransform.localPosition.x; 
        VideoManager.Instance.PauseVideo(); 
    }

    public void OnDrag(PointerEventData eventData)
    {
        currentMousePosition = eventData.position.x;    
        float mouseDelta = currentMousePosition - initialMousePosition;
        float position = initialBarPosition + mouseDelta;
        rectTransform.localPosition = new Vector2(position, 0);
        VideoManager.Instance.SetTime(position / TimelineManager.PixelsPerSecond());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }        
}