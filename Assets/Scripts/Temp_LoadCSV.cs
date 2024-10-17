using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class Temp_LoadCSV : MonoBehaviour
{
    private string filePath  = "https://ccrma.stanford.edu/~lloyd/Test/test1.mp4";
    private string fileName = "test1.mp4";
    private string fileFolder = "https://ccrma.stanford.edu/~lloyd/Test/"; // Folder where video files are stored
    // Start is called before the first frame update
    public IEnumerator LoadSRT()   
    {
        Debug.Log(filePath);
        UnityWebRequest uwr = UnityWebRequest.Get(filePath);
        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.Success)
        {
            // Use the downloaded video file in your project
            byte[] videoData = uwr.downloadHandler.data;

            // Pass it to your project manager or video loading function
            ProjectManager.Instance.LoadVideoFile(fileFolder, filePath);
        }
        else
        {
            Debug.LogError("Failed to load video: " + uwr.error);
        }
    
    }

}
