using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
[Serializable]
public class RCEProject // avioiding getter/setter pattern since the data model would explode in size and complexity (and not show in inspector)
{
    public string ProjectName;    
    public string Details; 
    public string fileName;
     
    public List<StyleGroup> StyleGroups; 
    public List<CueGroup> CueGroups;

   [JsonProperty]
    private int currentCueGroupIndex;
    public int GetCurrentCueGroupIndex()
    {
        currentCueGroupIndex = Mathf.Clamp(currentCueGroupIndex, 0, CueGroups.Count-1);
        return currentCueGroupIndex;        
    }
    public void SetCurrentCueGroupIndex(int _index)
    {
        currentCueGroupIndex = Mathf.Clamp(_index, 0, CueGroups.Count-1); 
    }
    public StyleGroup GetCurrentStyleGroup()
    {
        if (StyleGroups==null || StyleGroups.Count==0)
        {
            SetDefaultStyles();
        }
        return StyleGroups[0];  //TODO **************************** implement int CurrentStyleGroup and add functionality to style UI tab
    }
    public CueGroup GetCurrentCueGroup()
    {
        if (CueGroups==null || CueGroups.Count==0)
        {
            return null;
        } 
        return CueGroups[currentCueGroupIndex];        
    }
    public RCEProject()
    {
        StyleGroups = new List<StyleGroup>();
        CueGroups = new List<CueGroup>();
    }

    public RCEProject(string _defaultCueGroup)
    {        
        CueGroups = new List<CueGroup>();
        CueGroup SPEECHGROUP = new CueGroup(_defaultCueGroup);
        CueGroups.Add(SPEECHGROUP);
        SetDefaultStyles();
    }
    public void SetDefaultStyles()
    {
        StyleGroups = new List<StyleGroup>();
        StyleGroup styleGroup = new StyleGroup();
        styleGroup.SetDefaultStyles();
        StyleGroups.Add(styleGroup);        
    }     
    public string GetFileName(string _ext)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            fileName = ProjectName;
        }
        return fileName + _ext;
    }
}