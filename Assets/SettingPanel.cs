using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour
{
    private const string Disabled_Dropdown = "---------";
    public Setting mySetting;
    public TMP_Text NameLabel;
         
    public GameObject InputObject_1;
        public TMP_InputField inputField_1;//this should be an array
    
    public GameObject PositionObject_1;
        public PositionInput PositionInput_1;

    public GameObject ColorObject_1;
         public ColorInput colorInput_1;

    public GameObject DropdownObject_1;
    public TMP_Dropdown dropdown_1;

    public void Configure(Setting _setting)
    {
        mySetting = _setting;
        DisplaySetting();
    }

    private void DisplaySetting()
    {
        InputObject_1.SetActive(false); 
        PositionObject_1.SetActive(false);
        ColorObject_1.SetActive(false);
        DropdownObject_1.SetActive(false);

        if (mySetting ==null)
        {
            return;
        }       
        //make ojbects visible based on setting type
        NameLabel.text = mySetting.Name;
        switch (mySetting.SettingType)
        {
            case SettingTypes.none:
                if (string.IsNullOrWhiteSpace(mySetting.Name))
                {
                    DropdownObject_1.SetActive(true );
                    NameLabel.text = "Add Setting";
                    PopulateDropdownWithEnum<SettingTypes>(dropdown_1, 0);
                    dropdown_1.options.RemoveAt((int)SettingTypes.COUNT);
                    var setttingTypesUsed = FeaturesMenu.GetSettingTypesUsed();
                    TMP_Dropdown.OptionData option_none = dropdown_1.options[0];
                    option_none.text = Disabled_Dropdown;
                    for (int i = 0; i < (int)SettingTypes.COUNT; i++)
                    {
                        if (setttingTypesUsed[i])
                        {
                            TMP_Dropdown.OptionData option = dropdown_1.options[i];
                            option.text = Disabled_Dropdown;
                        }                        
                    }
                }
                break;
            case SettingTypes.Cue_Align:
                DropdownObject_1.SetActive(true);
                PopulateDropdownWithEnum<Cue_AlignTypes>(dropdown_1, (int)mySetting.Cue_AlignType);
                break;
            case SettingTypes.Cue_Line:
                InputObject_1.SetActive(true);
                inputField_1.SetTextWithoutNotify( mySetting.IntValue_1.ToString());
                break;
            case SettingTypes.Cue_Position:
                InputObject_1.SetActive(true);
                inputField_1.SetTextWithoutNotify (mySetting.IntValue_1.ToString());
                break;
            case SettingTypes.Cue_Size:
                InputObject_1.SetActive(true);
                inputField_1.SetTextWithoutNotify( mySetting.IntValue_1.ToString());
                break;
            case SettingTypes.Cue_Vertical:
                DropdownObject_1.SetActive(true);
                PopulateDropdownWithEnum<Cue_VerticalTypes>(dropdown_1, (int)mySetting.Cue_VerticalType);
                break;
            case SettingTypes.Cue_SnapToLines:
                InputObject_1.SetActive(true);
                inputField_1.SetTextWithoutNotify( mySetting.BoolValue?"true":"false");
                break;
            case SettingTypes.Cue_LineAlign:
                DropdownObject_1.SetActive(true);
                PopulateDropdownWithEnum<Cue_LineAlignTypes>(dropdown_1, (int)mySetting.Cue_LineAlignType);
                break;
            case SettingTypes.Region_Width:
                InputObject_1.SetActive(true);
                inputField_1.SetTextWithoutNotify ( mySetting.IntValue_1.ToString());
                break;
            case SettingTypes.Region_Lines:
                InputObject_1.SetActive(true);
                inputField_1.SetTextWithoutNotify ( mySetting.IntValue_1.ToString());
                break;
            case SettingTypes.Region_RegionAnchor:
                PositionObject_1.SetActive(true);
                PositionInput_1.SetPosition(mySetting.PositionValue_1);
                break;
            case SettingTypes.Region_RegionAnchorX:
                InputObject_1.SetActive(true);
                inputField_1.text = mySetting.IntValue_1.ToString();
                break;
            case SettingTypes.Region_RegionAnchorY:
                InputObject_1.SetActive(true);
                inputField_1.text = mySetting.IntValue_1.ToString();
                break;
            case SettingTypes.Region_ViewportAnchor:
                PositionObject_1.SetActive(true);
                PositionInput_1.SetPosition(mySetting.PositionValue_1);
                break;
            case SettingTypes.Region_ViewportAnchorX:
                InputObject_1.SetActive(true);
                inputField_1.SetTextWithoutNotify (mySetting.IntValue_1.ToString());
                break;
            case SettingTypes.Region_ViewportAnchorY:
                InputObject_1.SetActive(true);
                inputField_1.SetTextWithoutNotify (mySetting.IntValue_1.ToString());
                break;
            case SettingTypes.Region_Scroll:
                DropdownObject_1.SetActive(true);
                PopulateDropdownWithEnum<Region_ScrollTypes>(dropdown_1, (int)mySetting.Region_ScrollType);
                break;
            case SettingTypes.Style_Color:
                Color C1 = (Color)mySetting.ColorValue_1;
                ColorObject_1.SetActive(true);
                colorInput_1.SetColor(C1);
                break;
            case SettingTypes.Style_BackgroundColor:
                Color C2 = (Color)mySetting.ColorValue_1;
                ColorObject_1.SetActive(true);
                colorInput_1.SetColor(C2);
                break;
            case SettingTypes.Style_FontFamily:
                InputObject_1.SetActive(true);
                inputField_1.SetTextWithoutNotify (mySetting.StringValue);
                break;
            case SettingTypes.Style_FontSize:
                InputObject_1.SetActive(true); 
                inputField_1.SetTextWithoutNotify ( mySetting.IntValue_1.ToString() + (mySetting.BoolValue?"px":"%")); 
                break;
            case SettingTypes.Style_FontWeight:
                if (mySetting.Style_FontWeightType == Style_FontWeightTypes.Numeric)
                {
                    InputObject_1.SetActive(true);
                    inputField_1.SetTextWithoutNotify (mySetting.IntValue_1.ToString());
                }                
                DropdownObject_1.SetActive(true);
                PopulateDropdownWithEnum<Style_FontWeightTypes>(dropdown_1, (int)mySetting.Style_FontWeightType);
                break;
            case SettingTypes.Style_FontStyle:
                DropdownObject_1.SetActive(true);
                PopulateDropdownWithEnum<Style_FontStyleTypes>(dropdown_1, (int)mySetting.Style_FontStyleType);
                break;
            case SettingTypes.Style_TextDecoration:
                DropdownObject_1.SetActive(true);
                PopulateDropdownWithEnum<Style_TextDecorationTypes>(dropdown_1, (int)mySetting.Style_TextDecorationType);
                break;
            case SettingTypes.Style_TextShadow:
                break;
            case SettingTypes.Style_Opacity:
                InputObject_1.SetActive(true);
                inputField_1.SetTextWithoutNotify(mySetting.IntValue_1.ToString());
                break;
            default:
                break;
        }
    }

    public void PopulateDropdownWithEnum<T>(TMP_Dropdown _dropdown, int _selected) where T : Enum
    {
        // Clear existing options
        _dropdown.ClearOptions();

        // Get the names of the enum values
        string[] enumNames = System.Enum.GetNames(typeof(T));

        // Add options to the dropdown
        _dropdown.AddOptions(new List<string>(enumNames));
        _dropdown.SetValueWithoutNotify(_selected);
    }
    public void DeleteMySetting()
    {
        FeaturesMenu.DeleteSetting(mySetting);
    }
    public void GetValues()
    {
        if (mySetting == null)
        {
            return;
        }
        string StringValue = inputField_1.text;
        switch (mySetting.SettingType)
        {
            case SettingTypes.none:
                if (string.IsNullOrWhiteSpace(mySetting.Name))
                {
                    if (dropdown_1.options[dropdown_1.value].text == Disabled_Dropdown)
                    {
                        return;
                    }
                    mySetting.SettingType = (SettingTypes)dropdown_1.value;
                    mySetting.Name = dropdown_1.options[dropdown_1.value].text;
                    StringValue = "";
                    FeaturesMenu.AddSetting(mySetting);
                }
                break;
            case SettingTypes.Cue_Align:
                StringValue = dropdown_1.options[dropdown_1.value].text;
                break;
            case SettingTypes.Cue_Line:
                break;
            case SettingTypes.Cue_Position:
                break;
            case SettingTypes.Cue_Size:
                break;
            case SettingTypes.Cue_Vertical:
                StringValue = dropdown_1.options[dropdown_1.value].text;
                break;
            case SettingTypes.Cue_SnapToLines:
                break;
            case SettingTypes.Cue_LineAlign:
                StringValue = dropdown_1.options[dropdown_1.value].text;
                break;
            case SettingTypes.Region_Width:
                break;
            case SettingTypes.Region_Lines:
                break;
            case SettingTypes.Region_RegionAnchor:
                StringValue = PositionInput_1.GetPositionString();
                break;
            case SettingTypes.Region_RegionAnchorX:
                break;
            case SettingTypes.Region_RegionAnchorY:
                break;
            case SettingTypes.Region_ViewportAnchor:
                StringValue = PositionInput_1.GetPositionString();
                break;
            case SettingTypes.Region_ViewportAnchorX:
                break;
            case SettingTypes.Region_ViewportAnchorY:
                break;
            case SettingTypes.Region_Scroll:
                StringValue = dropdown_1.options[dropdown_1.value].text;
                break;
            case SettingTypes.Style_Color:
                StringValue = colorInput_1.GetColorHex();
                break;
            case SettingTypes.Style_BackgroundColor:
                StringValue = colorInput_1.GetColorHex();
                break;
            case SettingTypes.Style_FontFamily:
                break;
            case SettingTypes.Style_FontSize:
                break;
            case SettingTypes.Style_FontWeight:
                StringValue = dropdown_1.options[dropdown_1.value].text;
                break;
            case SettingTypes.Style_FontStyle:
                StringValue = dropdown_1.options[dropdown_1.value].text;
                break;
            case SettingTypes.Style_TextDecoration:
                StringValue = dropdown_1.options[dropdown_1.value].text;
                break;
            case SettingTypes.Style_TextShadow:
                break;
            case SettingTypes.Style_Opacity:
                break;
            default:
                break;
        }
        mySetting.SetValues(StringValue);
    }
}
