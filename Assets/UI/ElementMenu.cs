using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ElementMenu : MonoBehaviour
{ 
    public ButtonContainer elementsButtonContainer;
    public GameObject ElementButtonPrefab; 
    static ElementGroup CurrentElementGroup;

    public TMP_InputField ElementNameInput;
    public TMP_InputField ElementDetailsInput;
    public TMP_InputField ElementVersionDetailsInput;

    public TMP_Text PrevText;
    public TMP_Text NextText;
    public TMP_Text VersionNumberText;

    void Start()
    {
        ElementManager.ElementsChanged += OnElementsChanged;
        DisplayElements();
    }
    private void OnDestroy()
    {
        ElementManager.ElementsChanged -= OnElementsChanged;
    }
    private void OnEnable()
    {
        DisplayElements();
    }

    private void OnElementsChanged()
    {
        DisplayElements();
    }

    private void DisplayElements()
    {
        string[] _elementNames = ElementManager.GetElementGroupNames();
        elementsButtonContainer.Configure(ElementButtonPrefab, _elementNames.Length + 1);
        for (int i = 0; i < _elementNames.Length; i++)
        {
            elementsButtonContainer.buttons[i].GetComponent<ElementButton>().Configure(_elementNames[i]);
        }
        elementsButtonContainer.buttons[_elementNames.Length].GetComponent<ElementButton>().Configure("");//this is the add feature button


        //CURRENT ELEMENT
        if (CurrentElementGroup == null && _elementNames.Length > 0)
        {
            CurrentElementGroup = ElementManager.GetElementGroup(_elementNames[0]);
        }
        if (CurrentElementGroup != null)
        {
            ElementNameInput.text = CurrentElementGroup.Name;
            ElementDetailsInput.text = CurrentElementGroup.Description;
            var currentElementVersion = CurrentElementGroup.CurrentDefinition();
            if (currentElementVersion != null)
            {
                ElementVersionDetailsInput.text = currentElementVersion.Description; 
                 //other definition properties
            }
            PrevText.enabled = CurrentElementGroup.CurrentVersion > 0;
            NextText.text = CurrentElementGroup.IsLastVersion() ? "+" : ">";
            VersionNumberText.text = CurrentElementGroup.FeatureVersionString();

        }
    }
     
    public void GetCurrentElementInfo()
    {
        if (CurrentElementGroup != null)
        {
            string CurrentName = CurrentElementGroup.Name;
            string NewName = ElementNameInput.text;
            if (CurrentName != NewName)
            {
                // ElementManager.RenameFeatureGroup(CurrentElementGroup, NewName);  ******************************************* TODO
                CurrentElementGroup.Name = NewName;
            }
            CurrentElementGroup.Description = ElementDetailsInput.text;
            CurrentElementGroup.CurrentDefinition().Description = ElementVersionDetailsInput.text;
            ElementManager.ElementsHaveChanges = true;
            ElementManager.SaveElementGroupToJSON(CurrentElementGroup);
        }
    }

    public static void SetCurrentElement(string _elementName)  
    {
        CurrentElementGroup = ElementManager.GetElementGroup(_elementName);
        ElementManager.ElementsHaveChanges = true;
    }
    public static void SetElementActive(string _elementName, bool _active)
    {
        FeatureManager.FeaturesHaveChanges = true;
    }
    public void SetElementNextVersion(int _add)
    {
        if (CurrentElementGroup.IsLastVersion() && _add > 0)
        {
            CurrentElementGroup.CreateNewVersion();
        }
        CurrentElementGroup.AddCurrentVersion(_add);
        ElementManager.ElementsHaveChanges = true;
        ElementManager.SaveElementGroupToJSON(CurrentElementGroup);
    }
    public void DeleteCurrentVersion()
    {
        if (CurrentElementGroup != null)
        {
            CurrentElementGroup.DeleteCurrentVersion();
            ElementManager.ElementsHaveChanges = true;
            ElementManager.SaveElementGroupToJSON(CurrentElementGroup);
        }
    }
    
    public void DeleteCurrentElement()
    {
        if (CurrentElementGroup != null)
        {
            string[] elementNames = ElementManager.GetElementGroupNames();
            string currentName = CurrentElementGroup.Name;
            ElementManager.DeleteElementGroup(CurrentElementGroup);

            int currentIndex = Array.IndexOf(elementNames, currentName);
            int newIndex = (elementNames.Length > 1) ? Math.Max(0, currentIndex - 1) : -1;
            if (newIndex >= 0)
            {
                CurrentElementGroup = ElementManager.GetElementGroup(elementNames[newIndex]);
            }
            else
            {
                CurrentElementGroup = null;
            }

            ElementManager.ElementsHaveChanges = true;
        }
    }

}
