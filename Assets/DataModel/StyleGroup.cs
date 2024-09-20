using System;
using System.Collections.Generic;

[Serializable]
public class StyleGroup
{
    //add name
    public List<Style> Styles;

    public StyleGroup()
    {
        Styles = new List<Style>();        
    }

    public void SetDefaultStyles()
    {
        if (!Styles.Exists(s => s.element.Signature.Name == "b"))
        {
            Style bold = new Style();
            bold.element = new Element("b", "");
            bold.SetFeature("Bold");
            Styles.Add(bold);
        }
        if (!Styles.Exists(s => s.element.Signature.Name == "i"))
        {
            Style italic = new Style();
            italic.element = new Element("i", "");
            italic.SetFeature("Italic");
            Styles.Add(italic);
        }
        if (!Styles.Exists(s => s.element.Signature.Name == "u"))
        {
            Style underline = new Style();
            underline.element = new Element("u", "");
            underline.SetFeature("Underline");

            Styles.Add(underline);
        }
        if (!Styles.Exists(s => s.element.Signature.Name == "s"))
        {
            Style strikethrough = new Style();
            strikethrough.element = new Element("s", "");
            strikethrough.SetFeature("Strikethrough");
            Styles.Add(strikethrough);
        }
        if (!Styles.Exists(s => s.element.Signature.Name == "defaults"))
        {
            Style defaults = new Style();
            defaults.SetFeature("Defaults");
            Styles.Add(defaults);
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
                ElementGroup element = ElementManager.GetElementGroup(style.element.Signature.Name);
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
