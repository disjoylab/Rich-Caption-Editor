using System;
using System.Collections.Generic;

[Serializable]
public class CueGroup
{
    public string Name;
     
    public List<Cue> Cues;
    public List<string> TextVariations;//ADDING A TEXT VARIATION SHOULD CREATE TEXT VARIATIONS IN THE CUES
    public int CurrentVariation = 0; //clamp to TextVariations & update all cues on change
    public CueGroup(string _name)
    {
        Name = _name; 
        Cues = new List<Cue>();
        TextVariations = new List<string>();
        TextVariations.Add("Standard English");
    }
    public CueGroup()
    {
        Name = "";
        Cues = new List<Cue>();
        TextVariations = new List<string>(); 
    }
    public CueGroup(CueGroup _cueGroup)
    {
        Name = _cueGroup.Name;

        Cues = new List<Cue>();
        foreach (var cue in _cueGroup.Cues)
        {
            Cues.Add(cue.Copy());
        }
         
        TextVariations = new List<string>(_cueGroup.TextVariations);
         
        CurrentVariation = _cueGroup.CurrentVariation;
    }
}