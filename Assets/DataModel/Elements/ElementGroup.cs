using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

public class ElementGroup 
{
    public string Name;
    public string Description;

    [JsonProperty]
    public List<ElementDefinition> DefinitionVersions;

    public bool Active;
    public int CurrentVersion;

    public ElementGroup(string _name,bool _cueLevel, bool _textLevel)
    {
        Name = _name;
        Description = "";
        Active = true;
        DefinitionVersions = new List<ElementDefinition>();
        DefinitionVersions.Add(new ElementDefinition("", _cueLevel, _textLevel));
    }
    public ElementDefinition CurrentDefinition()
    {
        if (DefinitionVersions.Count == 0)
        {
            return null;
        }
        CurrentVersion = UnityEngine.Mathf.Clamp(CurrentVersion, 0, DefinitionVersions.Count - 1);
        return DefinitionVersions[CurrentVersion];
    }
    internal string ToFileName()
    {
        return $"{Name}.element";
    }
    internal bool IsLastVersion()
    {
        return CurrentVersion + 1 >= DefinitionVersions.Count;
    }
    internal void CreateNewVersion( bool _cueLevel, bool _textLevel)
    {
        ElementDefinition definition = new ElementDefinition("New Version " + (DefinitionVersions.Count + 1),_cueLevel,_textLevel);

        DefinitionVersions.Add(definition);
    }
    internal void DeleteCurrentVersion()
    {
        DefinitionVersions.Remove(CurrentDefinition());
        if (DefinitionVersions.Count == 0)
        {
            CreateNewVersion(true,true);
        }
    }
    public string FeatureVersionString()
    {
        return $"V: {(CurrentVersion + 1)} / {DefinitionVersions.Count}";
    }
    public void AddCurrentVersion(int _add)
    {
        CurrentVersion = UnityEngine.Mathf.Clamp(CurrentVersion + _add, 0, DefinitionVersions.Count - 1);
    }

}
