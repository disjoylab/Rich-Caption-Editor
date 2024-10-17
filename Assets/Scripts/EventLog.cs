using System;
using System.Collections.Generic;
using UnityEngine;

public class EventLog : MonoBehaviour
{ 
    //The eventlog was created to doucment the available events that can be subscribed to
    //also useful for tracking down event issues 
    //Future changes could include individual actions on objects rather than groups or higher level classes.

    public List<string> Log = new List<string>();
    public bool LogToList;
    public bool LogToConsole;
     
    void Start()
    { 
        ProjectManager.ProjectChanged += OnProjectChanged; 
        
        MenuManager.CurrentMenuStateChanged += OnCurrentMenuStateChanged;
        MenuManager.MenuLayoutUpdated += OnMenuLayoutUpdated;

        StylesMenu.StyleChanged += OnStyleChanged; 

        VideoManager.VideoLoaded += OnVideoLoaded;
        VideoManager.CurrentTimeChanged += OnCurrentTimeChanged;
              
        ElementTool.ElementToolsChanged += OnElementToolsChanged;
        ElementManager.ElementsChanged += OnElementsChanged;

        FeatureManager.FeaturesChanged += OnFeaturesChanged;

        CueGroupsMenu.CueGroupsChanged += OnCueGroupsChanged;
       // CueUI.CurrentCueChanged += OnCurrentCueChanged;
        Cue.CueChanged += OnCueChanged;
        TimelineManager.TimeScaleChanged += OnTimeScaleChanged;
   

}

   

    private void OnDestroy()
    {
        ProjectManager.ProjectChanged -= OnProjectChanged;

        MenuManager.CurrentMenuStateChanged -= OnCurrentMenuStateChanged;
        MenuManager.MenuLayoutUpdated -= OnMenuLayoutUpdated;

        StylesMenu.StyleChanged -= OnStyleChanged;

        VideoManager.VideoLoaded -= OnVideoLoaded;
        VideoManager.CurrentTimeChanged -= OnCurrentTimeChanged;

        ElementTool.ElementToolsChanged -= OnElementToolsChanged;
        ElementManager.ElementsChanged -= OnElementsChanged;

        FeatureManager.FeaturesChanged -= OnFeaturesChanged;

        CueGroupsMenu.CueGroupsChanged -= OnCueGroupsChanged;
      //  CueUI.CurrentCueChanged -= OnCurrentCueChanged;
        Cue.CueChanged -= OnCueChanged;
        
        TimelineManager.TimeScaleChanged -= OnTimeScaleChanged;
    }



    // TimelineManager Events
    /// <summary>
    /// Called when the time scale on the timeline changes.
    /// </summary>
    private void OnTimeScaleChanged()
    {
        AddLog("Time Scale Changed");
    }

    // Project Events
    /// <summary>
    /// Called when the current project is changed.
    /// </summary>
    private void OnProjectChanged()
    {
        AddLog("ProjectChanged");
    }

    // Menu Events
    /// <summary>
    /// Called when the current menu state changes.
    /// </summary>
    private void OnCurrentMenuStateChanged(MenuStates _menuState)
    {
        AddLog("CurrentMenuStateChanged " + _menuState);
    }

    private void OnMenuLayoutUpdated()
    {
        AddLog("OnMenuLayoutUpdated ");
    }

    // Style Events
    /// <summary>
    /// Called when a style is changed in the Styles menu.
    /// </summary>
    private void OnStyleChanged()
    {
        AddLog("StyleChanged");
    }

    // Video Events
    /// <summary>
    /// Called when a new video is loaded.
    /// </summary>
    private void OnVideoLoaded()
    {
        AddLog("VideoLoaded");
    }

    /// <summary>
    /// Called when the current playback time of the video changes.
    /// </summary>
    private void OnCurrentTimeChanged(double _time)
    {
      //  AddLog("CurrentTimeChanged " + _time); //too verbose 
    }

    // Element Events
    /// <summary>
    /// Called when the element tools are changed.
    /// </summary>
    private void OnElementToolsChanged()
    {
        AddLog("ElementToolsChanged");
    }

    /// <summary>
    /// Called when elements are added, removed, or modified.
    /// </summary>
    private void OnElementsChanged()
    {
        AddLog("ElementsChanged");
    }

    // Feature Events
    /// <summary>
    /// Called when features are added, removed, or updated.
    /// </summary>
    private void OnFeaturesChanged()
    {
        AddLog("FeaturesChanged");
    }

    // Cue Events
    /// <summary>
    /// Called when cue groups are changed.
    /// </summary>
    private void OnCueGroupsChanged()
    {
        AddLog("CueGroupsChanged");
    }

    /// <summary>
    /// Called when the current cue is changed.
    /// </summary>
 //   private void OnCurrentCueChanged()
  //  {
   //     AddLog("CurrentCueChanged");
  //  }

    /// <summary>
    /// Called when a cue's TriggerChanged method is called
    /// </summary>
    private void OnCueChanged(Cue _cue)
    {
        AddLog("Cue Changed: " + _cue.Identifier);
    }

    /// <summary>
    /// Adds a log entry with a timestamp and event name.
    /// </summary>
    /// <param name="eventName">Name of the event to log.</param>
    private void AddLog(string eventName)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        if (LogToConsole) Debug.Log($"EventLog: [{timestamp}] {eventName}");
        if (LogToList) Log.Add($"[{timestamp}] {eventName}");
    }
}
