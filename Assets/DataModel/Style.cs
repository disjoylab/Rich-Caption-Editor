
using System;
using System.Collections.Generic;
using Newtonsoft.Json;


[Serializable]
public class Style
{
    public String Name;
    public string Description;
    [JsonProperty]
    private string FeatureName; 
    public Element element;
    public FeatureFilter featureFilter;
    private string v;

    public Style()
    {
        Name = "";
        Description = "";
        FeatureName = "";
        element = new Element("");
        featureFilter = FeatureFilter.ALL;
    }

    public Style(string _name)
    {
        Name = _name;
        Description = "";
        FeatureName = "";
        element = new Element("");
        featureFilter = FeatureFilter.ALL;
    }

    public string GetFeatureName()
    {
        return FeatureName;
    }
    public void SetFeature(string _featureName)
    {
        FeatureName = _featureName;
        FeatureGroup feature = FeatureManager.GetFeatureGroup(FeatureName);
        if (feature != null)
        {
            featureFilter = feature.GetFeatureType(); 
        }
        if (string.IsNullOrWhiteSpace(_featureName))
        {
            featureFilter = FeatureFilter.ALL;
        }
    }
    public Feature GetFeature()
    {
        return FeatureManager.GetFeature(FeatureName);
    }
    
    internal string ToButtonText()
    {
        string buttonText = element.ToButtonText();
                
         buttonText += $" :: {FeatureName}";
        
        return buttonText;
    }
    public bool IsEmpty()
    {
        return string.IsNullOrEmpty(Name) && string.IsNullOrEmpty(FeatureName) && element.IsEmpty();
    }

    internal bool IsDefault(string name)
    {
        return element.Name.ToLower() == "default" &&
         (string.IsNullOrEmpty(element.Value) || element.Value == name);
    }
}