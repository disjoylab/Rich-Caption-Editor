using System.Collections;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;

public class CommitToServer : MonoBehaviour
{
    public void CommitToServerButton()
    {
        // Convert the CurrentRCEProject object to a JSON string
        string jsonString = JsonUtility.ToJson(ProjectManager.Instance.CurrentRCEProject);
        string fileName = "TestFile2.json"; //TODO, meaningful name
        // Start a new coroutine to check for file existence and commit to GitHub
        StartCoroutine(CheckAndSendToGitHub(jsonString, fileName));
    }

    private IEnumerator CheckAndSendToGitHub(string jsonString, string fileName)
    {
        string repo = "disjoylab/RCE-User-Projects";  // Replace with your GitHub repo
        string path = "RCE-User-Projects/User_Projects.json";  // Make sure to include the .json extension for your file
        string branch = "main";                       // Branch where you want to commit
        string token = "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";  // Replace with your GitHub token

        // URL to check if the file exists
        string url = $"https://api.github.com/repos/disjoylab/RCE-User-Projects/contents/User_Projects/{fileName}";
        Debug.Log(url);

        // Check if the file exists and get the SHA
        string sha = null;
        yield return StartCoroutine(GetFileSha(url, (retrievedSha) => sha = retrievedSha, token));

        // Now commit to GitHub (send the JSON with the retrieved SHA if exists)
        yield return StartCoroutine(SendToGitHub(url,jsonString,sha, branch, token));
    }

    private IEnumerator SendToGitHub(string url, string jsonString, string sha, string branch, string token) //
    {
        // Encode the JSON string in base64 (required for GitHub)
        string base64Json = System.Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonString));

        // Construct the request body
        string requestBody;

        if (string.IsNullOrEmpty(sha))
        {
            // File does not exist, create a new file
            requestBody = $"{{\"message\":\"Creating new file\",\"content\":\"{base64Json}\",\"branch\":\"{branch}\"}}";
        }
        else
        {
            // File exists, update the file with the SHA
            requestBody = $"{{\"message\":\"Updating file\",\"content\":\"{base64Json}\",\"sha\":\"{sha}\",\"branch\":\"{branch}\"}}";
        }

        // Create UnityWebRequest for HTTP PUT
        UnityWebRequest request = new UnityWebRequest(url, "PUT");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(requestBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", $"token {token}");
        request.SetRequestHeader("Content-Type", "application/json");

        // Send request and wait for response
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("JSON successfully uploaded to GitHub.");
        }
        else
        {
            Debug.LogError($"Error uploading to GitHub: {request.error}");
            Debug.LogError($"Response: {request.downloadHandler.text}");
        }
    }

    private IEnumerator GetFileSha(string url, System.Action<string> callback, string token)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Authorization", $"token {token}");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // Parse the response JSON to extract the `sha`
            var jsonResponse = request.downloadHandler.text;
            var jsonObj = JsonUtility.FromJson<GitHubFileResponse>(jsonResponse);
            callback(jsonObj.sha);
        }
        else if (request.responseCode == 404)
        {
            // File doesn't exist, callback with null
            Debug.Log("File does not exist, creating new one.");
            callback(null);
        }
        else
        {
            Debug.LogError($"Error checking file existence: {request.error}");
            callback(null);
        }
    }

    [System.Serializable]
    public class GitHubFileResponse
    {
        public string sha;
    }
}

