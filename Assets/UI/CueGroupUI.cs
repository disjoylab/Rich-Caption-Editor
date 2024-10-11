using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class CueGroupUI : MonoBehaviour
{
    //TODO *************************************************************** THIS SHOULD TAKE OWNERSHIP OF THE CUE UI AND TELL IT WHAT TO DISPLAY
    //TODO *************************************************************** THIS SHOULD ALSO TAKE OWNERSHIP OF (OR CREATE) A CAPTION RENDERER 
    public Image TabImage;
    public Image BackgroundImage;
    Color SelectedColor;
    Color UnselectedColor;

    public CueGroup myCueGroup;
    public Cue PrevCue;
    public Cue CurrentCue;
    public Cue NextCue;

    public int Index;
    public int TotalIndex;



    //TAB
    public RectTransform TabRt;
    public TextMeshProUGUI CueGroupName;
    
    //CUE GROUP
    public TMP_InputField CueGroupNameInput;
    public TMP_Dropdown TextVariationDropdown;

    //CUE;
    public TMP_Dropdown Region_Element_Dropdown;
    public TMP_InputField Cue_IdInput;
    public TMP_InputField Cue_StartTimeInput;
    public TMP_InputField Cue_EndTimeInput;
    public TextMeshProUGUI CurrenTime;
    public TextMeshProUGUI PrevEndTime;
    public TextMeshProUGUI NextStartTime;
    public ElementInput CueElementInput;

    CaptionRenderer myCaptionRenderer;

    void Start()
    {
        VideoManager.CurrentTimeChanged += OnCurrentTimeChanged;
        CueGroupsMenu.CueGroupsChanged += OnCueGroupChanged;
        CueElementInput.ElementInputChanged += OnElementInputChanged;
    }

    private void OnDestroy()
    {
        VideoManager.CurrentTimeChanged -= OnCurrentTimeChanged;
        CueGroupsMenu.CueGroupsChanged -= OnCueGroupChanged;
        CueElementInput.ElementInputChanged -= OnElementInputChanged;
        DestroyImmediate(myCaptionRenderer.gameObject);
    }
    private void OnElementInputChanged()
    {
        GetCueGroupInfo();
    }

    private void OnCueGroupChanged()
    {
        DisplayCueGroup();
        DisplayCue();
    }


    public void DeleteGroup()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            CueGroupsMenu.Instance.DeleteCueGroup(myCueGroup);
        }
    }
    private void OnCurrentTimeChanged(double _currentTime)
    {
        CurrenTime.text = Common.FloatToTimeString((float)_currentTime);
        if (myCueGroup == null || myCueGroup.Cues == null)
        {
            return;
        }

        CurrentCue = null;
        PrevCue = null;
        NextCue = null;

        foreach (Cue cue in myCueGroup.Cues)
        {
            if (cue.StartTime <= _currentTime && cue.EndTime > _currentTime)
            {
                CurrentCue = cue;
            }
            else if (cue.EndTime <= _currentTime)
            {
                if (PrevCue == null || cue.EndTime > PrevCue.EndTime)
                {
                    PrevCue = cue;
                }
            }
            else if (cue.StartTime > _currentTime)
            {
                if (NextCue == null || cue.StartTime < NextCue.StartTime)
                {
                    NextCue = cue;
                }
            }
        }
        DisplayCue();

    }


    internal void Configure(CueGroup _cueGroup, int _index, int _total, Color _color)
    {
        SelectedColor = _color;
        UnselectedColor = new Color(_color.r * .8f, _color.b * .8f, _color.g * .8f, 1);
        myCueGroup = _cueGroup;
        Index = _index;
        TotalIndex = _total;
        if (myCaptionRenderer == null && CaptionManager.Instance != null)
        {
            myCaptionRenderer = CaptionManager.Instance.CreateCaptionRenderer();
        }
        myCaptionRenderer.Configure(myCueGroup);
        SetTab();
        DisplayCueGroup();
    }



    private void SetTab()
    {
        RectTransform rt = GetComponent<RectTransform>();
        float newWidth = rt.rect.width / Mathf.Max(TotalIndex, 3);
        TabRt.sizeDelta = new Vector2(newWidth * .95f, 35);
        TabRt.anchoredPosition = new Vector2(Index * newWidth, 0);
    }

    public void GetCueGroupInfo()
    {
        myCueGroup.CurrentVariation = TextVariationDropdown.value;
        if ( myCueGroup.Name != CueGroupNameInput.text)
        {
            ProjectManager.Instance.CurrentRCEProject.GetCurrentStyleGroup().ChangeCueGroupDefaultName(myCueGroup.Name, CueGroupNameInput.text); 
            myCueGroup.Name = CueGroupNameInput.text;

        }
        if (CurrentCue != null)
        {
            CurrentCue.RegionFeature = Region_Element_Dropdown.options[Region_Element_Dropdown.value].text;
        }
        //CUE ELEMENT 

        CurrentCue.CueElement = CueElementInput.GetElement();
        CurrentCue.Identifier = Cue_IdInput.text;
        float StartTime = Common.TimeStringToFloat(Cue_StartTimeInput.text);
        float EndTime = Common.TimeStringToFloat(Cue_EndTimeInput.text);
        if (StartTime != CurrentCue.StartTime)
        {
            CurrentCue.StartTime = Mathf.Min(StartTime, EndTime - 1);
            if (PrevCue != null)
            {
                CurrentCue.StartTime = MathF.Max(CurrentCue.StartTime, PrevCue.StartTime + 1);
                if (PrevCue.EndTime > CurrentCue.StartTime)
                {
                    PrevCue.EndTime = CurrentCue.StartTime - .01f;
                }
            }
        }
        if (EndTime != CurrentCue.EndTime)
        {
            CurrentCue.EndTime = Mathf.Max(StartTime + 1, EndTime);
            if (NextCue != null)
            {
                CurrentCue.EndTime = MathF.Min(CurrentCue.EndTime, NextCue.EndTime - 1);
                if (NextCue.StartTime < CurrentCue.EndTime)
                {
                    NextCue.StartTime = CurrentCue.EndTime + .01f;
                }
            }
        }
        CueGroupsMenu.CueGroupsHaveChanges = true;
    }
    private void DisplayCueGroup()
    {
        int CurrentIndex = ProjectManager.Instance.CurrentRCEProject.GetCurrentCueGroupIndex();
        SetColor(Index == CurrentIndex);
        if (myCueGroup == null)
        {
            CueGroupName.text = "+";
            TextVariationDropdown.ClearOptions();
        }
        else
        {
            CueGroupName.text = myCueGroup.Name;
            if (CurrentIndex == Index)
            {
                CueGroupNameInput.SetTextWithoutNotify(myCueGroup.Name);
                TextVariationDropdown.ClearOptions();
                TextVariationDropdown.AddOptions(myCueGroup.TextVariations);
                TextVariationDropdown.SetValueWithoutNotify(myCueGroup.CurrentVariation);
            }
        }

    }
    private void SetColor(bool selected)
    {
        Color c = selected ? SelectedColor : UnselectedColor;
        BackgroundImage.color = c;
        TabImage.color = c;
    }

    private void DisplayCue()
    {
        int CurrentIndex = ProjectManager.Instance.CurrentRCEProject.GetCurrentCueGroupIndex();
        if (Index != CurrentIndex)
        {
            return;
        }
        ClearCue();
        CueUI.SetCurrentCue(CurrentCue);//done here so CueUI gets null cues rather than just holding the last non-null cue
        if (myCueGroup == null)
        {
            return;
        }
        if (PrevCue != null)
        {
            PrevEndTime.text = Common.FloatToTimeString(PrevCue.EndTime);
        }
        if (NextCue != null)
        {
            NextStartTime.text = Common.FloatToTimeString(NextCue.StartTime);
        }
        if (CurrentCue == null)
        {
            return;
        }
        Cue_IdInput.SetTextWithoutNotify(CurrentCue.Identifier);
        Cue_StartTimeInput.SetTextWithoutNotify(Common.FloatToTimeString(CurrentCue.StartTime));
        Cue_EndTimeInput.SetTextWithoutNotify(Common.FloatToTimeString(CurrentCue.EndTime));

        SetRegionDropdown();
        CueElementInput.DisplayElementDetails(CurrentCue.CueElement);
    }

    private void ClearCue()
    {
        Cue_IdInput.SetTextWithoutNotify("");
        Cue_StartTimeInput.SetTextWithoutNotify("");
        Cue_EndTimeInput.SetTextWithoutNotify("");
        PrevEndTime.text = "";
        NextStartTime.text = "";
        Region_Element_Dropdown.ClearOptions();
        CueElementInput.ClearOptions();
    }

    public void SetNextCue(int _add)
    {
        if (_add < 0) // Moving backwards
        {
            if (VideoManager.Instance.videoPlayer.isPaused && CurrentCue != null && VideoManager.currentTime > CurrentCue.StartTime + 1)
            {
                VideoManager.Instance.SetTime(CurrentCue.StartTime + .05f);
            }
            else if (PrevCue != null)
            {
                if (CurrentCue != null && CurrentCue.StartTime - PrevCue.EndTime > 1)
                {
                    VideoManager.Instance.SetTime(PrevCue.EndTime + .05f);
                }
                else
                {
                    VideoManager.Instance.SetTime(PrevCue.StartTime + .05f);
                }
            }
        }
        else if (_add > 0) // Moving forwards
        {
            if (VideoManager.Instance.videoPlayer.isPaused && CurrentCue != null)
            {
                VideoManager.Instance.SetTime(CurrentCue.EndTime + .05f);
            }
            else if (NextCue != null)
            {
                if (CurrentCue != null && CurrentCue.EndTime < NextCue.StartTime)
                {
                    VideoManager.Instance.SetTime(CurrentCue.EndTime + .05f);
                }
                else
                {
                    VideoManager.Instance.SetTime(NextCue.StartTime + .05f);
                }
            }
        }
        else
        {
            VideoManager.Instance.SetTime(VideoManager.currentTime + _add);
        }
    }

    public void DeleteCurrentCue()
    {
        if (CurrentCue != null)
        {
            myCueGroup.Cues.Remove(CurrentCue);
            CueGroupsMenu.CueGroupsHaveChanges = true;
        }
    }

    public void NewCue()
    {
        Cue newCue = new Cue("New Cue");
        if (CurrentCue != null)
        {
            return;
        }
        float starttime = (float)VideoManager.currentTime - .05f;
        float endtime = starttime + 2;
        if (PrevCue != null)
        {
            starttime = PrevCue.EndTime + .01f;
        }
        if (NextCue != null)
        {
            endtime = NextCue.StartTime - .01f;
        }
        if (endtime - starttime < .5f)
        {
            return;
        }

        if (PrevCue != null)
        {
            int index = myCueGroup.Cues.IndexOf(PrevCue);
            myCueGroup.Cues.Insert(index + 1, newCue);
        }
        else if (NextCue != null)
        {
            int index = myCueGroup.Cues.IndexOf(NextCue);
            myCueGroup.Cues.Insert(index, newCue);
        }
        else
        {
            myCueGroup.Cues.Add(newCue);
        }

        newCue.StartTime = starttime;
        newCue.EndTime = endtime;
        CurrentCue = newCue;
        CueGroupsMenu.CueGroupsHaveChanges = true;
        VideoManager.InvokeTimeChangedEvent();//there is probably a better way to handle this, minimally the caption renderer needs to pick up on the new cue which is currently based on a time change.
    }

    private void SetRegionDropdown()
    {
        string[] FeatureNames = FeatureManager.GetFeatureGroupNames(FeatureFilter.Region, true);
         
        List<string> options = new List<string>();
        options.Add("");
        foreach (var featurenName in FeatureNames)
        { 
            options.Add(featurenName);
        }
        options.Add(CurrentCue.RegionFeature);
        Region_Element_Dropdown.AddOptions(options.Distinct().ToList()); 
        int currentIndex = options.IndexOf(CurrentCue.RegionFeature);          
        Region_Element_Dropdown.SetValueWithoutNotify(currentIndex); 
    }
   


    public void SetMe()
    {
        CueGroupsMenu.Instance.SetCurrentCueGroup(Index);
    }
}
