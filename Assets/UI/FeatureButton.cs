
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FeatureButton : MonoBehaviour
{
    public string myFeature;
    public Image buttonImage;
    public TextMeshProUGUI buttonText;
    public Toggle featureToggle;
 
    internal void Configure(string _name)
    {        
        myFeature = _name;       
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(SetCurrentFeature);
        }
        buttonText.text = string.IsNullOrWhiteSpace(myFeature) ? "Add Feature":myFeature;

        FeatureGroup featureGroup = FeatureManager.GetFeatureGroup(myFeature);
        if (featureGroup == null)
        {
            featureToggle.isOn = false;
        }
        else
        {
            featureToggle.isOn = featureGroup.Active;
        }
       
        buttonImage.color = featureToggle.isOn ? Color.white : new Color(.8f, .8f, .8f);        
    }

    public void SetCurrentFeature()
    {
        if (string.IsNullOrWhiteSpace(myFeature))
        {
            FeatureGroup newFeatureGroup = new FeatureGroup(FeatureManager.GetUniqueName("New Feature"));
            newFeatureGroup.CreateNewVersion();
            FeatureManager.AddFeatureGroup(newFeatureGroup);
            myFeature = newFeatureGroup.Name;
        }
        FeaturesMenu.SetCurrentFeature(myFeature);
    }

    public void SetFeatureGroupActive()
    {
        FeatureGroup featureGroup = FeatureManager.GetFeatureGroup(myFeature); 
        if (featureGroup != null)
        {
            featureGroup.Active = featureToggle.isOn; 
            FeatureManager.SaveFeatureGroupToJSON(featureGroup); 
            FeaturesMenu.SetCurrentFeature(myFeature); 
        }
    }
}
