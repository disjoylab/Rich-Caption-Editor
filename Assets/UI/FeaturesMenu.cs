using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FeaturesMenu : MonoBehaviour
{ 

    public ButtonContainer featuresButtonContainer;
    public GameObject FeatureButtonPrefab;
    public ButtonContainer settingsButtonContainer; 
    public GameObject SettingButtonPrefab;
    static FeatureGroup CurrentFeatureGroup;

    public TMP_InputField FeatureNameInput;
    public TMP_InputField FeatureDetailsInput;
    public TMP_InputField FeatureVersionDetailsInput;

    public TMP_Text PrevText;
    public TMP_Text NextText;
    public TMP_Text VersionNumberText;
 
    void Start()
    {
        FeatureManager.FeaturesChanged += OnFeaturesChanged; 
        DisplayFeatures();
    }
    private void OnDestroy()
    {
        FeatureManager.FeaturesChanged -= OnFeaturesChanged;
    }
    private void OnEnable()
    {
        DisplayFeatures();
    }
    private void OnFeaturesChanged()
    { 
        DisplayFeatures();
    }

    private void DisplayFeatures()
    {
        string[] _featureNames = FeatureManager.GetFeatureGroupNames();        
        featuresButtonContainer.Configure(FeatureButtonPrefab, _featureNames.Length+1);
        for (int i = 0; i < _featureNames.Length; i++)
        { 
            featuresButtonContainer.buttons[i].GetComponent<FeatureButton>().Configure(_featureNames[i]); 
        }
        featuresButtonContainer.buttons[_featureNames.Length].GetComponent<FeatureButton>().Configure("");//this is the add feature button


        //CURRENT FEATURE
        if (CurrentFeatureGroup == null && _featureNames.Length > 0)
        {
            CurrentFeatureGroup = FeatureManager.GetFeatureGroup(_featureNames[0]);
        }
        if (CurrentFeatureGroup != null)
        {
            FeatureNameInput.text = CurrentFeatureGroup.Name;
            FeatureDetailsInput.text = CurrentFeatureGroup.Description;
            Feature currentFeatureVersion = CurrentFeatureGroup.CurrentFeature();
            if (currentFeatureVersion != null)
            {
                FeatureVersionDetailsInput.text = currentFeatureVersion.Description;
                settingsButtonContainer.Configure(SettingButtonPrefab, currentFeatureVersion.Settings.Count+1);
                for (int i = 0; i < currentFeatureVersion.Settings.Count; i++)
                {
                    settingsButtonContainer.buttons[i].GetComponent<SettingPanel>().Configure(currentFeatureVersion.Settings[i]);
                }
                settingsButtonContainer.buttons[currentFeatureVersion.Settings.Count].GetComponent<SettingPanel>().Configure(new Setting());
            } 
            PrevText.enabled = CurrentFeatureGroup.CurrentVersion > 0;
            NextText.text = CurrentFeatureGroup.IsLastVersion() ? "+" : ">";
            VersionNumberText.text = CurrentFeatureGroup.FeatureVersionString();

        }
    }

    public static bool[] GetSettingTypesUsed() 
    {
        bool[] settingTypesUsed = new bool[(int)SettingTypes.COUNT];
        foreach (Setting setting in CurrentFeatureGroup.CurrentFeature().Settings)
        {
            settingTypesUsed[(int)setting.GetSettingType()] = true;
        }
        return settingTypesUsed;
    }
   public void GetCurrentFeatureInfo()
    {
        if (CurrentFeatureGroup != null)
        {
            string CurrentName = CurrentFeatureGroup.Name;
            string NewName = FeatureNameInput.text;
            if (CurrentName != NewName)
            {
                FeatureManager.RenameFeatureGroup(CurrentFeatureGroup, NewName);
            } 
            CurrentFeatureGroup.Description = FeatureDetailsInput.text;
            CurrentFeatureGroup.CurrentFeature().Description = FeatureVersionDetailsInput.text; 
            FeatureManager.FeaturesHaveChanges = true;
            FeatureManager.SaveFeatureGroupToJSON(CurrentFeatureGroup);             
        }
    }
    
    public static void SetCurrentFeature(string _featureName) //get mapped feature 
    { 
        CurrentFeatureGroup = FeatureManager.GetFeatureGroup(_featureName); 
        FeatureManager.FeaturesHaveChanges = true;
    }
    public static void SetFeatureActive(string _featureName, bool _active)
    { 
        FeatureManager.FeaturesHaveChanges = true;
    }
    public  void SetNextFeatureVersion(int _add)
    {
        if (CurrentFeatureGroup.IsLastVersion()&&_add>0)
        {
            CurrentFeatureGroup.CreateNewVersion();
        }
        CurrentFeatureGroup.AddCurrentVersion(_add);
        FeatureManager.FeaturesHaveChanges = true;
        FeatureManager.SaveFeatureGroupToJSON(CurrentFeatureGroup);
    }
    public void DeleteCurrentVersion()
    {
        if (CurrentFeatureGroup != null)
        {
            CurrentFeatureGroup.DeleteCurrentVersion();
            FeatureManager.FeaturesHaveChanges = true;
            FeatureManager.SaveFeatureGroupToJSON(CurrentFeatureGroup);
        }
    }
    public static void AddSetting(Setting _setting) 
    {
        CurrentFeatureGroup.CurrentFeature().Settings.Add(_setting);
        FeatureManager.FeaturesHaveChanges = true;
        FeatureManager.SaveFeatureGroupToJSON(CurrentFeatureGroup);
    }

    internal static void DeleteSetting(Setting _setting)
    {
        CurrentFeatureGroup.CurrentFeature().Settings.Remove(_setting);
        FeatureManager.FeaturesHaveChanges = true;
        FeatureManager.SaveFeatureGroupToJSON(CurrentFeatureGroup);
    }
    public void DeleteCurrentFeature()
    {
        if (CurrentFeatureGroup != null)
        {
            string[] featureNames = FeatureManager.GetFeatureGroupNames();
            string currentName = CurrentFeatureGroup.Name;
            FeatureManager.DeleteFeatureGroup(CurrentFeatureGroup);
             
            int currentIndex = Array.IndexOf(featureNames, currentName);
            int newIndex = (featureNames.Length > 1) ? Math.Max(0, currentIndex - 1) : -1; 
            if (newIndex >= 0)
            {
                CurrentFeatureGroup = FeatureManager.GetFeatureGroup(featureNames[newIndex]);
            }
            else
            {
                CurrentFeatureGroup = null;  
            }

            FeatureManager.FeaturesHaveChanges = true;
        }
    }
}
