using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

internal class VttRegionParser
{
    public static Feature ProcessRegionBlock(List<string> lines)
    {
        string featureGroupName = "";
        Feature feature = new Feature("Imported Region");
        foreach (var line in lines)
        {
            var parts = line.Split(":");
            if (parts.Length == 2)
            {
                string settingName = parts[0].Trim();
                string settingValue = parts[1].Trim();
                if (settingName == "id")
                {
                    featureGroupName = "Region " + settingValue; 
                }
                else
                {
                    feature.Settings.Add(new Setting("Region_" + settingName, settingValue));
                }
            }
        } 
        if (feature.Settings.Count > 0 && !string.IsNullOrEmpty(featureGroupName))
        {
            FeatureGroup ExistingFeatureGroup = FeatureManager.GetFeatureGroup(featureGroupName);
            if (ExistingFeatureGroup == null)
            {
                FeatureGroup featureGroup = new FeatureGroup(featureGroupName);
                featureGroup.AddFeature(feature, false);
                FeatureManager.AddFeatureGroup(featureGroup);
            }
            else
            {
                ExistingFeatureGroup.AddFeature(feature, true);
            }
        }
        return feature;
    }
}