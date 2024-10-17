using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

[Serializable]
public class Cue
{
    public string Identifier;
    public float StartTime;
    public float EndTime;
    public string RegionFeature; 
    public Element CueElement; 
    public List<TextSegment> TextSegments;//TEXT VARIATIONS
    public int CurrentTextSegment = 0;

    public static Action<Cue> CueChanged;
    [JsonIgnore]
    public bool Deleted { get; private set; } = false;

    public void MarkAsDeleted()
    {
        Deleted = true;
        TriggerChanged();
    }

    public void TriggerChanged()
    {
        CueChanged?.Invoke(this);
    }

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
        RegionFeature = "";
        CueElement = new Element(""); 
        TextSegments = new List<TextSegment>();


        TextSegment Default = new TextSegment();
        TextSegments.Add(Default);
    }

    internal Cue Copy()
    {
        Cue copy = new Cue(Identifier);
        copy.StartTime = StartTime;
        copy.EndTime = EndTime;
        copy.RegionFeature = RegionFeature;
        copy.CueElement = CueElement.Copy(); 
        copy.TextSegments = TextSegments.Select(segment => new TextSegment
        {
            Content = segment.Content.Select(cueChar => cueChar.Copy()).ToList()
        }).ToList();
        copy.CurrentTextSegment = CurrentTextSegment;
        return copy;
    }    
}


