using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public struct ElementSignature
{
    public string Name; 
    public List<string> Attributes;
    public ElementSignature(string _name)
    {
        Name = _name; 
        Attributes = new List<string>();
    }
    public ElementSignature(string _name, List<string> _attributes)
    {
        Name = _name;
        Attributes = _attributes != null ? new List<string>(_attributes) : new List<string>();
    }
   
    //target signture matches if name is blank or all target properties exist in my properties    

    public bool Match(ElementSignature target)
    { 
        if (string.IsNullOrEmpty(Name) && (Attributes == null || !Attributes.Any()))
        {
            return true;
        }
         
        if (!string.IsNullOrEmpty(Name) && Name.ToLower() != target.Name.ToLower())
        {
            return false;
        }
         
        foreach (var attribute in Attributes)
        {
            if (!target.Attributes.Any(attr => attr.Equals(attribute, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }
        }

        return true;
    }


    public bool IsEqual(ElementSignature other)
    {
        if (other.Name.ToLower() != Name.ToLower())
        {
            return false;
        }
        if (other.Attributes == null && Attributes == null)
        {
            return true;
        }
        if (other.Attributes == null || Attributes == null)
        {
            return false;
        }
        if (other.Attributes.Count != Attributes.Count)
        {
            return false;
        }
        foreach (var attribute in Attributes)
        {
            if (!Attributes.Any(attr => attr.Equals(attribute, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }
        }
        return true;
    }

    public override string ToString()
    {
        string stringText = Name;
        stringText += AttributesToString();
        return stringText;
    }

    public string AttributesToString()
    {
        string stringText = "";
        if (Attributes.Count > 0)
        {
            stringText += string.Join("", Attributes.ConvertAll(attr => $".{attr}"));
        }
        return stringText;
    }

    internal void SetAttributesFromString(string text)
    { 
        Attributes.Clear(); 
        if (!string.IsNullOrEmpty(text))
        { 
            var attributes = text.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries); 
            Attributes.AddRange(attributes);
        }
    }

}

[Serializable]
public struct Element 
{
    public ElementSignature Signature;
    public string Value; // For elements like <v speaker>, 'speaker' would be the Value. 
    public int GetInt()
    {
        if (int.TryParse(Value, out int result))
        {
            return result;
        }
        return 0;
    } 
    public float GetFloatValue()
    {
        if (float.TryParse(Value, out float result))
        {
            return result;
        }
        //check ElementManager for an value   then use 
        //return ElementManager.GetValue(Signature,Value); //this looks up the string and finds the current element definiture for the signature.
        return 0;
    }     

    public Element(string _name) 
    {
        Signature = new ElementSignature(_name);
        Value = ""; 
    }
    public Element(string _name, string _value)
    {
        Signature = new ElementSignature(_name);
        Value = _value; 
    }
    public Element(string _name, string _value, List<string> _attributes)
    {
        Signature = new ElementSignature(_name,_attributes);
        Value = _value; 
    }
    public static Element NewElement()
    {
        return new Element("", "", new List<string>());
    }
    
    internal Element Copy()
    {
        return new Element
        {
            Value = this.Value,
            Signature = new ElementSignature(this.Signature.Name, new List<string>(this.Signature.Attributes))
        };
    }
    public String StartTag()
    {
        if (string.IsNullOrWhiteSpace(Signature.Name))
        {
            return "";
        }
        var tagBuilder = new StringBuilder("<");
        if (Signature.Name == Common.ElementName_TimeStamp) 
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
            tagBuilder.Append(Signature.Name);
            foreach (var attr in Signature.Attributes)
            {
               tagBuilder.Append(".");
               tagBuilder.Append(attr);
            }

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
        bool noAttributes = Signature.Attributes == null || Signature.Attributes.Count == 0;

        return string.IsNullOrEmpty(Signature.Name) && string.IsNullOrEmpty(Value) && noAttributes;
    }

    public String EndTag()
    {
        return string.IsNullOrWhiteSpace(Signature.Name) || Signature.Name == Common.ElementName_TimeStamp || Signature.Name.ToLower() == "v" || Signature.Name.ToLower() == "voice" ? "" : $"</{Signature.Name}>";
    }
    
    //Used to combine cueChars to cueStrings where value matters
    public bool IsEqual(Element other)
    {                 
        if (other.Value.ToLower()!= Value.ToLower())
        {
            return false;
        }
        return Signature.IsEqual(other.Signature);
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
    public bool Match(Element target)//target is a subset if value is blank or target value matches this value (and signature matches)
    {
        if (!string.IsNullOrEmpty(Value) && Value.ToLower() != target.Value.ToLower())
        {
            return false;
        }
        return Signature.Match(target.Signature);
    }
}


