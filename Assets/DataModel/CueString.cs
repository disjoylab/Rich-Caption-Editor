
using System;
using System.Collections.Generic;

using Newtonsoft.Json;

[Serializable]
public struct CueString
{ 
    public string s;
    public List<Element> elements;
    
    public CueString(CueChar cueChar)
    {
        s = cueChar.c.ToString();
        elements = new List<Element>();
        foreach (var e in cueChar.elements)
        {
            elements.Add(e.Copy());
        }
    }
    internal bool AddCueChar(CueChar cueChar)
    { 
        if (cueChar.elements == null || elements == null)
        {
            return false;
        }
        if (cueChar.elements.Count != elements.Count)
        {
            return false;
        }
        foreach (var element in elements)
        {
            if (!cueChar.elements.Exists(e=>e.IsEqual(element)))
            {
                return false;
            }
        }        
        s += cueChar.c;         
        return true;
    }
}