 
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
    public TMP_InputField TMP_ProjectFile_Input;
    public TMP_InputField TMP_VideoFile_Input;
    public TMP_InputField TMP_LoadURL_Input;

    public static string FileFolder
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
        TMP_ProjectName_Input.SetTextWithoutNotify(ProjectManager.Instance.CurrentRCEProject.ProjectName);
        TMP_Details_Input.SetTextWithoutNotify(ProjectManager.Instance.CurrentRCEProject.Details);
        TMP_ProjectFile_Input.SetTextWithoutNotify(ProjectManager.Instance.CurrentRCEProject.FileName);        
        TMP_VideoFile_Input.SetTextWithoutNotify(ProjectManager.Instance.CurrentRCEProject.VideoFile);
    }

    public void GetProjectInfo()
    {
        if (ProjectManager.Instance.CurrentRCEProject == null) return; 
        ProjectManager.Instance.CurrentRCEProject.ProjectName = TMP_ProjectName_Input.text;
        ProjectManager.Instance.CurrentRCEProject.Details = TMP_Details_Input.text;         
        ProjectManager.ProjectHasChanges = true;
    }

    public void OpenFilePicker()
    {
        ExtensionFilter[] extensions = new[] {
            new ExtensionFilter("RCE Files", "json")
        };
         
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open File", FileFolder, extensions, false);

        if (paths.Length > 0)
        {
            ProcessFile(paths[0]);
            DisplayProjectInfo();
        }
    }
       
    public static void ImportFilePicker()
    {
        ExtensionFilter[] extensions = new[] { 
            new ExtensionFilter("WebVTT Files", "vtt"),
            new ExtensionFilter("SRT Files", "srt")
        };
         
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open File", FileFolder, extensions, false);

        if (paths.Length > 0)
        {
            ProcessFile(paths[0]);
        }
    }

    public void LinkVideoPicker()
    {
        ExtensionFilter[] extensions = new[] {
            new ExtensionFilter("MP4 Files", "mp4")
        };
         
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Link Video", FileFolder, extensions, false);

        if (paths.Length > 0)
        {
            string FilePath = "file://" + paths[0].Replace("\\", "/");
            ProcessFile(FilePath);
            DisplayProjectInfo();
        }
    }

    public void SaveFile()
    {
        if (ProjectManager.Instance.CurrentRCEProject == null)
        {
            return;
        }
        ExtensionFilter[] extensions = new[] {
            new ExtensionFilter("RCE Files", "json")
        };
        string fileFolder = FileFolder;         
       string path = StandaloneFileBrowser.SaveFilePanel("Save Project", fileFolder, ProjectManager.Instance.CurrentRCEProject.FileName,extensions);

        if (!string.IsNullOrEmpty(path))
        {
            FileFolder = Path.GetDirectoryName(path);
            string fileName = Path.GetFileName(path);
            ProjectManager.Instance.SaveProject(FileFolder, fileName);//future get folder and possibly new file name or add SaveAs
            DisplayProjectInfo();
        }      
    }

    public void NewFile()
    {
        ProjectManager.Instance.NewProject();
        SaveFile();
    }

    private static void ProcessFile(string _filePath)
    {
        string fileFolder = Path.GetDirectoryName(_filePath);
        string fileName = Path.GetFileName(_filePath);
        string extension = Path.GetExtension(_filePath).ToLower();
        bool isURL = _filePath.ToLower().StartsWith("https://");

        switch (extension)
        {
            case ".json":               
                if (isURL)
                {
                    ProjectManager.Instance.LoadProjectFromURL(_filePath);
                }
                else
                {
                    FileFolder = fileFolder;
                    ProjectManager.Instance.LoadProjectFromLocalFile(fileFolder, fileName);
                }
                break;
            case ".vtt":
            case ".srt":
                ProjectManager.Instance.ImportFileFromVTTorSRT(fileFolder, fileName);
                break;
            case ".mp4":
                ProjectManager.Instance.LoadVideoFile(_filePath);//use the full path for videos
                break;
            default:
                Debug.LogError("Unsupported file format");
                break;
        }
    }
    public void LoadURL()
    {
        string URL = TMP_LoadURL_Input.text;

        if (IsValidURL(URL))
        {
            ProcessFile(URL);
            DisplayProjectInfo();
        }
        else
        {
            Debug.Log($"Invalid URL");
        }
    }

    private bool IsValidURL(string url)
    { 
        // Check if the URL is empty or null
        if (string.IsNullOrWhiteSpace(url))
        {
            Debug.Log( "URL cannot be empty.");
            return false;
        }

        // Check if the URL is well-formed
        if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
        {
            Debug.Log("URL is not well-formed.");
            return false;
        }

        // Create a Uri object for further analysis
        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult))
        {
            Debug.Log("URL is invalid.");
            return false;
        }        

        // Check if the URL ends with .json or .mp4
        if (!(uriResult.AbsolutePath.EndsWith(".json", StringComparison.OrdinalIgnoreCase) ||
              uriResult.AbsolutePath.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase)))
        {
            Debug.Log("URL must end with '.json' or '.mp4'.");
            return false;
        }

        return true;
    }

}
