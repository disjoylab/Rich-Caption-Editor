
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System;

public class ProjectManager : MonoBehaviour
{
    public static ProjectManager Instance;
    public RCEProject CurrentRCEProject; 
    public bool Quick_Fix;
    public  string FileFolder; 
    public FileInfo fileInfo;

    public static bool ProjectHasChanges;
    public static Action ProjectChanged; //simple way to update UI elements once after any change
    private void Awake()
    {
        Instance = this;
    }
    void Update()
    {        
        if (Quick_Fix)
        {
            Quick_Fix = false;
            QuickFix();
        }
        if (ProjectHasChanges)
        {
            ProjectHasChanges = false;
            ProjectChanged?.Invoke(); 
        }
    }

   public void LoadProjectFromJSON(string _fileFolder, string _fileName)
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
            CurrentRCEProject = JsonConvert.DeserializeObject<RCEProject>(json);
            CurrentRCEProject.fileName = _fileName.Replace(".json", "");
            if (string.IsNullOrWhiteSpace(CurrentRCEProject.ProjectName))
            {
                CurrentRCEProject.ProjectName = CurrentRCEProject.fileName.Replace(".json", "");
                if (string.IsNullOrWhiteSpace(CurrentRCEProject.ProjectName))
                {
                    CurrentRCEProject.ProjectName = CurrentRCEProject.fileName;
                }
            }
            foreach (var cueGroup in CurrentRCEProject.CueGroups)
            {
                foreach (var cue in cueGroup.Cues)
                {
                    foreach (var textSegment in cue.TextSegments)
                    {
                        textSegment.CueStringToCueChar(); 
                    }
                }
            }
            ProjectHasChanges = true;
        }
        else
        {
            Debug.LogError($"JSON file not found at path: {filePath}");
        }
    }

    public void SaveProject()
    {
        SaveProjectAsJSON(FileFolder);
    }

    void SaveProjectAsJSON(string _fileFolder)
    {
        foreach (var cueGroup in CurrentRCEProject.CueGroups)
        {
            foreach (var cue in cueGroup.Cues)
            {
                foreach (var textSegment in cue.TextSegments)
                {
                    textSegment.CueCharToCueString();
                }
            }
        }

        string filePath = Path.Combine(_fileFolder, CurrentRCEProject.GetFileName(".json"));
         
            string json = JsonConvert.SerializeObject(CurrentRCEProject, Formatting.Indented);
            File.WriteAllText(filePath, json);
         
    }
     
    public void LoadFileFromVTTorSRT(string _fileFolder, string _fileName)
    {
        if (!(_fileName.EndsWith(".vtt") || _fileName.EndsWith(".srt")))
        {
            Debug.LogError($"{_fileName} requires .vtt or .srt extension");
        }
        string filePath = Path.Combine(_fileFolder, _fileName);
        CurrentRCEProject = VTTParser.LoadVTT(filePath);
        CurrentRCEProject.fileName = _fileName.Replace(".vtt", "");
        CurrentRCEProject.fileName = CurrentRCEProject.fileName.Replace(".srt", "");
        if (string.IsNullOrWhiteSpace(CurrentRCEProject.ProjectName))
        {
            CurrentRCEProject.ProjectName = CurrentRCEProject.fileName;
        }
        ProjectHasChanges = true;
    }
    public void QuickFix()
    {


    }
}
