using System;
using Newtonsoft.Json;
using UnityEngine;
using System.Text.RegularExpressions;

public enum SettingTypes
{
    none,                   // No specific setting applied.

    Cue_Align,              // Cue_AlignTypes: How the text is aligned within the cue box ('start', 'center', 'end').
    Cue_Line,               // Integer: Vertical position, either as a number (line-based) or percentage (percent-based).
    Cue_Position,           // Integer: Horizontal position of the cue as a percentage.
    Cue_Size,               // Integer: Width of the cue box as a percentage of the video viewport.
    Cue_Vertical,           // Cue_VerticalTypes: Orientation of the text ('lr' for left-to-right, 'rl' for right-to-left).
    Cue_SnapToLines,        // Boolean: Whether the 'line' setting is line-based (true) or percentage-based (false).
    Cue_LineAlign,          // Cue_LineAlignTypes: Alignment of the text within the lines ('start', 'center', 'end').

    Region_Width,           // Integer: Specifies the width of the region as a percentage of the viewport (e.g., "50%").
    Region_Lines,           // Integer: Specifies the maximum number of lines that the region will display.
    Region_RegionAnchor,    // Position: Alignment point within the region as percentages (e.g., "50%,50%").
    Region_RegionAnchorX,   // Integer: X percentage of the region anchor within the region.
    Region_RegionAnchorY,   // Integer: Y percentage of the region anchor within the region.
    Region_ViewportAnchor,  // Position: Location of the region anchor in the viewport as percentages (e.g., "50%,50%").
    Region_ViewportAnchorX, // Integer: X percentage of the viewport anchor within the viewport.
    Region_ViewportAnchorY, // Integer: Y percentage of the viewport anchor within the viewport.
    Region_Scroll,          // Region_ScrollTypes: Specifies if and how text will scroll within the region. Options: 'up' or 'none'.

    Style_Color,            // ColorHolder: Text color.
    Style_BackgroundColor,  // ColorHolder: Background color.
    Style_FontFamily,       // String: Font family name.
    Style_FontSize,         // int: Font size, either in pixels or percentage.
    Style_FontWeight,       // Style_FontWeightTypes/Integer: Font weight, either as a keyword (e.g., 'Bold') or numeric value.
    Style_FontStyle,        // Style_FontStyleTypes: Font style (e.g., 'Italic').
    Style_TextDecoration,   // Style_TextDecorationTypes: Text decoration (e.g., 'Underline').
    Style_TextShadow,       // Not implemented: Placeholder for future text shadow settings.
    Style_Opacity,          // int: Opacity value from 0 (transparent) to 100 (fully opaque).

    COUNT                   // Count of setting types, used internally.
}

public enum Cue_AlignTypes { none, start, center, end, left, right }
public enum Cue_LineAlignTypes { start, center, end }
public enum Cue_VerticalTypes { none, lr, rl }
public enum Region_ScrollTypes { none, Up }
public enum Style_FontWeightTypes { None, Normal, Bold, Bolder, Lighter, Numeric }
public enum Style_FontStyleTypes { Normal, Italic, Oblique }
public enum Style_TextDecorationTypes { None, Underline, Overline, LineThrough }

[Serializable]
public class Setting
{

    [JsonProperty]
    SettingTypes SettingType;
    //  public string Name;//redundant, setting type is the source of truth, this is just for importing and storing potential input errors

    public string StringValue;
    public bool BoolValue;
    public int IntValue_1;

    public Position PositionValue_1;
    public ColorHolder ColorValue_1;

    public Cue_AlignTypes Cue_AlignType;
    public Cue_LineAlignTypes Cue_LineAlignType;
    public Cue_VerticalTypes Cue_VerticalType;

    public Region_ScrollTypes Region_ScrollType;

    public Style_FontWeightTypes Style_FontWeightType;
    public Style_FontStyleTypes Style_FontStyleType;
    public Style_TextDecorationTypes Style_TextDecorationType;

    [JsonIgnore]
    public bool ErrorParsing;
    private Setting setting;

    public Setting()
    {
        SettingType = SettingTypes.none;
        StringValue = "";
        IntValue_1 = 0;
        BoolValue = false;
        ColorValue_1 = (ColorHolder)Color.white;
        Cue_AlignType = Cue_AlignTypes.none;
        Cue_LineAlignType = Cue_LineAlignTypes.center;
        Cue_VerticalType = Cue_VerticalTypes.none;
        Region_ScrollType = Region_ScrollTypes.none;
        Style_FontWeightType = Style_FontWeightTypes.None;
        Style_FontStyleType = Style_FontStyleTypes.Normal;
        Style_TextDecorationType = Style_TextDecorationTypes.None;
        ErrorParsing = false;
    }
    public Setting(string _name, string _value)
    {
        IntValue_1 = 0;
        BoolValue = false;
        ColorValue_1 = (ColorHolder)Color.white;
        ErrorParsing = false;
        SettingType = GetEnum<SettingTypes>(_name);
        SetValues(_value);
    }

    public void SetValues(string _value)
    {
        StringValue = _value;
        switch (SettingType)
        {
            case SettingTypes.none:
                break;
            case SettingTypes.Cue_Align:
                Cue_AlignType = GetEnum<Cue_AlignTypes>(StringValue);
                break;
            case SettingTypes.Cue_LineAlign:
                Cue_LineAlignType = GetEnum<Cue_LineAlignTypes>(StringValue);
                break;
            case SettingTypes.Cue_Vertical: //this is literaly up and down like in asian languages, maybe a stretch goal
                Cue_VerticalType = GetEnum<Cue_VerticalTypes>(StringValue);
                break;
            case SettingTypes.Cue_Line: //very tricky, lines (region setting) is the available space. Line is the position within the available space.  varied length of text, actual number of lines, and font sizes makes this very difficult to interpret
                if (StringValue.EndsWith("%"))
                    SetPercent(StringValue);
                else
                    SetInt(StringValue);
                break;
            case SettingTypes.Cue_Position:  //a percent that impacts the top and bottom margins
                SetPercent(StringValue);
                break;
            case SettingTypes.Cue_Size:  //a percentage of the box which is inconsitent based on text size, also I think this is for LR although I have not found any info - will do %width+fixed number %height + fixed number
                SetPercent(StringValue);
                break;
            case SettingTypes.Cue_SnapToLines:
                BoolValue = StringValue.ToLower() == "true" || StringValue.ToLower() == "t" || StringValue.ToLower() == "y";
                break;
            case SettingTypes.Region_Width:
                SetPercent(StringValue);
                break;
            case SettingTypes.Region_Lines:
                SetInt(StringValue);
                break;
            case SettingTypes.Region_RegionAnchor:
                SetPercentVector(StringValue);
                break;
            case SettingTypes.Region_ViewportAnchor:
                SetPercentVector(StringValue);
                break;
            case SettingTypes.Region_Scroll:
                Region_ScrollType = GetEnum<Region_ScrollTypes>(StringValue);
                break;
            case SettingTypes.Region_RegionAnchorX:
                SetPercent(StringValue);
                break;
            case SettingTypes.Region_RegionAnchorY:
                SetPercent(StringValue);
                break;
            case SettingTypes.Region_ViewportAnchorX:
                SetPercent(StringValue);
                break;
            case SettingTypes.Region_ViewportAnchorY:
                SetPercent(StringValue);
                break;
            case SettingTypes.Style_Color:
                SetColor(StringValue);
                break;
            case SettingTypes.Style_BackgroundColor:
                SetColor(StringValue);
                break;
            case SettingTypes.Style_FontFamily: //currently just a string vlue
                break;
            case SettingTypes.Style_FontSize:
                SetPxOrPercent(StringValue);
                break;
            case SettingTypes.Style_FontWeight:
                if (SetInt(StringValue))
                {
                    Style_FontWeightType = Style_FontWeightTypes.Numeric;
                }
                else
                {
                    Style_FontWeightType = GetEnum<Style_FontWeightTypes>(StringValue);
                }
                break;
            case SettingTypes.Style_FontStyle:
                Style_FontStyleType = GetEnum<Style_FontStyleTypes>(StringValue);
                break;
            case SettingTypes.Style_TextDecoration:
                Style_TextDecorationType = GetEnum<Style_TextDecorationTypes>(StringValue);
                break;
            case SettingTypes.Style_TextShadow://not implemented
                break;
            case SettingTypes.Style_Opacity:
                SetInt100(StringValue);
                break;
            default:
                break;
        }
    }

    public Setting(Setting otherSetting)
    {
        SettingType = otherSetting.SettingType;
        StringValue = otherSetting.StringValue;
        IntValue_1 = otherSetting.IntValue_1;
        BoolValue = otherSetting.BoolValue;
        ColorValue_1 = otherSetting.ColorValue_1;
        PositionValue_1 = otherSetting.PositionValue_1;
        Equals(PositionValue_1, otherSetting.PositionValue_1);
        Equals(ColorValue_1, otherSetting.ColorValue_1);
        Cue_AlignType = otherSetting.Cue_AlignType;
        Cue_LineAlignType = otherSetting.Cue_LineAlignType;
        Cue_VerticalType = otherSetting.Cue_VerticalType;
        Region_ScrollType = otherSetting.Region_ScrollType;
        Style_FontWeightType = otherSetting.Style_FontWeightType;
        Style_FontStyleType = otherSetting.Style_FontStyleType;
        Style_TextDecorationType = otherSetting.Style_TextDecorationType;
    }

    public bool SetInt(string _stringValue)
    {
        if (!int.TryParse(_stringValue, out IntValue_1))
        {
            ErrorParsing = true;
            return false;
        }
        return true;
    }

    public bool SetInt100(string _stringValue)
    {
        float floatValue = 0;
        if (!float.TryParse(_stringValue, out floatValue))
        {
            ErrorParsing = true;
            return false;
        }
        IntValue_1 = (int)(floatValue * 100);
        return true;
    }

    public void SetPercent(string _stringValue)
    {
        string value = _stringValue.Replace("%", "");

        if (!int.TryParse(value, out IntValue_1))
        {
            ErrorParsing = true;
        }
    }

    public void SetPercentVector(string _stringValue)
    {
        string[] values = _stringValue.Replace("%", "").Split(',');
        if (values.Length == 2)
        {
            float x, y;
            if (float.TryParse(values[0].Trim(), out x) && float.TryParse(values[1].Trim(), out y))
            {
                PositionValue_1 = new Position(x, y);
            }
            else
            {
                ErrorParsing = true;
            }
        }
        else
        {
            ErrorParsing = true;
        }
    }
    public void SetPxOrPercent(string _stringValue)
    {
        BoolValue = true;
        float floatValue = 0;
        if (_stringValue.EndsWith("px"))
        {
            if (float.TryParse(_stringValue.Replace("px", ""), out floatValue))
            {
                IntValue_1 = (int)floatValue;
                BoolValue = true; // Indicates px
            }
            else
            {
                ErrorParsing = true;
            }
        }
        else if (_stringValue.EndsWith("%"))
        {
            if (float.TryParse(_stringValue.Replace("%", ""), out floatValue))
            {
                IntValue_1 = (int)floatValue;
                BoolValue = false; // Indicates %
            }
            else
            {
                ErrorParsing = true;
            }
        }
    }


    public void SetEnumValue<T>(string _value) where T : Enum
    {
        string adjValue = _value.Replace("-", "");
        if (Enum.TryParse(typeof(T), adjValue, true, out var result))
        {
            IntValue_1 = (int)result;
        }
        else
        {
            ErrorParsing = true;
        }
    }

    public T GetEnum<T>(string _value) where T : struct, Enum
    {
        string adjValue = _value.Replace("-", "");
        if (Enum.TryParse(adjValue, true, out T result))
        {
            return result;
        }
        else
        {
            ErrorParsing = true;
            return default(T);
        }
    }

    public void SetColor(string _stringValue)
    {
        switch (_stringValue.ToLower())
        {
            case "red":
                ColorValue_1 = (ColorHolder)Color.red;
                break;
            case "green":
                ColorValue_1 = (ColorHolder)Color.green;
                break;
            case "blue":
                ColorValue_1 = (ColorHolder)Color.blue;
                break;
            case "purple":
                ColorValue_1 = new ColorHolder(0.5f, 0, 0.5f);
                break;
            case "yellow":
                ColorValue_1 = (ColorHolder)Color.yellow;
                break;
            case "black":
                ColorValue_1 = (ColorHolder)Color.black;
                break;
            case "white":
                ColorValue_1 = (ColorHolder)Color.white;
                break;
            case "cyan":
                ColorValue_1 = (ColorHolder)Color.cyan;
                break;
            case "magenta":
                ColorValue_1 = (ColorHolder)Color.magenta;
                break;
            case "gray":
            case "grey":
                ColorValue_1 = (ColorHolder)Color.gray;
                break;
            case "orange":
                ColorValue_1 = new ColorHolder(1.0f, 0.65f, 0.0f);
                break;
            default:
                Color c = new Color();
                if (ColorUtility.TryParseHtmlString(_stringValue, out c))
                {
                    ColorValue_1 = (ColorHolder)c;
                }
                break;
        }
    }

    public override string ToString()
    {
        string valueString = SettingType switch
        {
            SettingTypes.Cue_Align => Cue_AlignType.ToString(),
            SettingTypes.Cue_LineAlign => Cue_LineAlignType.ToString(),
            SettingTypes.Cue_Vertical => Cue_VerticalType.ToString(),
            SettingTypes.Cue_Line => IntValue_1.ToString(),
            SettingTypes.Cue_Position => IntValue_1.ToString() + "%",
            SettingTypes.Cue_Size => IntValue_1.ToString() + "%",
            SettingTypes.Cue_SnapToLines => BoolValue.ToString(),
            SettingTypes.Region_Width => IntValue_1.ToString() + "%",
            SettingTypes.Region_Lines => IntValue_1.ToString(),
            SettingTypes.Region_RegionAnchor => PositionValue_1.ToString(),
            SettingTypes.Region_ViewportAnchor => PositionValue_1.ToString(),
            SettingTypes.Region_Scroll => Region_ScrollType.ToString(),
            SettingTypes.Region_RegionAnchorX => IntValue_1.ToString() + "%",
            SettingTypes.Region_RegionAnchorY => IntValue_1.ToString() + "%",
            SettingTypes.Region_ViewportAnchorX => IntValue_1.ToString() + "%",
            SettingTypes.Region_ViewportAnchorY => IntValue_1.ToString() + "%",
            SettingTypes.Style_Color => ColorValue_1.ToString(),
            SettingTypes.Style_BackgroundColor => ColorValue_1.ToString(),
            SettingTypes.Style_FontFamily => StringValue,
            SettingTypes.Style_FontSize => IntValue_1.ToString() + (BoolValue ? "px" : "%"),
            SettingTypes.Style_FontWeight => Style_FontWeightType == Style_FontWeightTypes.Numeric ? IntValue_1.ToString() : Style_FontWeightType.ToString(),
            SettingTypes.Style_FontStyle => Style_FontStyleType.ToString(),
            SettingTypes.Style_TextDecoration => Style_TextDecorationType.ToString(),
            SettingTypes.Style_Opacity => IntValue_1.ToString(),
            _ => StringValue
        };

        return $"{GetSettingName()}: {valueString}";
    }

    public bool IsMatch(Setting otherSetting)
    {
        if (otherSetting == null)
            return false;

        return
                SettingType == otherSetting.SettingType &&
                StringValue == otherSetting.StringValue &&
                IntValue_1 == otherSetting.IntValue_1 &&
                BoolValue == otherSetting.BoolValue &&
                Equals(PositionValue_1, otherSetting.PositionValue_1) &&
                Equals(ColorValue_1, otherSetting.ColorValue_1) &&
                Cue_AlignType == otherSetting.Cue_AlignType &&
                Cue_LineAlignType == otherSetting.Cue_LineAlignType &&
                Cue_VerticalType == otherSetting.Cue_VerticalType &&
                Region_ScrollType == otherSetting.Region_ScrollType &&
                Style_FontWeightType == otherSetting.Style_FontWeightType &&
                Style_FontStyleType == otherSetting.Style_FontStyleType &&
                Style_TextDecorationType == otherSetting.Style_TextDecorationType;
    }

    public string GetSettingName()
    {
        return GetSettingName(SettingType);
    }

    public static string GetSettingName(SettingTypes _settingType)
    {
        string returnString = _settingType.ToString()
                .Replace("Style_", "Text:")
                .Replace("Region_", "Region:")
                .Replace("Cue_", "CaptionBox:");

        returnString = Regex.Replace(returnString, "(?<!^)([A-Z])", " $1");
        return returnString;
    }

    public void SetSettingTypeFromString(string _settingType)
    {
        string formattedString = _settingType
            .Replace("Caption Box:", "Cue_")
            .Replace("Region:", "Region_")
            .Replace("Text:", "Style_");

        formattedString = Regex.Replace(formattedString, @"\s+", "");

        SettingType = (SettingTypes)Enum.Parse(typeof(SettingTypes), formattedString);
    }

    internal SettingTypes GetSettingType()
    {
        return SettingType;
    }
}


