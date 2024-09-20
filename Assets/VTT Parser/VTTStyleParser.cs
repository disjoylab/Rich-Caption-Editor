using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

internal class VTTStyleParser
{
    internal static Style ProcessStyleBlock(string line)
    {
        bool settingFound = false;
        Style style = new Style();
        style.element = new Element(""); 
        var cueModifierMatch = Regex.Match(line, @"::cue\(([^)]+)\)");
        if (cueModifierMatch.Success)
        {
            string cueModifiers = cueModifierMatch.Groups[1].Value; 
            var idPattern = new Regex(@"#([^\s.]+)");
            var attributePattern = new Regex(@"\.([a-zA-Z0-9\\]+)");
            var simpleElementPattern = new Regex(@"^[a-zA-Z0-9]+");
            var complexElementPattern = new Regex(@"\[([a-zA-Z0-9]+)=""([^""]+)""\]"); 

            if (cueModifiers.StartsWith("#"))
            {
               string Identifier = Regex.Replace(cueModifiers.Substring(1), @"\\([0-9a-fA-F]{2} )|\\( )", match =>
                {
                    if (match.Groups[1].Success)
                    {
                        string hex = match.Groups[1].Value.Trim();
                        int value = Convert.ToInt32(hex, 16);
                        return ((char)value).ToString();
                    }
                    else if (match.Groups[2].Success)
                    {
                        return match.Groups[2].Value;
                    }
                    return match.Value;
                });
                style.element.Signature.Name = Identifier;
            } 
            foreach (Match attributeMatch in attributePattern.Matches(cueModifiers))
            {
                style.element.Signature.Attributes.Add(attributeMatch.Groups[1].Value);
            }             
            foreach (Match complexElementMatch in complexElementPattern.Matches(cueModifiers))
            {
                string elementName = complexElementMatch.Groups[1].Value;
                style.element.Signature.Name = elementName.ToLower() == "voice" ? "v" : elementName;                
                style.element.Value= complexElementMatch.Groups[2].Value; 
            }
            var simpleElementMatch = simpleElementPattern.Match(cueModifiers);
            if (simpleElementMatch.Success)
            {
                style.element.Signature.Name = simpleElementMatch.Value;                 
            }         
        }
        var settingsPattern = new Regex(@"\{([^}]*)\}");
        var settingsMatch = settingsPattern.Match(line);
        if (settingsMatch.Success)
        {
            string settingsText = settingsMatch.Groups[1].Value;
            var settingPairs = settingsText.Split(';');
            FeatureGroup featureGroup = new FeatureGroup(FeatureManager.GetUniqueName("Style_Feature"));
            Feature feature = new Feature("Imported_Style");
            foreach (var pair in settingPairs)
            {
                var setting = pair.Split(new[] { ':' }, 2);
                if (setting.Length == 2)
                {
                    string settingName = setting[0].Trim();
                    string settingValue = setting[1].Trim();
                    feature.Settings.Add(new Setting("Style_"+settingName, settingValue));
                    settingFound = true;
                }
            }
            if (feature.Settings.Count>0)
            {
                FeatureGroup ExistinFeatureGroup = FeatureManager.FindMatch(feature);
                if (ExistinFeatureGroup == null)
                {
                    featureGroup.AddFeature(feature,false);
                    FeatureManager.AddFeatureGroup(featureGroup);
                }
                else
                {
                    featureGroup = ExistinFeatureGroup; 
                }
                style.SetFeature(featureGroup.Name); 
            }
        }
        return settingFound ? style : null;
    }
}