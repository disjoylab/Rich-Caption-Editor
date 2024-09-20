 
using UnityEngine;
using TMPro;
using System; 
using System.IO;
using SFB;

public class FileMenu : MonoBehaviour
{
    public TextMeshProUGUI TMP_ProjectName;
    public TMP_InputField TMP_ProjectName_Input;
    public TMP_InputField TMP_Details_Input;

    public string FileFolder
    {
        get
        {
            return PlayerPrefs.GetString("FileFolder", Path.Combine(UnityEngine.Application.dataPath, "StreamingAssets"));
        }
        set
        {
            PlayerPrefs.SetString("FileFolder", value);
            PlayerPrefs.Save(); 
        }
    }

    private void Start()
    {
        ProjectManager.ProjectChanged += OnProjectChanged;
    }
    private void OnDestroy()
    {
        ProjectManager.ProjectChanged -= OnProjectChanged;
    }

    private void OnProjectChanged()
    { 
        DisplayProjectInfo();
    }

    private void DisplayProjectInfo()
    {
        TMP_ProjectName.text = ProjectManager.Instance.CurrentRCEProject.ProjectName;
        TMP_ProjectName_Input.text = ProjectManager.Instance.CurrentRCEProject.ProjectName;
        TMP_Details_Input.text = ProjectManager.Instance.CurrentRCEProject.Details;
    }
    public void GetProjectInfo()
    {
        ProjectManager.Instance.CurrentRCEProject.ProjectName = TMP_ProjectName_Input.text;
        ProjectManager.Instance.CurrentRCEProject.Details = TMP_Details_Input.text;
        ProjectManager.ProjectHasChanges = true;
    }
    public void OpenFilePicker()
    {
        ExtensionFilter[] extensions = new[] {
            new ExtensionFilter("RCE Files", "json")
        };

        // Open file panel with filters and initial directory
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open File", FileFolder, extensions, false);

        if (paths.Length > 0)
        {
            ProcessFile(paths[0]);
        }
    }
    public void ImportFilePicker()
    {
        ExtensionFilter[] extensions = new[] { 
            new ExtensionFilter("WebVTT Files", "vtt"),
            new ExtensionFilter("SRT Files", "srt")
        };

        // Open file panel with filters and initial directory
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open File", FileFolder, extensions, false);

        if (paths.Length > 0)
        {
            ProcessFile(paths[0]);
        }
    }
    public void SaveFile()
    {
        ProjectManager.Instance.SaveProject();//future get folder and possibly new file name or add SaveAs
    }

    // Process the selected file
    private void ProcessFile(string filePath)
    {
        string fileFolder = Path.GetDirectoryName(filePath);
        string fileName = Path.GetFileName(filePath);

        string extension = Path.GetExtension(filePath).ToLower();
        switch (extension)
        {
            case ".json":
                ProjectManager.Instance.LoadProjectFromJSON(fileFolder, fileName);
                break;
            case ".vtt":
            case ".srt":
                ProjectManager.Instance.LoadFileFromVTTorSRT(fileFolder, fileName);
                break;
            default:
                Debug.LogError("Unsupported file format");
                break;
        }
    }
     

}
