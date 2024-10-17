using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ElementValueTypes {None=0,Integer=1, Text=2, Array=3 }
public class ElementDefinition
{
    public string Description;
    public int? Min;
    public int? Max;
    public ElementValueTypes ValueType;
    public List<string> StringOptions; 
    public bool CueLevel;
    public bool TextLevel;
    public static int MaxArrayCount = 20;

    public ElementDefinition(string _description,  bool _cueLevel, bool _textLevel)
    {
        Description = _description;
        TextLevel = _textLevel;
        CueLevel = _cueLevel;
        ValueType = ElementValueTypes.None;
        StringOptions = new List<string>(MaxArrayCount);
        StringOptions.AddRange(Enumerable.Repeat("", MaxArrayCount)); 
    }   

    internal void ClampMinMax()
    {
        if (Max.HasValue && Min.HasValue)
        {
            if (Max < Min)
            {
                Max = Min;
            }
        }
        if (ValueType == ElementValueTypes.Array)
        {
            Min = 1;
            int max = Max.HasValue ? Max.Value : 5;
            Max = Mathf.Clamp(max, 1, MaxArrayCount);
        }
    }

    public List<string> GetStringOptions()
    {
        int count = Max.HasValue && Max.Value > 0 ? Max.Value : 1;
        return StringOptions.Take(count).ToList();
    }
}
