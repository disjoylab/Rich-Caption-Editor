using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharSetting
{
    public char c;
    public bool Bold=false;
    public bool Italic = false;
    public bool Underline = false;
    public bool Strikethrough = false;

    //SHOULD THIS OBJECT ENCAPSULATE THE CONCEPT OF ANIMATION FRO SIZE COLOR AND POSITION?
    public float FontSize = 12;//default
    public Color TextColor = new Color(1,1,1,1);
    public Vector2 Position; 

    public CharSetting(CueChar cueChar)
    {
        c = cueChar.c;
    }

    internal void AddStyleSettings(List<Setting> _settings)
    {
        foreach (Setting _setting in _settings)
        {
            switch (_setting.SettingType)
            {
                case SettingTypes.Style_Color:
                    TextColor.r = _setting.ColorValue_1.R;
                    TextColor.g = _setting.ColorValue_1.G;
                    TextColor.b = _setting.ColorValue_1.B;
                    break;
                case SettingTypes.Style_BackgroundColor:// how would/can this work per char
                    break;
                case SettingTypes.Style_FontFamily:
                    break;
                case SettingTypes.Style_FontSize:
                    FontSize = _setting.BoolValue? _setting.IntValue_1:12f* (((float)_setting.IntValue_1)/ 100f);
                    break;
                case SettingTypes.Style_FontWeight:
                    Bold = _setting.Style_FontWeightType == Style_FontWeightTypes.Bold; // there are other options here like bolder or numeric
                    break;
                case SettingTypes.Style_FontStyle:
                    Italic = _setting.Style_FontStyleType == Style_FontStyleTypes.Italic;
                    break;
                case SettingTypes.Style_TextDecoration:
                    Strikethrough = _setting.Style_TextDecorationType == Style_TextDecorationTypes.LineThrough;
                    Underline = _setting.Style_TextDecorationType == Style_TextDecorationTypes.Underline;
                    break;
                case SettingTypes.Style_TextShadow:
                    break;
                case SettingTypes.Style_Opacity:
                    TextColor.a = ((float)_setting.IntValue_1) / 100; ;
                    break;
                default:
                    break;
            }
        }
    }
}
