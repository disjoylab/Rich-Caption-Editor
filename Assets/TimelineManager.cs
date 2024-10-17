using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TimelineManager : MonoBehaviour
{
    public static TimelineManager Instance;

    public GameObject TimeLineCuePrefab;
    public List<GameObject> TimelineCueObjects;
    public RectTransform contentRectTransform;
    public RectTransform viewportRectTransform;
   
    //content area scaling variables
    double totalTime;
    float ContentWidth;
    float ContentHeight;
    public static float currentTimePixel;
  
    public Slider TimeScaleSlider;
    public Toggle FollowTimeToggle;

    public TextMeshProUGUI CurrentTime;
    public TextMeshProUGUI MinTime;
    public TextMeshProUGUI MaxTime;

    public static Action TimeScaleChanged;

    public Sprite PlaySprite;
    public Sprite PauseSprite;
    public Image PlayPauseImage;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        VideoManager.CurrentTimeChanged += OnCurrentTimeChanged;
        VideoManager.VideoLoaded += OnVideoLoaded;
        ProjectManager.ProjectChanged += OnProjectChanged;            
        CueGroupsMenu.CueGroupsChanged += OnCueGroupsChanged; 
    }
    private void OnDestroy()
    {
        VideoManager.CurrentTimeChanged -= OnCurrentTimeChanged;
        VideoManager.VideoLoaded -= OnVideoLoaded;
        ProjectManager.ProjectChanged -= OnProjectChanged;
        CueGroupsMenu.CueGroupsChanged -= OnCueGroupsChanged;
    }
         
    private void OnCueGroupsChanged()
    {
        CreateTimelineCues();
    }

    private void CreateTimelineCues()
    {
        if (TimelineCueObjects==null)
        {
            TimelineCueObjects = new List<GameObject>();
        }
        foreach (var TimelineCueObject in TimelineCueObjects)
        {
            Destroy(TimelineCueObject);
        }
        TimelineCueObjects.Clear(); 

         var cueGroups = ProjectManager.Instance.CurrentRCEProject.CueGroups;        
        
        for (int i = 0; i < cueGroups.Count; i++)
        { 
            CreateCueGroupCues(cueGroups[i], GetCueGroupHeight(i), GetCueGroupPosition(i),ProjectManager.GetCueGroupColor(i), IsCurrentCueGroup(i));           
        }
        SetSize();
    }

    private bool IsCurrentCueGroup(int index)
    {
        return index == ProjectManager.Instance.CurrentRCEProject.GetCurrentCueGroupIndex();
    }

    private int GetCueGroupHeight(int index)
    {
        return IsCurrentCueGroup(index) ? 36 : 13;
    }

    private int GetCueGroupPosition(int index)
    {
        int position = -6;
        for (int i = 0; i < index; i++)
        {
            position -= GetCueGroupHeight(i) + 2;
        }
        return position;
    }

    public void NewCue()
    {
        if (ProjectManager.Instance.CurrentRCEProject.CueGroups.Count == 0) return;
        int index = ProjectManager.Instance.CurrentRCEProject.GetCurrentCueGroupIndex();
        
        GameObject go = Instantiate(TimeLineCuePrefab, contentRectTransform);
        TimelineCue tlc = go.GetComponent<TimelineCue>();
        TimelineCueObjects.Add(go);
        CueGroup currentCueGruop = ProjectManager.Instance.CurrentRCEProject.GetCurrentCueGroup();
        
        Cue newCue = new Cue("New Cue");
        float leftTimePixel = -contentRectTransform.localPosition.x;
        float viewportWidth = viewportRectTransform.rect.width;
        float middleTime = (leftTimePixel + (viewportWidth / 2)) / PixelsPerSecond();

        newCue.StartTime = middleTime-1;
        newCue.EndTime = middleTime+1;
        tlc.NewCue = true; 
        tlc.Configure(currentCueGruop,newCue, GetCueGroupHeight(index), GetCueGroupPosition(index), ProjectManager.GetCueGroupColor(index),true);      
        newCue.TriggerChanged();//FORCES OTHER UNUSED NEW CUES TO DELETE THEMSELVES        
    }

    private void CreateCueGroupCues(CueGroup cueGroup, int height, int pos, Color c,bool active)
    {
        foreach (var cue in cueGroup.Cues)
        {
            GameObject go = Instantiate(TimeLineCuePrefab, contentRectTransform);
            TimelineCue tlc = go.GetComponent<TimelineCue>();
            tlc.Configure(cueGroup, cue, height,pos, c,active);
            TimelineCueObjects.Add(go);
        }
    }

    private void OnProjectChanged()
    {
        CreateTimelineCues();
        SetPlayPauseButton();
    }
       
    private void OnVideoLoaded()
    {
        SetSize(); 
    }

    private void OnCurrentTimeChanged(double _currentTime)
    {
        CurrentTime.text = Common.FloatToTimeString((float)_currentTime);
        SetTimeAndPosition();
        SetPlayPauseButton();
    }     
     
    public void SetSize()
    {
        if (VideoManager.Instance != null && VideoManager.Instance.videoPlayer != null)
        {
            totalTime = VideoManager.totalDuration;
            float newContentWidth = (float)(totalTime * PixelsPerSecond());
            float newContentHeight = -GetCueGroupPosition(ProjectManager.Instance.CurrentRCEProject.CueGroups.Count)+5;
            newContentHeight = MathF.Max(newContentHeight, 85);
            if (ContentWidth!=newContentWidth || ContentHeight != newContentHeight)
            {
                ContentHeight = newContentHeight;
                ContentWidth = newContentWidth;
                contentRectTransform.sizeDelta = new Vector2(ContentWidth, ContentHeight);
                TimeScaleChanged?.Invoke();
            }
        } 
    }

    public void SetTimeAndPosition()
    {
        currentTimePixel = (float)VideoManager.currentTime * PixelsPerSecond();
        float leftTimePixel = -contentRectTransform.localPosition.x;
        float viewportWidth = viewportRectTransform.rect.width;
        float rightTimePixel = leftTimePixel + viewportWidth;
        if (VideoManager.Instance != null && VideoManager.Instance.videoPlayer != null && FollowTimeToggle.isOn)
        {             
            if (currentTimePixel < leftTimePixel)
            {
                leftTimePixel = Mathf.Clamp( currentTimePixel - viewportWidth,0,ContentWidth-viewportWidth);
                rightTimePixel = leftTimePixel + viewportWidth;
                contentRectTransform.localPosition =  new Vector2(-leftTimePixel, 0);
            }
            if (currentTimePixel > rightTimePixel)
            {
                leftTimePixel = Mathf.Clamp(currentTimePixel, 0, ContentWidth - viewportWidth);
                rightTimePixel = leftTimePixel + viewportWidth;
                contentRectTransform.localPosition = new Vector2(-leftTimePixel, 0);
            }
        }
        MinTime.text = Common.FloatToTimeString((float)leftTimePixel / PixelsPerSecond());
        MaxTime.text = Common.FloatToTimeString((float)rightTimePixel / PixelsPerSecond());
    }
     
    public static float PixelsPerSecond()
    {
        if (Instance == null)
        { 
            return 50.0f; // Default fallback value
        }

        return Mathf.Pow(Instance.TimeScaleSlider.value/10, 2) * 50;
    }
    public void TogglePlay()
    { 
        if (VideoManager.Instance.IsPaused())
        {
            VideoManager.Instance.PlayVideo();
        }
        else
        {
            VideoManager.Instance.PauseVideo();
        }
        SetPlayPauseButton();
    }
    public void SetPlayPauseButton()
    {
        PlayPauseImage.sprite = VideoManager.Instance.IsPaused() ? PlaySprite: PauseSprite;
    
    }
}
