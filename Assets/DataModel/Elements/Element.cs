using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


[Serializable]
public struct Element 
{
    public string Name;
    public string Value; // For elements like <v speaker>, 'speaker' would be the Value. 
  
    public Element(string _name) 
    {
        Name = _name;
        Value = ""; 
    }
    public Element(string _name, string _value)
    {
        Name = _name;
        Value = _value; 
    } 
    public static Element NewElement()
    {
        return new Element("", "");
    }
    
    internal Element Copy()
    {
        return new Element
        {
            Value = this.Value,

            Name = this.Name
        };
    }
     
    public int GetIntValue(ElementDefinition _definition)
    {
        
        int intValue = 0;

        if (_definition.ValueType == ElementValueTypes.Array)
        {
            int index = _definition.StringOptions.IndexOf(Value);
            
        }
        if (_definition.ValueType == ElementValueTypes.Integer)
        {
            if (float.TryParse(Value, out float result))
            {
                intValue = (int)Math.Floor(result);
            }
        }
        if (_definition.Min.HasValue)
        { 
            intValue = Mathf.Max(intValue, _definition.Min.Value); 
        }
        if (_definition.Max.HasValue)
        {
            intValue = Mathf.Min(intValue, _definition.Max.Value);
        }
        return intValue;
    }

    public String StartTag()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            return "";
        }
        var tagBuilder = new StringBuilder("<");
        if (Name == Common.ElementName_TimeStamp) 
        {
            if (float.TryParse(Value, out float timeStamp))
            {
                tagBuilder.Append(Common.FloatToTimeString(timeStamp));
            }
            else
            {
                tagBuilder.Append(Value);
            }
        }
        else
        {
            tagBuilder.Append(Name);
            if (!string.IsNullOrEmpty(Value))
            { 
                tagBuilder.Append($"=\"{Value}\"");
            }
        }
        tagBuilder.Append(">");
        return tagBuilder.ToString();
    }

    internal bool IsEmpty()
    {
        return string.IsNullOrEmpty(Name) && string.IsNullOrEmpty(Value);
    }

    public String EndTag()
    {
        return string.IsNullOrWhiteSpace(Name) || Name == Common.ElementName_TimeStamp || Name.ToLower() == "v" || Name.ToLower() == "voice" ? "" : $"</{Name}>";
    }
    
    //Used to combine cueChars to cueStrings where value matters
    public bool IsEqual(Element other)
    {                 
        if (other.Value.ToLower()!= Value.ToLower())
        {
            return false;
        }
        return Name == other.Name;
    }

    public bool Match(List<Element> targets)
    {
        foreach (var targetElement in targets)
        {
            if (Match(targetElement))
            {
                return true;
            }
        }       
        return false;
    }

    //target is a subset if value is blank or target value matches this value
    public bool Match(Element target)
    {
        if (!string.IsNullOrEmpty(Value) && Value.ToLower() != target.Value.ToLower())
        {
            return false;
        }
        return  string.IsNullOrEmpty(Name) || Name == target.Name;
    }

    internal string ToButtonText()
    {
        string returnText = Name;
        if (!string.IsNullOrEmpty(Value))
        {
            returnText += $" [{Value}]";
        }
        return returnText;
    }
}


