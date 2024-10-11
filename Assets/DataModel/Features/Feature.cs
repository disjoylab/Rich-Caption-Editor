using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

[Serializable]
public class Feature
{ 
    public string Description;
    public List<Setting> Settings; 
    
    public Feature()
    {
        Description = "";
        Settings = new List<Setting>(); 
    }
    public Feature(string _description)
    {
        Description = _description;
        Settings = new List<Setting>();
    } 

      
    public bool IsMatch(Feature _feature)
    {
        if (_feature == null)
            return false;
        if (Settings.Count != _feature.Settings.Count) return false; 
        foreach (Setting setting1 in Settings)
        {
            if (!_feature.Settings.Any(setting2 => setting1.IsMatch(setting2)))
            {
                return false;
            }
        } 
        return true;
    }
    public bool IsFeatureType(FeatureFilter _featureFilter)
    {
        FeatureFilter currentFeatureType = GetFeatureType();
        return (currentFeatureType & _featureFilter) !=FeatureFilter.None;
    }


    public FeatureFilter GetFeatureType()
    {
        FeatureFilter result = FeatureFilter.None;
         
            foreach (Setting setting in Settings)
            {
                if (setting.GetSettingType().ToString().ToLower().StartsWith("region"))
                {
                    result |= FeatureFilter.Region;
                }
                if (setting.GetSettingType().ToString().ToLower().StartsWith("cue"))
                {
                    result |= FeatureFilter.Cue;
                }
                if (setting.GetSettingType().ToString().ToLower().StartsWith("style"))
                {
                    result |= FeatureFilter.Text;
                }
            } 
        return result;
    }

    internal void CopySettings(Feature _feature)
    {         
        Settings = new List<Setting>();
        foreach (Setting setting in _feature.Settings)
        {
            Settings.Add(new Setting(setting));
        }
    }
    public override string ToString()
    {
        string stringText = Description  +"\n";
        foreach (var setting in Settings)
        {
            stringText += setting.ToString() +"\n";
        }
        return stringText;
    }
}
