using System;
using System.Collections.Generic;
using UnityEngine;

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

    public Cue GetCueByTime(double _currentTime, int offest = 0)
    {
        for (int i = 0; i < Cues.Count; i++)
        {
            if (Cues[i].StartTime <= _currentTime && Cues[i].EndTime > _currentTime)
            {
                int index = i + offest;
                if (i >= 0 && i < Cues.Count)
                {
                    return Cues[index];
                }
            }
        }
        return null;
    }

    internal void SetCueStartTime(Cue _cue, float _startTime)
    {
        int cueIndex = Cues.IndexOf(_cue);
        if (cueIndex == -1) return;

        Cue prevCue = cueIndex > 0 ? Cues[cueIndex - 1] : null;

        if (prevCue != null && _startTime <= prevCue.StartTime + 1f)
        {
            _startTime = prevCue.StartTime + 1f;
        }
        if (_startTime > _cue.EndTime - 1)
        {
            _startTime = _cue.EndTime - 1;
        }

        if (prevCue != null && prevCue.EndTime > _startTime)
        {
            prevCue.EndTime = _startTime;
            prevCue.TriggerChanged();
        }

        if (_cue.StartTime != _startTime)
        {
            _cue.StartTime = _startTime;
            _cue.TriggerChanged();
        }
    }

    internal void SetCueEndTime(Cue _cue, float _endTime)
    {
        int cueIndex = Cues.IndexOf(_cue);
        if (cueIndex == -1) return;

        Cue nextCue = cueIndex < Cues.Count - 1 ? Cues[cueIndex + 1] : null;

        if (nextCue != null && _endTime >= nextCue.EndTime - 1f)
        {
            _endTime = nextCue.EndTime - 1f;
        }
        if (_endTime < _cue.StartTime + 1)
        {
            _endTime = _cue.StartTime + 1;
        }

        if (nextCue != null && nextCue.StartTime < _endTime)
        {
            nextCue.StartTime = _endTime;
            nextCue.TriggerChanged();
        }

        if (_cue.EndTime != _endTime)
        {
            _cue.EndTime = _endTime;
            _cue.TriggerChanged();
        }
    }

    internal bool MoveCue(Cue _cue)
    {
        if (!IsValidSpot(_cue)) return false;

        Cues.Remove(_cue);

        for (int i = 0; i < Cues.Count; i++)
        {
            if (_cue.StartTime < Cues[i].StartTime)
            {
                Cues.Insert(i, _cue);
                SetCueStartTime(_cue, _cue.StartTime);
                SetCueEndTime(_cue, _cue.EndTime);
                _cue.TriggerChanged();//above trigger changes but for the most part only is next/prev cues since the current logic only changes the other cues arount the one moved
                return true;
            }
        }
        Cues.Add(_cue);
        SetCueStartTime(_cue, _cue.StartTime);
        SetCueEndTime(_cue, _cue.EndTime);
        return true;
    }

    internal bool IsValidSpot(Cue _cue)
    {
        foreach (Cue otherCue in Cues)
        {
            if (otherCue == _cue)
                continue;
            if (otherCue.StartTime < _cue.StartTime && otherCue.EndTime > _cue.StartTime)
            {
                if (_cue.StartTime - otherCue.StartTime < 1) return false;
            }
            if (otherCue.StartTime < _cue.EndTime && otherCue.EndTime > _cue.EndTime)
            {
                if (otherCue.EndTime - _cue.EndTime < 1) return false;
            }
            // Check if _cue is completely inside otherCue
            if (_cue.StartTime >= otherCue.StartTime && _cue.EndTime <= otherCue.EndTime) return false;

            // Check if otherCue is completely inside _cue
            if (otherCue.StartTime >= _cue.StartTime && otherCue.EndTime <= _cue.EndTime) return false;
        }
        return true;
    }

    internal void DeleteCue(Cue _cue)
    {
        _cue.MarkAsDeleted();
        Cues.Remove(_cue);
    }
}