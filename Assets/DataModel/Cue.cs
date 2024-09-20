using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class Cue
{    
    public string Identifier;
    public float StartTime;
    public float EndTime;
    public Element RegionElement;
    public Element CueElement;
    public Element TextElement; 
    public List<TextSegment> TextSegments;//TEXT VARIATIONS
    public int CurrentTextSegment = 0;

    internal TextSegment GetCurrentTextSegment()
    { 
        if (TextSegments==null)
        {
            TextSegments = new List<TextSegment>();
        }
        if (TextSegments.Count< (CurrentTextSegment + 1))
        {
            TextSegments.Add(new TextSegment());
        }
        return TextSegments[CurrentTextSegment];
    }
    
    public Cue(string _identifier)
    { 
        Identifier = _identifier;
        RegionElement = new Element("");
        CueElement = new Element("");
        TextElement = new Element("");
        TextSegments = new List<TextSegment>();


        TextSegment Default = new TextSegment();
        TextSegments.Add(Default);
    }

    internal Cue Copy()
    {
        var copy = new Cue(Identifier);

        copy.StartTime = StartTime;
        copy.EndTime = EndTime;
        copy.RegionElement = RegionElement.Copy();
        copy.CueElement = CueElement.Copy();
        copy.TextElement = TextElement.Copy();
        copy.TextSegments = TextSegments.Select(segment => new TextSegment
        {
            Content = segment.Content.Select(cueChar => cueChar.Copy()).ToList()
        }).ToList();
        copy.CurrentTextSegment = CurrentTextSegment;
        return copy;
    }
}


