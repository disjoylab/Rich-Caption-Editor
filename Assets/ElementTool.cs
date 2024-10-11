using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ElementTool : MonoBehaviour
{
    public Toggle myToggle;
    public TMP_Text Element_Name_Text;
    public TMP_InputField Element_Value_Input;
    public TMP_Dropdown Element_Value_DropDown;
    public static ElementTool SelectedElementTool;
    public static Action ElementToolsChanged;
    public void Configure(string _elementName)
    {
        ElementToolsChanged += OnElementToolChanged;
        myToggle.onValueChanged.AddListener(SetSelected);
        SetObjects(_elementName);
    }
    private void OnDestroy()
    {
        ElementToolsChanged -= OnElementToolChanged;
    }

    private void OnElementToolChanged()
    {
       myToggle.SetIsOnWithoutNotify(SelectedElementTool == this);         
    }

    public void SetSelected(bool _selected)
    {
        SelectedElementTool = _selected ? this : null;
        ElementToolsChanged?.Invoke();
    }

    private void SetObjects(string _elementName)
    {
        Element_Name_Text.text = _elementName;
        float textWidth =Mathf.Max( Element_Name_Text.preferredWidth+10,75);
        Element_Value_DropDown.ClearOptions();
        Element_Value_Input.SetTextWithoutNotify("");
        ElementGroup group = ElementManager.GetElementGroup(_elementName);
        ElementDefinition elementDefinition = group.CurrentDefinition();
        Element_Value_DropDown.gameObject.SetActive(elementDefinition.ValueType == ElementValueTypes.Array);
        Element_Value_Input.gameObject.SetActive(elementDefinition.ValueType == ElementValueTypes.Integer || elementDefinition.ValueType == ElementValueTypes.Text);
        if (elementDefinition.ValueType == ElementValueTypes.Array)
        {
            List<string> valueOptions = new List<string>();
            valueOptions.Add("");
            valueOptions.AddRange(elementDefinition.GetStringOptions());
            Element_Value_DropDown.AddOptions(valueOptions);
            textWidth = MathF.Max(textWidth, 180);
        }
        if (elementDefinition.ValueType == ElementValueTypes.Integer)
        {
            Element_Value_Input.contentType = TMP_InputField.ContentType.IntegerNumber;
            textWidth = MathF.Max(textWidth, 75);
        }
        if (elementDefinition.ValueType == ElementValueTypes.Text)
        {
            Element_Value_Input.contentType = TMP_InputField.ContentType.Standard;
            textWidth = MathF.Max(textWidth, 180);
        }
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(textWidth, 75);
    }

    internal Element GetElement()
    {
        Element e = new Element(Element_Name_Text.text);
        ElementDefinition elementDefinition = ElementManager.GetElementGroup(e.Name).CurrentDefinition();
        if (elementDefinition.ValueType == ElementValueTypes.Array)
        {
            e.Value = Element_Value_DropDown.options[Element_Value_DropDown.value].text;
        }
        if (elementDefinition.ValueType == ElementValueTypes.Integer || elementDefinition.ValueType == ElementValueTypes.Text)
        {
            e.Value = Element_Value_Input.text;
        }
        return e;
    }
}
