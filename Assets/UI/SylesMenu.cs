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

    public TMP_Dropdown ElementDropDown;
    public TMP_InputField AttributesInput;
    public TMP_InputField ValueInput;

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
        if (CurrentStyle==null && myStyles.Count>0)
        {
            CurrentStyle = myStyles[0];
        }
        if (CurrentStyle != null)
        {
            StyleNameInput.SetTextWithoutNotify( CurrentStyle.Name);
            StyleDetailsInput.SetTextWithoutNotify(CurrentStyle.Description);
            AttributesInput.SetTextWithoutNotify(CurrentStyle.element.Signature.AttributesToString());
            ValueInput.SetTextWithoutNotify(CurrentStyle.element.Value);
            List<string> elements = ElementManager.GetElementGroupNames(true).ToList();
            List<string> features = FeatureManager.GetFeatureGroupNames(CurrentStyle.featureFilter,true).ToList();

            string currentElementName = CurrentStyle.element.Signature.Name;
            string currentFeatureName = CurrentStyle.GetFeatureName();

            if (ElementManager.ElementGroupExists(currentElementName)&&!string.IsNullOrWhiteSpace(currentElementName))
            {
                ElementManager.CreateElementGroup(currentElementName);
            }
            if (FeatureManager.GetFeatureGroup(currentFeatureName)==null && !string.IsNullOrWhiteSpace(currentFeatureName))
            {
                FeatureManager.AddFeatureGroup(new FeatureGroup(currentFeatureName));
            }
            ElementDropDown.ClearOptions();
            FeatureDropDown.ClearOptions();

            elements.Insert(0, "");           
            features.Insert(0, "");
            if (!features.Contains(currentFeatureName))
            {
                features.Add(currentFeatureName); //Add it to the list in case it was filtered out (we know it exists and the UI should show it)
            }
            // Add new options
            ElementDropDown.AddOptions(elements.ToList());
            FeatureDropDown.AddOptions(features.ToList());

            // Select the current element and feature
            int elementIndex = elements.FindIndex(e=>e.ToLower()== currentElementName.ToLower());
            int featureIndex =  features.FindIndex(f=>f.ToLower()== currentFeatureName.ToLower());

            if (elementIndex >= 0)
            {
                ElementDropDown.SetValueWithoutNotify(elementIndex);
            }
            if (featureIndex >= 0)
            {
                FeatureDropDown.SetValueWithoutNotify(featureIndex);
            }

            // Refresh the dropdown to show the selected value
            ElementDropDown.RefreshShownValue();
            FeatureDropDown.RefreshShownValue();

            Feature feature = CurrentStyle.GetFeature();
            if (feature!=null)
            {
                FeaturesDetails.text = feature.ToString();
            }
            RegionToggle.SetIsOnWithoutNotify( (CurrentStyle.featureFilter & FeatureFilter.Region) == FeatureFilter.Region); 
            CueToggle.SetIsOnWithoutNotify(  (CurrentStyle.featureFilter & FeatureFilter.Cue) == FeatureFilter.Cue);
            TextToggle.SetIsOnWithoutNotify( (CurrentStyle.featureFilter & FeatureFilter.Text) == FeatureFilter.Text);
        }
    }

    public void GetCurrentStyleInfo()
    {
        if (CurrentStyle == null)
            return;
         
        CurrentStyle.Name = StyleNameInput.text;
        CurrentStyle.Description = StyleDetailsInput.text;

        CurrentStyle.element.Signature.Name = ElementDropDown.options[ElementDropDown.value].text;
        CurrentStyle.element.Signature.SetAttributesFromString(AttributesInput.text);
        CurrentStyle.element.Value = ValueInput.text;

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
        myButtonContainer.Configure(StyleButtonPrefab, myStyles.Count+1);
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
