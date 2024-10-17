using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StylesMenu : MonoBehaviour
{
    public ButtonContainer myButtonContainer; 
    public GameObject StyleButtonPrefab;

    public TMP_InputField StyleNameInput;
    public TMP_InputField StyleDetailsInput;

    public ElementInput myElementInput;

    public TMP_Dropdown FeatureDropDown;
    public TextMeshProUGUI FeaturesDetails;

    public static Style CurrentStyle;
    public static bool StylesHaveChanges; //could replace with event on each style
    public static Action StyleChanged; 

    public Toggle RegionToggle;
    public Toggle CueToggle;
    public Toggle TextToggle;

    //STYLE GROUP
    public Button PrevStyleGroupButton;
    public Button NextStyleGroupButton;
    public TMP_Text NextStyleGroupButtonText;
    public Button DeleleStyleGroupButton;
    public Text StyleGroupVersionNumberText;
    public TMP_InputField StyleGroupNameInput;

    void Start()
    {
        ProjectManager.ProjectChanged += OnProjectChanged;
        StyleChanged += OnStyleChanged;
    }
    private void OnEnable()
    {
        DisplayStyles();
    }

    private void OnDestroy()
    {
        ProjectManager.ProjectChanged -= OnProjectChanged;
        StyleChanged -= OnStyleChanged;
    }

    private void Update()
    {
        if (StylesHaveChanges)
        {
            StylesHaveChanges = false;
            StyleChanged?.Invoke();
        }
    }

    private void OnStyleChanged()
    {
        DisplayStyles();
    }

    private void OnProjectChanged()
    {
        DisplayStyles();
    }

    void DisplayStyles()
    {
        StyleGroup CurrentStyleGroup = ProjectManager.Instance.CurrentRCEProject.GetCurrentStyleGroup();
        if (CurrentStyleGroup != null)
        {
            int CurrentIndex = ProjectManager.Instance.CurrentRCEProject.GetCurrentStyleGroupIndex();
            int TotalGroups = ProjectManager.Instance.CurrentRCEProject.StyleGroups.Count;

            PrevStyleGroupButton.gameObject.SetActive(CurrentIndex > 0);
            NextStyleGroupButtonText.text = (CurrentIndex == TotalGroups - 1) ? "+" : ">";
            DeleleStyleGroupButton.gameObject.SetActive(TotalGroups > 1);
            StyleGroupVersionNumberText.text = $"{CurrentIndex+1}/{TotalGroups}";
            StyleGroupNameInput.SetTextWithoutNotify(CurrentStyleGroup.Name);
        }
        else
        { return; }
        CreateStyleButtons();
        myElementInput.ClearOptions();
        if (CurrentStyle == null && CurrentStyleGroup.Styles.Count > 0)
        {
            CurrentStyle = CurrentStyleGroup.Styles[0];
        }
        if (CurrentStyle != null)
        {
            StyleNameInput.SetTextWithoutNotify(CurrentStyle.Name);
            StyleDetailsInput.SetTextWithoutNotify(CurrentStyle.Description);

            //element...
            string currentElementName = CurrentStyle.element.Name;
            myElementInput.DisplayElementDetails(CurrentStyle.element,true,true); if (ElementManager.ElementGroupExists(currentElementName) && !string.IsNullOrWhiteSpace(currentElementName))
            {
                ElementManager.CreateElementGroup(currentElementName, true, true);
            }

            //feature...
            List<string> features = FeatureManager.GetFeatureGroupNames(CurrentStyle.featureFilter, true).ToList();           
            string currentFeatureName = CurrentStyle.GetFeatureName();           
            if (FeatureManager.GetFeatureGroup(currentFeatureName) == null && !string.IsNullOrWhiteSpace(currentFeatureName))
            {
                FeatureManager.AddFeatureGroup(new FeatureGroup(currentFeatureName));
            }
            FeatureDropDown.ClearOptions();

            features.Insert(0, "");
            if (!features.Contains(currentFeatureName))
            {
                features.Add(currentFeatureName); //Add it to the list in case it was filtered out (we know it exists and the UI should show it)
            }
            FeatureDropDown.AddOptions(features.ToList());

            int featureIndex = features.FindIndex(f => f.ToLower() == currentFeatureName.ToLower());

            if (featureIndex >= 0)
            {
                FeatureDropDown.SetValueWithoutNotify(featureIndex);
            }
            FeatureDropDown.RefreshShownValue();

            Feature feature = CurrentStyle.GetFeature();
            FeaturesDetails.text = feature == null ? "" : feature.ToString();
            RegionToggle.SetIsOnWithoutNotify((CurrentStyle.featureFilter & FeatureFilter.Region) == FeatureFilter.Region);
            CueToggle.SetIsOnWithoutNotify((CurrentStyle.featureFilter & FeatureFilter.Cue) == FeatureFilter.Cue);
            TextToggle.SetIsOnWithoutNotify((CurrentStyle.featureFilter & FeatureFilter.Text) == FeatureFilter.Text);
        }
    }

    public void GetCurrentStyleInfo()
    {
        StyleGroup CurrentStyleGroup = ProjectManager.Instance.CurrentRCEProject.GetCurrentStyleGroup();
        
        if (StyleGroupNameInput.text != CurrentStyleGroup.Name )
        {
            CurrentStyleGroup.Name = StyleGroupNameInput.text;
        }
        if (CurrentStyle == null)
            return;

        CurrentStyle.Name = StyleNameInput.text;
        CurrentStyle.Description = StyleDetailsInput.text;
        CurrentStyle.element = myElementInput.GetElement();

        CurrentStyle.featureFilter = FeatureFilter.None;
        if (RegionToggle.isOn)
        {
            CurrentStyle.featureFilter |= FeatureFilter.Region;
        }
        if (CueToggle.isOn)
        {
            CurrentStyle.featureFilter |= FeatureFilter.Cue;
        }
        if (TextToggle.isOn)
        {
            CurrentStyle.featureFilter |= FeatureFilter.Text;
        }

        string selectedFeatureName = FeatureDropDown.options[FeatureDropDown.value].text;
        if (selectedFeatureName != CurrentStyle.GetFeatureName())
        {
            CurrentStyle.SetFeature(selectedFeatureName); //must do only if name changes and after featurefilter because setting feature changes filter
        }

        StylesHaveChanges = true;
    }

    public void DeleteCurrentStyle()
    {
        ProjectManager.Instance.CurrentRCEProject.GetCurrentStyleGroup().Styles.Remove(CurrentStyle);
        StylesHaveChanges = true;
    }

    public static void AddStyle()
    {
        Style style = new Style("New Style");
        style.element = new Element("");
        style.SetFeature("");
        StyleGroup sg = ProjectManager.Instance.CurrentRCEProject.GetCurrentStyleGroup();
        sg.AddStyle(style);
    }

    void CreateStyleButtons()
    {
        StyleGroup CurrentStyleGroup = ProjectManager.Instance.CurrentRCEProject.GetCurrentStyleGroup();
        myButtonContainer.Configure(StyleButtonPrefab, CurrentStyleGroup.Styles.Count + 1);
        for (int i = 0; i < CurrentStyleGroup.Styles.Count; i++)
        {
            StyleButton sb = myButtonContainer.buttons[i].GetComponent<StyleButton>();
            sb.Configure(CurrentStyleGroup.Styles[i]);
        }
        StyleButton sbAdd = myButtonContainer.buttons[CurrentStyleGroup.Styles.Count].GetComponent<StyleButton>();
        sbAdd.Configure(null);
    }
    public static void SetCurrentStyle(Style _style)
    {
        CurrentStyle = _style;
        StylesHaveChanges = true;
    }
    public void SetNextStyleGroup(int _add)
    {
        ProjectManager.Instance.CurrentRCEProject.SetNextStyleGroup(_add);
        DisplayStyles();
    }
    public void DeleteCurrentStyleGroup()
    {
        ProjectManager.Instance.CurrentRCEProject.DeleteCurrentStyleGroup();
        DisplayStyles();
    }
}
