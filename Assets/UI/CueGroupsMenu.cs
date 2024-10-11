using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CueGroupsMenu : MonoBehaviour
{
    public static CueGroupsMenu Instance;
    //set current cue group, display cuegroup order, tell current cuegroup to update display

    public List<CueGroupUI> CueGroupUIs;
    public GameObject CueGroupUIPrefab;

    public static bool CueGroupsHaveChanges;
    public static Action CueGroupsChanged;

    public Color[] TabColors;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ProjectManager.ProjectChanged += OnProjectChanged;
        MenuManager.CurrentMenuStateChanged += OnCurrentMenuStateChanged;
    }

    private void OnDestroy()
    {
        ProjectManager.ProjectChanged -= OnProjectChanged;
        MenuManager.CurrentMenuStateChanged -= OnCurrentMenuStateChanged;
    }
    private void Update()
    {
        if (CueGroupsHaveChanges)
        {
            CueGroupsHaveChanges = false;
            CueGroupsChanged?.Invoke();
        }
    }
    private void OnProjectChanged()
    {
        CreateCueGroups();
    }

    private void OnCurrentMenuStateChanged(MenuStates _menuState)
    {
        if (_menuState == MenuStates.CueGroup)
        {
           //
        }
    }

    private void CreateCueGroups()
    {
        if (ProjectManager.Instance == null)
        {
            return;
        }

        var currentCueGroups = ProjectManager.Instance.CurrentRCEProject.CueGroups;
        int count = currentCueGroups.Count;

        for (int i = CueGroupUIs.Count; i < count + 1; i++)
        {
            GameObject go = Instantiate(CueGroupUIPrefab, transform);
            CueGroupUI cg = go.GetComponent<CueGroupUI>();
            CueGroupUIs.Add(cg);
        }

        // Configure each UI element
        for (int i = 0; i < count + 1; i++)
        {
            if (i < count)
            {
                CueGroupUIs[i].Configure(currentCueGroups[i], i,count + 1,TabColors[i%TabColors.Length]);
            }
            else
            {
                CueGroupUIs[i].Configure(null, i, count + 1,Color.white);
            }
        }

        // Remove any excess CueGroupUIs
        for (int i = CueGroupUIs.Count - 1; i >= count + 1; i--)
        {
            Destroy(CueGroupUIs[i].gameObject);
            CueGroupUIs.RemoveAt(i);
        }
         

        int currentIndex = ProjectManager.Instance.CurrentRCEProject.GetCurrentCueGroupIndex();
        CueGroupUIs[currentIndex].transform.SetAsLastSibling();
    }

    internal void DeleteCueGroup(CueGroup _cueGroup)
    {
        ProjectManager.Instance.CurrentRCEProject.CueGroups.Remove(_cueGroup);
        CreateCueGroups();
        CueGroupsHaveChanges = true;
    }

    public void SetCurrentCueGroup(int _index)
    {
        if (_index>= ProjectManager.Instance.CurrentRCEProject.CueGroups.Count)
        {
            var currentCueGroup = ProjectManager.Instance.CurrentRCEProject.GetCurrentCueGroup();
            if (currentCueGroup==null)
            {
                ProjectManager.Instance.CurrentRCEProject.CueGroups.Add(new CueGroup("New"));
            }
            else
            {
                var newCueGroup = new CueGroup(currentCueGroup);
                newCueGroup.Name += "_copy";
                ProjectManager.Instance.CurrentRCEProject.CueGroups.Add(newCueGroup);
            }
            CreateCueGroups();
        }
        ProjectManager.Instance.CurrentRCEProject.SetCurrentCueGroupIndex( _index);
        CueGroupUIs[ProjectManager.Instance.CurrentRCEProject.GetCurrentCueGroupIndex()].transform.SetAsLastSibling();
        CueGroupsChanged?.Invoke();
    }
    public CueGroupUI GetCurrentCueGroupUI()
    {
        if (CueGroupUIs == null || ProjectManager.Instance.CurrentRCEProject.CueGroups.Count==0)
        {
            return null;
        }
        if (CueGroupUIs.Count > ProjectManager.Instance.CurrentRCEProject.GetCurrentCueGroupIndex())
        {
            return CueGroupUIs[ProjectManager.Instance.CurrentRCEProject.GetCurrentCueGroupIndex()];
        }
        return null;
    }
}
