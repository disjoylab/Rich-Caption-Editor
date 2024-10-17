using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using Newtonsoft.Json;
using System.IO;

public class DropDownScript : MonoBehaviour
{   
    public TMP_Dropdown videoDropdown; // Reference to the TMP_Dropdown UI
    private string[] baseFileNames = { "test1", "test2" }; // Base file names without extensions
    private string serverUrl = "https://ccrma.stanford.edu/~lloyd/Test/"; // URL of the server

    void Start()
    {
        // Clear current options in case they exist
        videoDropdown.ClearOptions();

        // Convert the array to a List<string> and add new options
        videoDropdown.AddOptions(new List<string>(baseFileNames));

        // Set listener to detect dropdown value changes
        videoDropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(videoDropdown);
        });
    }

    // Function to handle dropdown value changes
    void DropdownValueChanged(TMP_Dropdown change)
    {
        // Get the selected base file name (e.g., "Test1" or "Test2")
        string selectedBaseFile = baseFileNames[change.value];

        Debug.Log("Loading..."); //TODO, do some fun UI things while it's loading

        // Start a new coroutine that handles both file loads
        StartCoroutine(LoadFilesFromServer(selectedBaseFile));
    }

    IEnumerator LoadFilesFromServer(string baseFileName)
    {
        // Start coroutines for both video and JSON loading
        yield return StartCoroutine(LoadJsonFromServer(baseFileName + ".json"));
        yield return StartCoroutine(LoadVideoFromServer(baseFileName + ".mp4"));

        // This will only be called once both coroutines have completed
        Debug.Log("Finished Loading");
    }

    IEnumerator LoadVideoFromServer(string videoFileName)
    {
        // Create the full URL for the video file
        string videoFileUrl = serverUrl + videoFileName;

        // Use UnityWebRequest to get the video file from the server
        UnityWebRequest videoRequest = UnityWebRequest.Get(videoFileUrl);
        yield return videoRequest.SendWebRequest();

        if (videoRequest.result == UnityWebRequest.Result.Success)
        {
            // Pass the video file URL to your project manager's LoadVideoFile function
            ProjectManager.Instance.LoadVideoFile(serverUrl, videoFileName);
        }
        else
        {
            Debug.LogError("Failed to load video: " + videoRequest.error);
        }
    }

    IEnumerator LoadJsonFromServer(string jsonFileName)
    {
        // Create the full URL for the JSON file
        string jsonFileUrl = serverUrl + jsonFileName;

        // Use UnityWebRequest to get the JSON file from the server
        UnityWebRequest jsonRequest = UnityWebRequest.Get(jsonFileUrl);
        yield return jsonRequest.SendWebRequest();

        if (jsonRequest.result == UnityWebRequest.Result.Success)
        {
            // Get the JSON content as a string
            string jsonData = jsonRequest.downloadHandler.text;

            // Pass the JSON file content to the ProcessFile function
            //ProjectManager.Instance.ProcessFile(jsonFileUrl);
            ProjectManager.Instance.LoadJsonFromServer(jsonFileUrl);
        }
        else
        {
            Debug.LogError("IN DROPDOWN Failed to load JSON file: " + jsonRequest.error);
        }
    }
}


