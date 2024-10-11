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
    public List<Style> myStyles;
    public GameObject StyleButtonPrefab;

    public TMP_InputField StyleNameInput;
    public TMP_InputField StyleDetailsInput;

    public ElementInput myElementInput;

    public TMP_Dropdown FeatureDropDown;
    public TextMeshProUGUI FeaturesDetails;

    public static Style CurrentStyle;
    public static bool StylesHaveChanges; //safe pattern to not wind up in an endless loop in a single update if you forget to use SetValueWithoutNotify
    public static Action StyleChanged; //overkill, created to allow for future subscriptions to style changes.

    public Toggle RegionToggle;
    public Toggle CueToggle;
    public Toggle TextToggle;


    // Start is called before the first frame update
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
            Debug.Log("styles have changes");
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
        myStyles = ProjectManager.Instance.CurrentRCEProject.GetCurrentStyleGroup().Styles;
        CreateStyleButtons();
        myElementInput.ClearOptions();
        if (CurrentStyle == null && myStyles.Count > 0)
        {
            CurrentStyle = myStyles[0];
        }
        if (CurrentStyle != null)
        {
            StyleNameInput.SetTextWithoutNotify(CurrentStyle.Name);
            StyleDetailsInput.SetTextWithoutNotify(CurrentStyle.Description); 
            myElementInput.DisplayElementDetails(CurrentStyle.element); 
            List<string> features = FeatureManager.GetFeatureGroupNames(CurrentStyle.featureFilter, true).ToList();

            string currentElementName = CurrentStyle.element.Name;
            string currentFeatureName = CurrentStyle.GetFeatureName();

            if (ElementManager.ElementGroupExists(currentElementName) && !string.IsNullOrWhiteSpace(currentElementName))
            {
                ElementManager.CreateElementGroup(currentElementName, true, true);
            }
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
        myButtonContainer.Configure(StyleButtonPrefab, myStyles.Count + 1);
        for (int i = 0; i < myStyles.Count; i++)
        {
            StyleButton sb = myButtonContainer.buttons[i].GetComponent<StyleButton>();
            sb.Configure(myStyles[i]);
        }
        StyleButton sbAdd = myButtonContainer.buttons[myStyles.Count].GetComponent<StyleButton>();
        sbAdd.Configure(null);
    }
    public static void SetCurrentStyle(Style _style)
    {
        CurrentStyle = _style;
        StylesHaveChanges = true;
    }
}
