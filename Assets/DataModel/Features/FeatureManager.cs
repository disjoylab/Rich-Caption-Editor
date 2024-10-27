using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using static System.Net.WebRequestMethods;
using System.Collections;
using UnityEngine.Networking;

public class FeatureManager : MonoBehaviour
{
    static List<FeatureGroup> FeatureGroups = new List<FeatureGroup>(); 

    public static Action FeaturesChanged; 
    public static bool FeaturesHaveChanges;
    private readonly string baseURL = "https://redarmy34.github.io/RCE-WebGL-Page/StreamingAssets/";



    private void Start()
    {
        FeatureGroups = new List<FeatureGroup>();

        if (WebGLManager.WebGL_Build)
        {
            StartCoroutine(LoadDefaultFeaturesFromServer());
        }
        else
        {
            LoadAllFeatureGroups(Common.FeatureFilePath);
            LoadAllFeatureGroups(Common.FeatureFilePath_DEFAULT);//only load defaults if saved versions do not exist
        }
        
    }

    private FeatureGroup LoadFeatureGroupFromJSON_WebGL(string jsonContent)
    {
        // Deserialize JSON string to FeatureGroup object
        // Implement this method based on your JSON structure
        return JsonUtility.FromJson<FeatureGroup>(jsonContent);
    }
    public IEnumerator LoadDefaultFeaturesFromServer()
    {
        // List of feature files to load (add all the .feature files you need to load here)
        List<string> featureFiles = new List<string> 
            {
                "Blue Text.feature",
                "Bold.feature",
                "Default Feature.feature",
                "Defaults.feature",
                "Green Text.feature",
                "Grey Text.feature",
                "Italic.feature",
                "Long background box.feature",
                "New Feature.feature",
                "Red Text.feature",
                "Red, Bold, Underlined, Text.feature",
                "Region Top.feature",
                "Strikethrough.feature",
                "Underline.feature"
            };


        foreach (var fileName in featureFiles)
        {
            string fileURL = baseURL + fileName;

            using (UnityWebRequest request = UnityWebRequest.Get(fileURL))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string jsonContent = request.downloadHandler.text;
                    FeatureGroup featureGroup = LoadFeatureGroupFromJSON_WebGL(jsonContent);

                    if (featureGroup != null && !FeatureGroups.Exists(f => f.Name == featureGroup.Name))
                    {
                        FeatureGroups.Add(featureGroup);
                    }
                }
                else
                {
                    Debug.LogError($"Failed to load {fileName}: {request.error}");
                }
            }
        }

        // Add default features manually as before
        AddDefaultFeature("Bold", new List<Setting> { new Setting("Style_FontWeight", "bold") });
        AddDefaultFeature("Italic", new List<Setting> { new Setting("Style_FontStyle", "italic") });
        AddDefaultFeature("Underline", new List<Setting> { new Setting("Style_TextDecoration", "underline") });
        AddDefaultFeature("Strikethrough", new List<Setting> { new Setting("Style_TextDecoration", "line-through") });
        AddDefaultFeature("Default Feature", new List<Setting>
        {
            new Setting("Style_FontSize", "12px"),
            new Setting("Region_Width", "90%"),
            new Setting("Region_Lines", "3"),
            new Setting("Region_RegionAnchor", "50%,100%"),
            new Setting("Region_viewportAnchor", "50%,90%"),
            new Setting("Region_scroll", "up")
        });

        FeaturesChanged?.Invoke();
    }

    private void Update()
    {
        if (FeaturesHaveChanges)
        { 
            FeaturesHaveChanges = false;
            FeaturesChanged?.Invoke(); 
            
        }
    }

    internal static string[] GetFeatureGroupNames(FeatureFilter _featureFilter = FeatureFilter.ALL, bool _active = false)
    {
        return FeatureGroups
            .Where(f => !_active || f.Active && f.IsFeatureType(_featureFilter))
            .Select(f => f.Name)
            .ToArray();
    }

    public void LoadAllFeatureGroups(string _filepath)
    {
        if (Directory.Exists(_filepath))
        {
            foreach (var file in Directory.GetFiles(_filepath, "*.feature"))
            {
                FeatureGroup featureGroup = LoadFeatureGroupFromJSON(_filepath, Path.GetFileName(file));
                if (featureGroup != null && !FeatureGroups.Exists(f => f.Name == featureGroup.Name))
                {
                    FeatureGroups.Add(featureGroup);
                }
            }
        } 
        AddDefaultFeature("Bold", new List<Setting> { new Setting("Style_FontWeight", "bold") });
        AddDefaultFeature("Italic", new List<Setting> { new Setting("Style_FontStyle", "italic") });
        AddDefaultFeature("Underline", new List<Setting> { new Setting("Style_TextDecoration", "underline") });
        AddDefaultFeature("Strikethrough", new List<Setting> { new Setting("Style_TextDecoration", "line-through") });
        AddDefaultFeature("Default Feature", new List<Setting>
        {
            new Setting("Style_FontSize", "12px"),
            new Setting("Region_Width", "90%"),
            new Setting("Region_Lines", "3"),
            new Setting("Region_RegionAnchor", "50%,100%"),
            new Setting("Region_viewportAnchor", "50%,90%"),
            new Setting("Region_scroll", "up")
        });

        FeaturesChanged?.Invoke();
    }

    private void AddDefaultFeature(string name, List<Setting> settings)
    {
        if (!FeatureGroups.Exists(f => f.Name == name))
        {
            Feature newFeature = new Feature();
            foreach (var setting in settings)
            {
                newFeature.Settings.Add(setting);
            }
            if (newFeature.Settings.Count > 0)
            {
                AddFeatureGroup(new FeatureGroup(name, newFeature));
            }
        }
    }

    internal static string GetUniqueName(string _root)
    {
        if (!FeatureGroups.Exists(f => f.Name == _root))
        {
            return _root;
        }

        int index = 2;
        string uniqueName="";
        bool exists = true;
        while (exists)
        {
            uniqueName = $"{_root} {index++}";
            exists = FeatureGroups.Exists(f => f.Name == uniqueName);
        }
        return uniqueName;
    }

    public static FeatureGroup LoadFeatureGroupFromJSON(string _filepath, string _fileName)
    {
        string fullPath = Path.Combine(_filepath, _fileName);
        if (System.IO.File.Exists(fullPath))
        {
            string json = System.IO.File.ReadAllText(fullPath);
            FeatureGroup featureGroup = JsonConvert.DeserializeObject<FeatureGroup>(json);
            return featureGroup;
        }
        return null; 
    }

    //**************************** TODO ****************** check if the newnmae exists first, return the value so the calling funciton knows it did not work
    internal static void RenameFeatureGroup(FeatureGroup _featureGroup, string _newName)
    {
        string OldFileName = _featureGroup.ToFileName();
        string oldFilePath = Path.Combine(Common.FeatureFilePath, OldFileName);
        if(System.IO.File.Exists(oldFilePath))
        {
            System.IO.File.Delete(oldFilePath);
        }
        _featureGroup.Name = _newName;
        SaveFeatureGroupToJSON(_featureGroup);
    }

    internal static void DeleteFeatureGroup(FeatureGroup _featureGroup)
    {
        if (_featureGroup == null)
        {
            return;
        }
        string currentFileName = _featureGroup.ToFileName();
        string newFileName = currentFileName + ".delete";
        string currentFullPath = Path.Combine(Common.FeatureFilePath, currentFileName);
        string newFullPath = Path.Combine(Common.FeatureFilePath, newFileName); 
        if (System.IO.File.Exists(currentFullPath))
        {
            if(System.IO.File.Exists(newFullPath))
            { 
                System.IO.File.Delete(newFullPath);
            }
            System.IO.File.Move(currentFullPath, newFullPath);
        }
        FeatureGroups.Remove(_featureGroup);
    }

    public static void AddFeatureGroup(FeatureGroup _featureGroup)
    {
        FeatureGroup existingFeatureGroup = GetFeatureGroup(_featureGroup.Name); // Possibly combine them with matching under specific cicumstances?
        if (existingFeatureGroup != null)
        {
            FeatureGroups.Remove(existingFeatureGroup);
        }
        FeatureGroups.Add(_featureGroup);
        SaveFeatureGroupToJSON(_featureGroup);
        FeaturesChanged?.Invoke();
    }

    public static void SaveFeatureGroupToJSON(FeatureGroup _featureGroup)
    {
        string json = JsonConvert.SerializeObject(_featureGroup, Formatting.Indented);  
        string fullPath = Path.Combine(Common.FeatureFilePath, _featureGroup.ToFileName());
        System.IO.File.WriteAllText(fullPath, json);
    }

    public static void DeleteFeatureGroupJsonFile(FeatureGroup _featureGroup)
    { 
        string fullPath = Path.Combine(Common.FeatureFilePath, _featureGroup.ToFileName());
        if (System.IO.File.Exists(fullPath))
        {
            System.IO.File.Delete(fullPath);
        }
    }

    internal static Feature GetFeature(string _name)
    {
        FeatureGroup fg = GetFeatureGroup(_name);
        if (fg!=null)
        {
            return fg.CurrentFeature();
        }
        return null;
    }

    internal static FeatureGroup GetFeatureGroup(string _name)
    {
        if (FeatureGroups == null || _name == null) return null;        
        return FeatureGroups.Find(fg => fg.Name.ToLower() == _name.ToLower());
    } 

    public static FeatureGroup FindMatch(Feature _otherFeature)
    {
        foreach (FeatureGroup featureGroup in FeatureGroups)
        {
            if (featureGroup.IsMatch(_otherFeature))
            {
                return featureGroup;
            }
        }
        return null;
    }
}
