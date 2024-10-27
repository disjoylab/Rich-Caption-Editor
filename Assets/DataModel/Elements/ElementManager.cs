using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class ElementManager : MonoBehaviour
{
    static List<ElementGroup> ElementGroups = new List<ElementGroup>();

    public static Action ElementsChanged;
    public static bool ElementsHaveChanges;
    private readonly string baseURL = "https://redarmy34.github.io/RCE-WebGL-Page/StreamingAssets/";



    private void Start()
    {
        ElementGroups = new List<ElementGroup>();

        if (WebGLManager.WebGL_Build)
        {
            StartCoroutine(LoadDefaultElementsFromServer());
        }
        else
        {
            LoadAllElementGroups(Common.FeatureFilePath);
            LoadAllElementGroups(Common.FeatureFilePath_DEFAULT);//only load defaults if saved versions do not exist
        }       
    }

    private ElementGroup LoadElementGroupFromJSON_WebGL(string jsonContent)
    {
        // Deserialize JSON string to FeatureGroup object
        // Implement this method based on your JSON structure
        ElementGroup elementGroup = JsonConvert.DeserializeObject<ElementGroup>(jsonContent);
        return elementGroup;
        //return JsonUtility.FromJson<ElementGroup>(jsonContent);
    }
    public IEnumerator LoadDefaultElementsFromServer()
    {
        Debug.Log("element loaded");
        // List of feature files to load (add all the .feature files you need to load here)
        List<string> featureFiles = new List<string>
            {
                //".element",
                "B.element",
                "Default.element",
                "Emphasis.element",
                "i.element",
                "Narrative Importance.element",
                "s.element",
                "Soft background.element",
                "Speaker.element",
                "u.element",
                "Visual Redundancy.element"
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
                    ElementGroup elementGroup = LoadElementGroupFromJSON_WebGL(jsonContent);

                    if (elementGroup != null && !ElementGroupExists(elementGroup.Name))
                    {
                        ElementGroups.Add(elementGroup);
                    }
                }
                else
                {
                    Debug.LogError($"Failed to load {fileName}: {request.error}");
                }
            }
        }

        // Add default Elements manually
        CreateElementGroup("Default", false, false);
        CreateElementGroup("B", false, true);
        CreateElementGroup("i", false, true);
        CreateElementGroup("u", false, true);
        CreateElementGroup("s", false, true);        
        ElementsChanged?.Invoke();
    }

    private void Update()
    {
        if (ElementsHaveChanges)
        { 
            ElementsHaveChanges = false;
            ElementsChanged?.Invoke();
        }
    }

    public static List<string> GetAllElementGroupNames(bool _onlyActive)
    {
        List<string> elements = new List<string>();
        foreach (var elementGroup in ElementGroups)
        {
            bool isActiveCondition = !_onlyActive || elementGroup.Active;
            if (isActiveCondition )
            {
                elements.Add(elementGroup.Name);
            }             
        }
        return elements;
    }

    internal static List<string> GetCueLevelElementGroupNames()
    {
        List<string> elements = new List<string>();
        foreach (var elementGroup in ElementGroups)
        { 
            if (elementGroup.Active && (elementGroup.CurrentDefinition().CueLevel))
            {
                elements.Add(elementGroup.Name);
            }
        }
        return elements;
    }

    internal static List<string> GetTextLevelElementGroupNames()
    {
        List<string> elements = new List<string>();
        foreach (var elementGroup in ElementGroups)
        { 
            if (elementGroup.Active && (elementGroup.CurrentDefinition().TextLevel))
            {
                elements.Add(elementGroup.Name);
            }
        }
        return elements;
    }

    public void LoadAllElementGroups(string _filepath)
    {
        if (Directory.Exists(_filepath))
        {
            foreach (var file in Directory.GetFiles(_filepath, "*.element"))
            {
                ElementGroup elementGroup = LoadElementGroupFromJSON(_filepath,Path.GetFileName(file));
                if (elementGroup != null && !ElementGroupExists(elementGroup.Name))
                {
                    ElementGroups.Add(elementGroup);
                }
            }
        }
        CreateElementGroup("B",false, true);
        CreateElementGroup("i", false, true);
        CreateElementGroup("u", false, true);
        CreateElementGroup("s", false, true);
        CreateElementGroup("Default", false, false);
        ElementsChanged?.Invoke();
    }
     
    public static ElementGroup CreateElementGroup(String _name, bool _cueLevel, bool _textLevel)
    {
        ElementGroup elementGroup = GetElementGroup(_name);
        if (elementGroup == null)
        {
            elementGroup = new ElementGroup(_name,_cueLevel, _textLevel);
            AddElementGroup(elementGroup); 
        }
        return elementGroup;
    }
     
    public static ElementGroup LoadElementGroupFromJSON(string _filepath, string _fileName)
    {
        string fullPath = Path.Combine(_filepath, _fileName);
        if (File.Exists(fullPath))
        {
            string json = File.ReadAllText(fullPath);
            ElementGroup elementGroup = JsonConvert.DeserializeObject<ElementGroup>(json);
            return elementGroup;
        }
        return null;
    }
     
    internal static void RenameElementGroup(ElementGroup _elementGroup, string _name)
    {
        
    }
    
    internal static void DeleteElementGroup(ElementGroup _elementGroup)
    {
        if (_elementGroup == null)
        {
            return;
        }

        string currentFileName = _elementGroup.ToFileName();
        string newFileName = currentFileName + ".delete";
        string currentFullPath = Path.Combine(Common.FeatureFilePath, currentFileName);
        string newFullPath = Path.Combine(Common.FeatureFilePath, newFileName);
        if (File.Exists(currentFullPath))
        {
            if (File.Exists(newFullPath))
            {
                File.Delete(newFullPath);
            }
            File.Move(currentFullPath, newFullPath);
        }
        ElementGroups.Remove(_elementGroup);
    }

    public static void AddElementGroup(ElementGroup _elementGroup)
    {         
        ElementGroup existingElementGroup = GetElementGroup(_elementGroup.Name); // Possibly combine them with matching under specific cicumstances?
        if (existingElementGroup != null)
        {
            ElementGroups.Remove(existingElementGroup);
        }
        ElementGroups.Add(_elementGroup);
        SaveElementGroupToJSON(_elementGroup);
        ElementsChanged?.Invoke();
    }

    public static void SaveElementGroupToJSON(ElementGroup _elementGroup)
    {
        string json = JsonConvert.SerializeObject(_elementGroup, Formatting.Indented);
        string fullPath = Path.Combine(Common.FeatureFilePath, _elementGroup.ToFileName());
        File.WriteAllText(fullPath, json);
    }

    public static void DeleteFeatureGroupJsonFile(FeatureGroup _featureGroup)
    {
        string fullPath = Path.Combine(Common.FeatureFilePath, _featureGroup.ToFileName());
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }
 
    internal static ElementGroup GetElementGroup(string _name)
    {
        return ElementGroups.Find(eg => eg.Name.ToLower() == _name.ToLower());
    }
    internal static bool ElementGroupExists(string _name)
    {
        return ElementGroups.Exists(e => e.Name.ToLower() == _name.ToLower());
    }

    internal static string GetUniqueName(string _root)
    {
        if (!ElementGroups.Exists(e=> e.Name == _root))
        {
            return _root;
        }

        int index = 2;
        string uniqueName = "";
        bool exists = true;
        while (exists)
        {
            uniqueName = $"{_root} {index++}";
            exists = ElementGroups.Exists(f => f.Name == uniqueName);
        }
        return uniqueName;
    }
}
