using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoFileLoader : MonoBehaviour
{
    public TMPro.TMP_Dropdown FileURL_Dropdown;//keep value 0 as blank
    private Dictionary<string, string> fileUrlDictionary;

    void Start()
    {
        // Initialize the dictionary with label-URL pairs
        fileUrlDictionary = new Dictionary<string, string>
        {
            { "Select Demo To Load...", "" },
            { "Action Demo", "https://ccrma.stanford.edu/~lloyd/RCE/Action-Gundam_Requiem_for_Vengeance.json" }
        };

        // Populate the dropdown with labels only
        FileURL_Dropdown.ClearOptions();
        List<string> labels = new List<string>(fileUrlDictionary.Keys);
        FileURL_Dropdown.AddOptions(labels);
    }

    public void LoadFile()
    {
        // Prevent processing if no selection is made
        if (FileURL_Dropdown.value == 0) return;

        // Retrieve the label text selected in the dropdown
        string selectedLabel = FileURL_Dropdown.options[FileURL_Dropdown.value].text;

        // Get the URL associated with this label
        if (fileUrlDictionary.TryGetValue(selectedLabel, out string url))
        {
            // Process the URL
            FileMenu.ProcessFile(url);
        }
        else
        {
            Debug.LogError("Selected label does not have an associated URL.");
        }
    }
}
