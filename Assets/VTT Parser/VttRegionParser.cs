using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

internal class VttRegionParser
{
    public static Style ProcessRegionBlock(List<string> lines)
    {
        Style regionStyle = new Style();
        FeatureGroup featureGroup = new FeatureGroup(FeatureManager.GetUniqueName("Region_Feature"));
        Feature feature = new Feature("Imported_Region");
        foreach (var line in lines)
        {
            var parts = line.Split(":");
            if (parts.Length == 2)
            {
                string settingName = parts[0].Trim();
                string settingValue = parts[1].Trim();
                if (settingName == "id")
                {
                    featureGroup.Name = FeatureManager.GetUniqueName("Region_Feature_" + settingValue);
                    regionStyle.element = new Element(settingValue); 
                }
                else
                {
                    feature.Settings.Add(new Setting("Region_" + settingName, settingValue));
                }
            }
        }
        regionStyle.SetFeature("Not Found");//hacky way to return this state, empty is a valid region while no settings is not valid
        if (feature.Settings.Count > 0)
        {
            FeatureGroup ExistingFeatureGroup = FeatureManager.FindMatch(feature);
            if (ExistingFeatureGroup == null)
            {
                featureGroup.AddFeature(feature,false);
                FeatureManager.AddFeatureGroup(featureGroup);
            }
            else
            {
                featureGroup = ExistingFeatureGroup;
            }
            regionStyle.SetFeature(featureGroup.Name); 
            
        }

        return regionStyle;
    }
}