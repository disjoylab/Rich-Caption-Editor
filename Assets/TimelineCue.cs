using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TimelineCue : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    CueGroup myCueGroup;
    public Color myColor;
    public Cue myCue;
    public Image backgroundImage;
    public GameObject Highlight;
    public TMPro.TMP_Text IdText;
    
    float Height;
    float Pos;

    enum DragRegion { None, Left, Middle, Right }
    DragRegion currentRegion = DragRegion.None;

    RectTransform rectTransform;
    Vector2 initialMousePosition;
    float initialStartTime;
    float initialEndTime;

    float LeftRigthDragRegion = 20f; 
    public bool Active;
    public bool MoveCue;
    public bool Delete;
    public bool NewCue;

    private float lastClickTime = 0f;
    private float doubleClickThreshold = 0.3f;
    public void Configure(CueGroup _cueGroup, Cue _cue, float _height, float _pos, Color _color, bool _active)
    {
        rectTransform = GetComponent<RectTransform>();
        VideoManager.CurrentTimeChanged += OnCurrentTimeChanged;
        Cue.CueChanged += OnCueChanged;
        TimelineManager.TimeScaleChanged += OnTimeScaleChanged;
        myCueGroup = _cueGroup;
        myCue = _cue;
        Height = _height;
        Pos = _pos;
        Active = _active;
        myColor = _color;
        DisplayCue();
        transform.SetAsLastSibling();
    } 
    
    private void OnDestroy()
    {
        VideoManager.CurrentTimeChanged -= OnCurrentTimeChanged;
        Cue.CueChanged -= OnCueChanged;
        TimelineManager.TimeScaleChanged -= OnTimeScaleChanged;
    }

    private void OnTimeScaleChanged()
    {
        DisplayCue();
    }

    private void OnCurrentTimeChanged(double _currentTime)
    {
        DisplayCue();
    }

    private void OnCueChanged(Cue _cue)
    {
        if (myCue == _cue)
        {
            if (myCue.Deleted)
            {
                Destroy(gameObject);
            }
            DisplayCue();
        }
        else
        {
            if (NewCue)
            {
                Destroy(gameObject);//has not registered with a cuegroup yet, ok to just destroy
            }
        }
    }

    public void DisplayCue()
    {
        bool CurrentCue = myCue.StartTime < VideoManager.currentTime && myCue.EndTime > VideoManager.currentTime;
        Highlight.SetActive(CurrentCue&&Active&&!NewCue);
        IdText.text = (myCue == null) ? "" : myCue.Identifier;
        float xPos = (myCue == null)?0: myCue.StartTime * TimelineManager.PixelsPerSecond();
        float width = (myCue == null)?0: (myCue.EndTime - myCue.StartTime) * TimelineManager.PixelsPerSecond();
        rectTransform.localPosition = new Vector2(xPos, Pos);
        rectTransform.sizeDelta = new Vector2(width, Height);
        backgroundImage.color = myColor;
        if (NewCue)
        {
            backgroundImage.color = Color.gray;
        }
        if (MoveCue)
        {
            if (Delete)
            {
                backgroundImage.color = Color.gray;
                IdText.text = "Delete";
            }
            else
            {
                backgroundImage.color = myCueGroup.IsValidSpot(myCue) ? Color.green : Color.red;
            }
        }        
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!Active) return;
        float timeSinceLastClick = Time.time - lastClickTime;
        if (timeSinceLastClick <= doubleClickThreshold)
        { 
            VideoManager.Instance.SetTime(myCue.StartTime+.1f);
            return;
        }
        lastClickTime = Time.time;
        CaptureInitialData(eventData.position); 

        Vector2 localMousePos = WorldToLocal(eventData.position);
        if (localMousePos.x < LeftRigthDragRegion)
        {
            currentRegion = DragRegion.Left;
        }
        else if (localMousePos.x > rectTransform.rect.width - LeftRigthDragRegion)
        {
            currentRegion = DragRegion.Right;
        }
        else
        {
            currentRegion = DragRegion.Middle;
        }
    }
    internal void CaptureInitialData(Vector3 mousePosition)
    {
        MoveCue = false;
        Delete = false;
        initialMousePosition = mousePosition;
        initialStartTime = myCue.StartTime;
        initialEndTime = myCue.EndTime;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (!Active) return;
        Vector2 mouseDelta = eventData.position - initialMousePosition;

        switch (currentRegion) 
        {
            case DragRegion.Left:
                MoveCue = true;
                myCue.StartTime = initialStartTime + (mouseDelta.x / TimelineManager.PixelsPerSecond());
                break;
            case DragRegion.Middle:

                MoveCue = true;
                Delete = GetMouseOver("Trash");
                transform.SetAsLastSibling();
                myCue.StartTime = initialStartTime + (mouseDelta.x / TimelineManager.PixelsPerSecond());
                myCue.EndTime = initialEndTime + (mouseDelta.x / TimelineManager.PixelsPerSecond());              
                break;
            case DragRegion.Right:
                MoveCue = true;
                myCue.EndTime = initialEndTime + (mouseDelta.x / TimelineManager.PixelsPerSecond());
                break;
        }
        DisplayCue();
    }   

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!Active) return;
        currentRegion = DragRegion.None;
        if (MoveCue)
        {
            ResolveMoveCue();           
        }
        Delete = false;
        MoveCue = false;
        DisplayCue();
    }

    private void ResolveMoveCue()
    {
        if (NewCue)
        {
            NewCue = !myCueGroup.IsValidSpot(myCue);//setting it after moving it is too late, it can delete itself based on the cue's change trigger event 
        }
        if (Delete)
        {
            myCueGroup.DeleteCue(myCue); 
        }
        else if (!myCueGroup.MoveCue(myCue))
        {
            myCue.StartTime = initialStartTime;
            myCue.EndTime = initialEndTime;
        }               
    }

    private Vector2 WorldToLocal(Vector2 worldPosition)
    {
        Vector2 localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, worldPosition, null, out localPosition);
        return localPosition;
    }

    private bool GetMouseOver(string ObjectName)
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
            foreach (RaycastResult result in results)
            {
                if (result.gameObject.name==ObjectName)
                {
                    return true;
                }
            }
        }
        return false;
    }    
}
