using System; 
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

[Flags]
public enum FeatureFilter { None = 0, Region = 1, Cue = 2, Text = 4, ALL = Region | Cue | Text }

[Serializable]
public class FeatureGroup
{
    public string Name;
    public string Description;
     [JsonProperty]
     List<Feature> FeatureVersions;

    public bool Active;
    public int CurrentVersion;

    public FeatureGroup()
    {
        Name = "";
        Description = "";
        FeatureVersions = new List<Feature>();
        Active = true;
    }
    public FeatureGroup(string _name)
    {
        Name = _name;
        Description = "";
        FeatureVersions = new List<Feature>();
        Active = true;
    }

    public FeatureGroup(string _name, Feature _feature)
    {
        Name = _name;
        Description = "";
        FeatureVersions = new List<Feature>();
        AddFeature(_feature,false);
        Active = true;
    }

    public string[] GetFeatureVersionNames()
    {         
        return FeatureVersions.Select(f => f.Description).ToArray();
    }  

    public bool AddFeature(Feature _feature,bool _checkExists)
    {
        if (_feature == null)
        {
            return false;
        }
        if (_checkExists && IsMatch(_feature))
        {
            return false;
        }
        FeatureVersions.Add(_feature);
        CurrentVersion = FeatureVersions.Count - 1;
        return true;
    }
     
    public Feature CurrentFeature()
    {
        if (FeatureVersions.Count==0)
        {
            return null;
        }
        CurrentVersion = UnityEngine.Mathf.Clamp(CurrentVersion , 0, FeatureVersions.Count - 1);
        return FeatureVersions[CurrentVersion];
    }

    public void AddCurrentVersion(int _add)
    {
        CurrentVersion = UnityEngine.Mathf.Clamp(CurrentVersion+ _add,0, FeatureVersions.Count-1);
    }

    public bool IsMatch(Feature _feature)
    {
        foreach (Feature feature in FeatureVersions)
        {
            if (feature.IsMatch(_feature))
            {
                return true;
            }
        }
        return false;
    }

    internal bool IsFeatureType(FeatureFilter _featureFilter)
    {
        foreach (Feature feature in FeatureVersions)
        {
            if (feature.IsFeatureType(_featureFilter))
            {
                return true;
            }
        }
        return false;
    }

    internal bool IsLastVersion()
    {
       return CurrentVersion + 1 >= FeatureVersions.Count;
    }

    internal string ToFileName()
    {
        return $"{Name}.feature";
    }

    public FeatureFilter GetFeatureType()
    {
        FeatureFilter result = FeatureFilter.None;

        foreach (Feature feature in FeatureVersions)
        {
            result |= feature.GetFeatureType();
        }
        return result;
    }

    internal void CreateNewVersion()
    {
        Feature feature = new Feature("New Version " + (FeatureVersions.Count + 1));
        if (FeatureVersions.Count>0)
        {
            feature.CopySettings(FeatureVersions[FeatureVersions.Count - 1]);
        }
        FeatureVersions.Add(feature);
    }

    public string FeatureVersionString()
    { 
        return $"V: {(CurrentVersion + 1)} / {FeatureVersions.Count}";
    }

    internal void DeleteCurrentVersion()
    {        
        FeatureVersions.Remove(CurrentFeature()); 
        if (FeatureVersions.Count==0)
        {
            CreateNewVersion();
        }       
    }
}
