using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ElementMenu : MonoBehaviour
{
    public ButtonContainer elementsButtonContainer;
    public GameObject ElementButtonPrefab;
    static ElementGroup CurrentElementGroup;

    public TMP_InputField ElementNameInput;
    public TMP_InputField ElementDetailsInput;

    public TMP_InputField Version_ElementDetailsInput;
    public TMP_Text Version_PrevText;
    public TMP_Text Version_NextText;
    public TMP_Text Version_NumberText;
    public TMP_InputField Version_MinInput;
    public TMP_InputField Version_MaxInput;
    public TMP_Dropdown Version_ValueTypeDropDown;
    public Toggle Version_CueLevelToggle;
    public Toggle Version_TextLevelToggle;
    public GameObject Version_ArrayInputPrefab;
    public TMP_InputField[] Version_ArrayInputs;
    public ButtonContainer Version_ArrayContainer;

    private void Awake()
    {
        CreateArrayObjects();
    }

    void Start()
    {
        ElementManager.ElementsChanged += OnElementsChanged;
        DisplayElements();
    }

    private void CreateArrayObjects()
    {
        Version_ArrayInputs = new TMP_InputField[ElementDefinition.MaxArrayCount];
        Version_ArrayContainer.Configure(Version_ArrayInputPrefab, ElementDefinition.MaxArrayCount);
        for (int i = 0; i < ElementDefinition.MaxArrayCount; i++)
        {
            Version_ArrayInputs[i] = Version_ArrayContainer.buttons[i].GetComponentInChildren<TMP_InputField>();
            Version_ArrayContainer.buttons[i].GetComponentInChildren<Text>().text = (i + 1).ToString("00");
            Version_ArrayInputs[i].onEndEdit.AddListener(delegate { GetCurrentElementInfo(); });
        }
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
        List<string> _elementNames = ElementManager.GetAllElementGroupNames(false);
        elementsButtonContainer.Configure(ElementButtonPrefab, _elementNames.Count + 1);
        for (int i = 0; i < _elementNames.Count; i++)
        {
            elementsButtonContainer.buttons[i].GetComponent<ElementButton>().Configure(_elementNames[i]);
        }
        elementsButtonContainer.buttons[_elementNames.Count].GetComponent<ElementButton>().Configure("Add Element");
        elementsButtonContainer.buttons[_elementNames.Count].GetComponent<ElementButton>().ElementToggle.isOn = false;
        elementsButtonContainer.buttons[_elementNames.Count].GetComponent<ElementButton>().ElementToggle.enabled = false;


        //CURRENT ELEMENT
        if (CurrentElementGroup == null && _elementNames.Count > 0)
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
                Version_ElementDetailsInput.SetTextWithoutNotify(currentElementVersion.Description);
                Version_CueLevelToggle.SetIsOnWithoutNotify(currentElementVersion.CueLevel);
                Version_TextLevelToggle.SetIsOnWithoutNotify(currentElementVersion.TextLevel);
                Version_ValueTypeDropDown.SetValueWithoutNotify((int)currentElementVersion.ValueType);
                Version_MaxInput.SetTextWithoutNotify(currentElementVersion.Max?.ToString() ?? "");
                Version_MinInput.SetTextWithoutNotify(currentElementVersion.Min?.ToString() ?? "");
                for (int i = 0; i < ElementDefinition.MaxArrayCount; i++)
                {
                    Version_ArrayInputs[i].SetTextWithoutNotify(currentElementVersion.StringOptions[i]);
                    Version_ArrayInputs[i].interactable = currentElementVersion.ValueType == ElementValueTypes.Array ? i < currentElementVersion.Max.Value : i == 0;
                }
                Version_MaxInput.gameObject.SetActive(currentElementVersion.ValueType == ElementValueTypes.Integer || currentElementVersion.ValueType == ElementValueTypes.Array);
                Version_MinInput.gameObject.SetActive(currentElementVersion.ValueType == ElementValueTypes.Integer);
                Version_ArrayContainer.gameObject.SetActive(currentElementVersion.ValueType == ElementValueTypes.Array);

            }
            Version_PrevText.enabled = CurrentElementGroup.CurrentVersion > 0;
            Version_NextText.text = CurrentElementGroup.IsLastVersion() ? "+" : ">";
            Version_NumberText.text = CurrentElementGroup.FeatureVersionString();
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
                CurrentElementGroup.Name = NewName;
            }
            CurrentElementGroup.Description = ElementDetailsInput.text;
            CurrentElementGroup.CurrentDefinition().Description = Version_ElementDetailsInput.text;
            CurrentElementGroup.CurrentDefinition().CueLevel = Version_CueLevelToggle.isOn;
            CurrentElementGroup.CurrentDefinition().TextLevel = Version_TextLevelToggle.isOn;
            CurrentElementGroup.CurrentDefinition().ValueType = (ElementValueTypes)Version_ValueTypeDropDown.value;
            CurrentElementGroup.CurrentDefinition().Min = GetValueOrNull(Version_MinInput.text);
            CurrentElementGroup.CurrentDefinition().Max = GetValueOrNull(Version_MaxInput.text);
            CurrentElementGroup.CurrentDefinition().ClampMinMax();
            for (int i = 0; i < ElementDefinition.MaxArrayCount; i++)
            {
                CurrentElementGroup.CurrentDefinition().StringOptions[i] = Version_ArrayInputs[i].text;
            }
            ElementManager.ElementsHaveChanges = true;
            ElementManager.SaveElementGroupToJSON(CurrentElementGroup);
        }
    }
    int? GetValueOrNull(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return null;
        }

        if (int.TryParse(input, out int value))
        {
            return value;
        }

        return null; // In case input isn't a valid integer
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
            CurrentElementGroup.CreateNewVersion(true, true);
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
            List<string> elementNames = ElementManager.GetAllElementGroupNames(false);
            string currentName = CurrentElementGroup.Name;
            ElementManager.DeleteElementGroup(CurrentElementGroup);

            int currentIndex = elementNames.FindIndex(e => e == currentName);
            int newIndex = (elementNames.Count > 1) ? Math.Max(0, currentIndex - 1) : -1;
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
