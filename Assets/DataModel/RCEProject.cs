using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;

[Serializable]
public class RCEProject // avioiding getter/setter pattern since the data model would explode in size and complexity (and not show in inspector)
{
    public string ProjectName;
    public string Details;
    public string FileName; 
    public string VideoFile; 

    public List<StyleGroup> StyleGroups;
    public List<CueGroup> CueGroups;

    [JsonProperty]
    private int currentCueGroupIndex;
    [JsonProperty]
    private int currentStyleGroupIndex;

    public int GetCurrentCueGroupIndex()
    {
        currentCueGroupIndex = Mathf.Clamp(currentCueGroupIndex, 0, CueGroups.Count - 1);
        return currentCueGroupIndex;
    }

    public int GetCurrentStyleGroupIndex()
    {
        currentStyleGroupIndex = Mathf.Clamp(currentStyleGroupIndex, 0, StyleGroups.Count - 1);
        return currentStyleGroupIndex;
    }

    public void SetCurrentCueGroup(CueGroup _cueGroup)
    {
        if (CueGroups == null || CueGroups.Count == 0)
        {
            return;
        }
        CueGroups[GetCurrentCueGroupIndex()] = new CueGroup(_cueGroup);
    }

    public void SetCurrentCueGroupIndex(int _index)
    {
        currentCueGroupIndex = Mathf.Clamp(_index, 0, CueGroups.Count - 1);
    }

    public StyleGroup GetCurrentStyleGroup()
    {
        if (StyleGroups == null || StyleGroups.Count == 0)
        {
            CreateNewStyleGroup();
        }
        StyleGroups[GetCurrentStyleGroupIndex()].SetDefaultStyles(CueGroups);
        return StyleGroups[GetCurrentStyleGroupIndex()];
    }

    public CueGroup GetCurrentCueGroup()
    {
        if (CueGroups == null || CueGroups.Count == 0)
        {
            return null;
        }

        return CueGroups[GetCurrentCueGroupIndex()];
    }

    public RCEProject()
    {
        StyleGroups = new List<StyleGroup>();
        CueGroups = new List<CueGroup>();
        ProjectName = "";
        Details = "";
        FileName = ""; 
        VideoFile = ""; 
    }

    public RCEProject(string _defaultCueGroup)
    {
        ProjectName = "";
        Details = "";
        FileName = "";
        VideoFile = ""; 
        CueGroups = new List<CueGroup>();
        CueGroup SPEECHGROUP = new CueGroup(_defaultCueGroup);
        CueGroups.Add(SPEECHGROUP);
        CreateNewStyleGroup();
    }
    public void CreateNewStyleGroup()
    {
        if (StyleGroups == null)
        {
            StyleGroups = new List<StyleGroup>();
        }
        StyleGroup styleGroup = new StyleGroup();
        styleGroup.SetDefaultStyles(CueGroups);
        StyleGroups.Add(styleGroup);
        styleGroup.Name = "Mapping Group " + StyleGroups.Count;
        currentStyleGroupIndex = StyleGroups.Count - 1;
    }

    //change cueStrings to cueChars - strings have smaller, slightly easier to read json files, chars are easier to parse and work with.
    internal void ConvertCueStringsToCueChars()
    {
        foreach (var cueGroup in CueGroups)
        {
            foreach (var cue in cueGroup.Cues)
            {
                foreach (var textSegment in cue.TextSegments)
                {
                    textSegment.CueStringToCueChar();
                }
            }
        }
    }

    internal void ConvertCueCharsToCueStrings()
    {
        foreach (var cueGroup in CueGroups)
        {
            foreach (var cue in cueGroup.Cues)
            {
                foreach (var textSegment in cue.TextSegments)
                {
                    textSegment.CueCharToCueString();
                }
            }
        }
    }

    internal void AddMapping(StyleGroup _styleGroup)
    {
        if (_styleGroup == null) return;
        foreach (Style style in _styleGroup.Styles)
        {
            if (!GetCurrentStyleGroup().Styles.Exists(s => s.IsMatch(style)))
            {
                GetCurrentStyleGroup().AddStyle(style);
            }
        }
    }

    internal void SetNextStyleGroup(int add)
    {
        if (GetCurrentStyleGroupIndex() + add == StyleGroups.Count)
        {
            CreateNewStyleGroup();
        }
        else
        {
            currentStyleGroupIndex += add;
        }
    }

    internal void DeleteCurrentStyleGroup()
    {
        if (StyleGroups == null || StyleGroups.Count == 0) return;
        if (StyleGroups.Count == 1)
        {
            return;//dont delete the only group. *************************** may want to clear the current group
        }
        StyleGroups.RemoveAt(GetCurrentStyleGroupIndex());
    }
}