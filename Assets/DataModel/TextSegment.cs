using System;
using System.Collections.Generic;
using Newtonsoft.Json;
 
[Serializable]
public class TextSegment
{ 
    [JsonIgnore]
    public List<CueChar> Content;     
    public List<CueString> StringContent;
    public TextSegment()
    {
        Content = new List<CueChar>();
    }    

    internal string ToTaggedString()
    {
        return MarkupFormatter.GetTaggedString(Content);
    }

    public void CueCharToCueString()
    {
        StringContent = new List<CueString>();
        if (Content.Count==0)
        {
            return;
        }
        CueString current = new CueString(Content[0]);

        for (int i = 1; i < Content.Count; i++)
        {
            if (!current.AddCueChar(Content[i]))
            {
                StringContent.Add(current);
                current = new CueString(Content[i]);
            }
        }
        StringContent.Add(current);
    }

    public void CueStringToCueChar()
    {
        Content = new List<CueChar>();
        foreach (var cueString in StringContent)
        {
            foreach (char c in cueString.s)
            {
                Content.Add(new CueChar(c,cueString.elements)); 
            }
        } 
    }

    public string RawText()
    {
        string rawText = "";

        foreach (CueChar cueChar in Content)
        {
            rawText += cueChar.c;
        }
        return rawText;
    }
    public string HighlightedText(Element _element)
    {
        if (_element.IsEmpty())
        {
            return RawText();
        }

        string rawText = "";
        bool isHighlighted = false;

        foreach (CueChar cueChar in Content)
        {
            bool elementMatch = cueChar.elements.Exists(e => e.Match(_element));

            if (elementMatch && !isHighlighted)
            {
                rawText += "<color=#FFFF00><u>";//can add colors 
                isHighlighted = true;
            }
            else if (!elementMatch && isHighlighted)
            {
                rawText += "</u></color>";
                isHighlighted = false;
            }
            rawText += cueChar.c;
        } 
        if (isHighlighted)
        {
            rawText += "</mark>";
        }

        return rawText;
    }



}