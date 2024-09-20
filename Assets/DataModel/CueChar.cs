

using System;
using System.Collections.Generic;

[Serializable]
public struct CueChar 
{
    public char c; 
    public List<Element> elements; 

    public CueChar Copy()
    {        
         List<Element> elementsCopy = new List<Element>();
        foreach (var e in elements)
        {
            elementsCopy.Add(e.Copy());
        }
        return new CueChar(c, elementsCopy);
    }
    public CueChar Duplicate(char _c)
    {
        List<Element> elementsCopy = new List<Element>();
        foreach (var e in elements)
        {
            elementsCopy.Add(e.Copy());
        }
        return new CueChar(_c, elementsCopy);
    }
    public CueChar FilteredCopy(List<string> filter) //this can be used to get a copy with a specific element name like mood, may need the ability to grab hydrated elements (with attributes and values)
    {
        List<Element> elementsCopy = new List<Element>();
        foreach (string f in filter)
        {
            if (elements.Exists(e=>e.Signature.Name==f))
            {
                elementsCopy.Add(new Element(f,""));
            }           
        }
        return new CueChar(c, elementsCopy);
    }

    public CueChar(char _c, List<Element> _elements)
    {
        c = _c;
        elements = new List<Element>();
        foreach (var e in _elements)
        {
            elements.Add(e.Copy());
        }
    }
    public CueChar(char _c)
    {
        c = _c;
        elements = new List<Element>() ;
    }

    internal void AddElement(Element e)
    {
        if (e.IsEmpty())
        {
            return;
        }

        // Check if the element already exists
        if (!elements.Exists(existingElement => existingElement.IsEqual(e)))
        {
            elements.Add(e.Copy());
        }
    }

    internal void RemoveElement(Element e)
    {
        if (e.IsEmpty())
        {
            return;
        }

        // Find and remove the element if it exists
        int index = elements.FindIndex(existingElement => existingElement.IsEqual(e));
        if (index >= 0)
        {
            elements.RemoveAt(index);
        }
    }

}
