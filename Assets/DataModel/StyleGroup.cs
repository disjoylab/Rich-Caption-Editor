using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class StyleGroup
{
    //add name
    public List<Style> Styles;

    public StyleGroup()
    {
        Styles = new List<Style>();        
    }

    public void SetDefaultStyles(List<CueGroup> _cueGroups)
    {
        if (!Styles.Exists(s => s.Name == "Default"))
        {
            Style DefaultStyle = new Style("Default");
            DefaultStyle.element = new Element("Default","");
            DefaultStyle.SetFeature("Default Feature"); 
            Styles.Insert(0,DefaultStyle);
        }
        if (!Styles.Exists(s => s.Name == "Strikethrough"))
        {
            Style strikethrough = new Style("Strikethrough");
            strikethrough.element = new Element("s", "");
            strikethrough.SetFeature("Strikethrough");
            Styles.Insert(1,strikethrough);
        }
        if (!Styles.Exists(s => s.Name == "Underline"))
        {
            Style underline = new Style("Underline");
            underline.element = new Element("u", "");
            underline.SetFeature("Underline");

            Styles.Insert(1,underline);
        }
        if (!Styles.Exists(s => s.Name == "Italic"))
        {
            Style italic = new Style("Italic");
            italic.element = new Element("i", "");
            italic.SetFeature("Italic");
            Styles.Insert(1,italic);
        }
        if (!Styles.Exists(s => s.Name == "Bold"))
        {
            Style bold = new Style("Bold");
            bold.element = new Element("b", "");
            bold.SetFeature("Bold");
            Styles.Insert(1,bold);
        }
        foreach (var cueGroupName in _cueGroups.Select(cg => cg.Name).ToList())
        {
            string GroupDefaultStyleName = "Default " + cueGroupName;
            if (!Styles.Exists(s => s.Name == GroupDefaultStyleName))
            {
                Style GroupDefaultStyle = new Style(GroupDefaultStyleName);
                GroupDefaultStyle.element = new Element("Default",cueGroupName); 
                Styles.Insert(1,GroupDefaultStyle);
            }
        }
    }
    public void ChangeCueGroupDefaultName(string _oldCueGroupName, string _newCueGroupName)
    { 
        foreach (var style in Styles)
        {            
            if (style.element.Name.ToLower() == "default" && style.element.Value.ToLower() == _oldCueGroupName.ToLower())
            {
                style.element.Value = _newCueGroupName;
                style.Name = "Default " + _newCueGroupName;
            }
        }
    }

    public List<Style> GetStylesByType(FeatureFilter _filter, bool _includeInactive = false)// use GetStylesByType(StyleFilter.Cue | StyleFilter.Text); for multiple style filters

    {
        List<Style> returnStyles = new List<Style>();

        foreach (var style in Styles)
        {
            bool filterMatch = (style.featureFilter & _filter) != FeatureFilter.None;

            if (filterMatch)
            {
                ElementGroup element = ElementManager.GetElementGroup(style.element.Name);
                if (element != null)
                {
                    if (element.Active || _includeInactive)
                    {
                        returnStyles.Add(style);
                    }
                }
            }
        }
        return returnStyles;
    }

    internal void AddStyle(Style _style)
    {
        Styles.Add(_style);
    }
}
