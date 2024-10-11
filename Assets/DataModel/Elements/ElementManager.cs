using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ElementManager : MonoBehaviour
{
    static List<ElementGroup> ElementGroups = new List<ElementGroup>();

    public static Action ElementsChanged;

    public static bool ElementsHaveChanges;
    static string FilePath
    {
        get => PlayerPrefs.GetString("FeatureFilePath", Application.persistentDataPath);
        set => PlayerPrefs.SetString("FeatureFilePath", value);
    }
    private void Awake()
    {
        LoadAllElementGroups();
    }
    private void Update()
    {
        if (ElementsHaveChanges)
        {
            //Debug.Log("elements have changes");
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

    public void LoadAllElementGroups()
    {
        ElementGroups = new List<ElementGroup>();
        if (Directory.Exists(FilePath))
        {
            foreach (var file in Directory.GetFiles(FilePath, "*.element"))
            {
                ElementGroup elementGroup = LoadElementGroupFromJSON(Path.GetFileName(file));
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
     
    public static ElementGroup LoadElementGroupFromJSON(string fileName)
    {
        string fullPath = Path.Combine(FilePath, fileName);
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
        string currentFullPath = Path.Combine(FilePath, currentFileName);
        string newFullPath = Path.Combine(FilePath, newFileName);
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
        string fullPath = Path.Combine(FilePath, _elementGroup.ToFileName());
        File.WriteAllText(fullPath, json);
    }
    public static void DeleteFeatureGroupJsonFile(FeatureGroup _featureGroup)
    {
        string fullPath = Path.Combine(FilePath, _featureGroup.ToFileName());
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
