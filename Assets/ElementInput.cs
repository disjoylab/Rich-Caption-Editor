using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ElementInput : MonoBehaviour
{

    public TMP_Dropdown Element_Name_Dropdown;
    public TMP_InputField Element_Value_Input;
    public TMP_Dropdown Element_Value_DropDown;
    public Action ElementInputChanged;

    private void Awake()
    {
        Element_Name_Dropdown.onValueChanged.AddListener(NameInputChanged);
        Element_Value_DropDown.onValueChanged.AddListener(ValueInputChanged);
        Element_Value_Input.onEndEdit.AddListener(ValueDropDownChanged);        
    }
    private void NameInputChanged(int _value) 
    {
        ConfigureValueOptions(Element_Name_Dropdown.options[Element_Name_Dropdown.value].text, "");
        ElementInputChanged?.Invoke();
    }
    private void ValueInputChanged(int _value)
    {
        ElementInputChanged?.Invoke();
    }
    private void ValueDropDownChanged(string _text)
    {
        ElementInputChanged?.Invoke();
    }
    internal Element GetElement()
    {
        Element e = new Element(Element_Name_Dropdown.options[Element_Name_Dropdown.value].text);
        ElementDefinition elementDefinition = ElementManager.GetElementGroup(e.Name).CurrentDefinition();
        if (elementDefinition.ValueType == ElementValueTypes.Array )
        {
            e.Value = Element_Value_DropDown.options[Element_Value_DropDown.value].text;
        }
        if (elementDefinition.ValueType == ElementValueTypes.Integer || elementDefinition.ValueType == ElementValueTypes.Text)
        { 
            e.Value = Element_Value_Input.text;
        }
        return e;
    }

    internal void ClearOptions()
    {
        Element_Value_Input.text="";
        Element_Name_Dropdown.ClearOptions();
        Element_Value_DropDown.ClearOptions();
    }

    //THIS IS CONFIGURED TO FILTER ONLY CUE LEVEL ELEMENTS 
    public void DisplayElementDetails(Element _element)
    {
        string elementName = _element.Name;
        string elementValue = _element.Value;


        List<string> CueElementNames = new List<string>();
        CueElementNames.Add("");
        List<string> elements = new List<string>();
         
        elements.AddRange(ElementManager.GetCueLevelElementGroupNames());
         
        foreach (var e in elements)
        {
            if (!string.IsNullOrEmpty(e))
            {
                CueElementNames.Add(e);
            }
        }
        Element_Name_Dropdown.AddOptions(CueElementNames);
        int nameIndex = CueElementNames.FindIndex(e=>e.ToLower()== elementName.ToLower());
        if (nameIndex >= 0)
        {
            Element_Name_Dropdown.SetValueWithoutNotify(nameIndex);
        }
        ConfigureValueOptions(elementName, elementValue);
    }

    private void ConfigureValueOptions(string elementName, string elementValue)
    {
        Element_Value_DropDown.ClearOptions();
        Element_Value_Input.text = "";
        ElementGroup group = ElementManager.GetElementGroup(elementName); 
        ElementDefinition elementDefinition = group.CurrentDefinition();
        Element_Value_DropDown.gameObject.SetActive(elementDefinition.ValueType == ElementValueTypes.Array);
        Element_Value_Input.gameObject.SetActive(elementDefinition.ValueType == ElementValueTypes.Integer || elementDefinition.ValueType == ElementValueTypes.Text);
        if (elementDefinition.ValueType == ElementValueTypes.Array)
        {
            List<string> valueOptions = new List<string>();
            valueOptions.Add(""); 
            valueOptions.AddRange(elementDefinition.GetStringOptions());
           
            Element_Value_DropDown.AddOptions(valueOptions);
            int valueIndex = valueOptions.FindIndex(e => e.ToLower() == elementValue.ToLower());
            if (valueIndex >= 0)
            {
                Element_Value_DropDown.SetValueWithoutNotify(valueIndex);
            }
        }
        if (elementDefinition.ValueType == ElementValueTypes.Integer )
        {
            Element_Value_Input.text = elementValue;
            Element_Value_Input.contentType = TMP_InputField.ContentType.IntegerNumber;
        }
        if (elementDefinition.ValueType == ElementValueTypes.Text)
        {
            Element_Value_Input.text = elementValue;
            Element_Value_Input.contentType = TMP_InputField.ContentType.Standard;
        }
    }
}
