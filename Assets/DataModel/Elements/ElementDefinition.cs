using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ElementValueTypes {noValue, intValue,floatValue } 
public class ElementDefinition
{
    public string Description;
    public ElementValueTypes ElementType;
    public bool Range;// min max values - just clamp the numbers or if its an array lerp them.
    public float Min;
    public float Max;
    public bool Array;
    public List<string> StringOptions;  

    
    public ElementDefinition(ElementValueTypes _elementType, string _description = "")
    {
        Description = _description;
        ElementType = _elementType;
        StringOptions = new List<string>();
    }
    public float GetValue(string _StringValue) // based on the definition get a value: '1.1' and the type is int return 1: 'sad' return index or a lerped float if a range is defined
    {
        return 0;
    }
}
