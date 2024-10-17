using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class CueGroupUI : MonoBehaviour
{
    public Image TabImage;
    public Image BackgroundImage;
    Color SelectedColor;
    Color UnselectedColor;

    public CueGroup myCueGroup;
    public Cue CurrentCue;

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
    public ElementInput CueElementInput;

    CaptionRenderer myCaptionRenderer;

    void Start()
    {
        VideoManager.CurrentTimeChanged += OnCurrentTimeChanged;
        CueGroupsMenu.CueGroupsChanged += OnCueGroupChanged;
        CueElementInput.ElementInputChanged += OnElementInputChanged;
        Cue.CueChanged += OnCueChanged;
        MenuManager.MenuLayoutUpdated += OnMenuLayoutUpdated;
        MenuManager.CurrentMenuStateChanged += OnCurrentMenuStateChanged;
    }

    private void OnDestroy()
    {
        VideoManager.CurrentTimeChanged -= OnCurrentTimeChanged;
        CueGroupsMenu.CueGroupsChanged -= OnCueGroupChanged;
        CueElementInput.ElementInputChanged -= OnElementInputChanged;
        DestroyImmediate(myCaptionRenderer.gameObject);
        Cue.CueChanged -= OnCueChanged;
        MenuManager.MenuLayoutUpdated -= OnMenuLayoutUpdated;
        MenuManager.CurrentMenuStateChanged -= OnCurrentMenuStateChanged;
    }

    private void OnMenuLayoutUpdated()
    {
        SetTab();
    }

    private void OnCurrentMenuStateChanged(MenuStates _menuStates)
    {
        SetTab();
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
    private void OnCueChanged(Cue _cue)
    {
        SetCurrentCue(VideoManager.currentTime);
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
        SetCurrentCue(_currentTime);
    }

    private void SetCurrentCue(double _currentTime)
    {
        if (myCueGroup == null || myCueGroup.Cues == null)
        {
            return;
        }
        CurrentCue = myCueGroup.GetCueByTime(_currentTime);

        DisplayCue();
    }

    public void ImportCueGroup()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            FileMenu.ImportFilePicker();
        }
    }

    internal void Configure(CueGroup _cueGroup, int _index, int _total, Color _color)
    {
        SelectedColor = _color;
        UnselectedColor = new Color(_color.r, _color.g, _color.b, .65f);
        myCueGroup = _cueGroup;
        Index = _index;
        TotalIndex = _total;
        if (myCaptionRenderer == null && CaptionManager.Instance != null)
        {
            myCaptionRenderer = CaptionManager.Instance.CreateCaptionRenderer();
        }
        myCaptionRenderer.Configure(myCueGroup);
       
        DisplayCueGroup();
    }

  
    public void GetCueGroupInfo()
    {
        if (myCueGroup.CurrentVariation != TextVariationDropdown.value)
        {
            myCueGroup.CurrentVariation = TextVariationDropdown.value;
            CueGroupsMenu.CueGroupsHaveChanges = true;
        }
        if (myCueGroup.Name != CueGroupNameInput.text)
        {
            ProjectManager.Instance.CurrentRCEProject.GetCurrentStyleGroup().ChangeCueGroupDefaultName(myCueGroup.Name, CueGroupNameInput.text);
            myCueGroup.Name = CueGroupNameInput.text;
            CueGroupsMenu.CueGroupsHaveChanges = true;
        }
        if (CurrentCue != null)
        {
            bool currentCueHasChanges = false;
            if (CurrentCue.RegionFeature != Region_Element_Dropdown.options[Region_Element_Dropdown.value].text)
            {
                CurrentCue.RegionFeature = Region_Element_Dropdown.options[Region_Element_Dropdown.value].text;
                currentCueHasChanges = true;
            }
            if (!CurrentCue.CueElement.IsEqual(CueElementInput.GetElement()))
            {
                CurrentCue.CueElement = CueElementInput.GetElement();
                currentCueHasChanges = true;
            }
            if (CurrentCue.Identifier != Cue_IdInput.text)
            {
                CurrentCue.Identifier = Cue_IdInput.text;
                currentCueHasChanges = true;
            }
            float StartTime = Common.TimeStringToFloat(Cue_StartTimeInput.text);
            float EndTime = Common.TimeStringToFloat(Cue_EndTimeInput.text);
            if (StartTime != CurrentCue.StartTime || EndTime != CurrentCue.EndTime)
            {
                myCueGroup.SetCueStartTime(CurrentCue, StartTime);//automatically triggers changed event
                myCueGroup.SetCueEndTime(CurrentCue, EndTime);//automatically triggers changed event
            }
            if (currentCueHasChanges) CurrentCue.TriggerChanged();
        }
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
        SetTab();
    }
    private void SetTab()
    { 
        RectTransform rt = GetComponent<RectTransform>();
        float newWidth = rt.rect.width / Mathf.Max(TotalIndex, 3);
        TabRt.sizeDelta = new Vector2(newWidth * .95f, 35);
        TabRt.anchoredPosition = new Vector2(Index * newWidth, 0);
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
        if (myCueGroup == null || CurrentCue == null)
        {
            return;
        }
        Cue_IdInput.SetTextWithoutNotify(CurrentCue.Identifier);
        Cue_StartTimeInput.SetTextWithoutNotify(Common.FloatToTimeString(CurrentCue.StartTime));
        Cue_EndTimeInput.SetTextWithoutNotify(Common.FloatToTimeString(CurrentCue.EndTime));

        SetRegionDropdown();
        CueElementInput.DisplayElementDetails(CurrentCue.CueElement, true, false);
    }

    private void ClearCue()
    {
        Cue_IdInput.SetTextWithoutNotify("");
        Cue_StartTimeInput.SetTextWithoutNotify("");
        Cue_EndTimeInput.SetTextWithoutNotify("");
        Region_Element_Dropdown.ClearOptions();
        CueElementInput.ClearOptions();
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
