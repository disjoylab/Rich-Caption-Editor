
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Collections;
using UnityEngine.Networking;

// Manages the project loading, saving, and importing VTTs or SRTs  

public class ProjectManager : MonoBehaviour
{
    public static ProjectManager Instance;
    public RCEProject CurrentRCEProject;  
    public string FileFolder; 
    public FileInfo fileInfo;

    public static bool ProjectHasChanges;
    public static Action ProjectChanged; //simple way to update UI elements once after any change, may wantto make this more granular
    public Color[] CueGroupColors;
        
    private void Awake()
    {
        Instance = this;
    }
    void Update()
    {                
        if (ProjectHasChanges)
        {
            ProjectHasChanges = false;
            ProjectChanged?.Invoke(); 
        }
    }

   public void LoadProjectFromLocalFile(string _fileFolder, string _fileName)
    {
        if (!_fileName.EndsWith(".json"))
        {
            Debug.LogError($"{_fileName} requires .json extension");
            return;
        }

        string filePath = Path.Combine(_fileFolder, _fileName);
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            ParseJson(_fileName, json);
        }
        else
        {
            Debug.LogError($"JSON file not found at path: {filePath}");
        }
    }

    internal void LoadProjectFromURL(string jsonFileUrl)
    {
        StartCoroutine(LoadJsonFromServer(jsonFileUrl)); 
    }
    public IEnumerator LoadJsonFromServer(string jsonFileUrl)
    {
        // Use UnityWebRequest to fetch the JSON file from the server
        UnityWebRequest jsonRequest = UnityWebRequest.Get(jsonFileUrl);
        yield return jsonRequest.SendWebRequest();

        if (jsonRequest.result == UnityWebRequest.Result.Success)
        {
            string json = jsonRequest.downloadHandler.text;
            ParseJson(Path.GetFileName( jsonFileUrl), json); 
        }
        else
        {
            Debug.LogError($"Failed to load JSON from server: {jsonRequest.error}");
        }
    }

    private void ParseJson(string _fileName, string json)
    {
        CurrentRCEProject = JsonConvert.DeserializeObject<RCEProject>(json);
        CurrentRCEProject.FileName = _fileName;
        if (string.IsNullOrWhiteSpace(CurrentRCEProject.ProjectName))
        {
            CurrentRCEProject.ProjectName = CurrentRCEProject.FileName.Replace(".json", "");
        }
        CurrentRCEProject.ConvertCueStringsToCueChars();
        LoadVideoFile(CurrentRCEProject.VideoFile);
        ProjectHasChanges = true;
    }

    public void SaveProject (string _fileFolder, string _fileName)
    {
        CurrentRCEProject.ConvertCueCharsToCueStrings();
        CurrentRCEProject.FileName = _fileName;
        string filePath = Path.Combine(_fileFolder, _fileName);         
        string json = JsonConvert.SerializeObject(CurrentRCEProject, Formatting.Indented);
        File.WriteAllText(filePath, json);
        if (string.IsNullOrEmpty(CurrentRCEProject.ProjectName))
        {
            CurrentRCEProject.ProjectName = _fileName.Replace(".json", "");
            ProjectHasChanges = true;
        } 
    }

    //Modified to import Current CueGroup Can be changed back to import entire project or add toggle to do either
    public void ImportFileFromVTTorSRT(string _fileFolder, string _fileName) 
    {
        if (!(_fileName.EndsWith(".vtt") || _fileName.EndsWith(".srt")))
        {
            Debug.LogError($"{_fileName} requires .vtt or .srt extension");
        }
        string filePath = Path.Combine(_fileFolder, _fileName);
        var ImportRCEProject = VTTParser.LoadVTT(filePath);
        CueGroup importedCueGroup = ImportRCEProject.GetCurrentCueGroup();
        importedCueGroup.Name = CurrentRCEProject.GetCurrentCueGroup().Name;
        CurrentRCEProject.SetCurrentCueGroup(importedCueGroup);
        CurrentRCEProject.AddMapping(ImportRCEProject.GetCurrentStyleGroup()); 
        ProjectHasChanges = true;
    }

    internal void LoadVideoFile(string _fileName)
    {
        if (CurrentRCEProject!=null)
        {
            CurrentRCEProject.VideoFile = _fileName; 
        }
        VideoManager.LoadVideo(_fileName);
    }
    public void NewProject()
    {
        CurrentRCEProject = new RCEProject("Speech"); 
        CurrentRCEProject.FileName = "New Project";        
    }
    public static Color GetCueGroupColor(int index)
    {
        return Instance.CueGroupColors[index % Instance.CueGroupColors.Length];

    }

}
