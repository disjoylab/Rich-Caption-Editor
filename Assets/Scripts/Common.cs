using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Common
{
    public static string ElementName_TimeStamp = "TimeStamp"; //to maintain consistant naming convention for storing in-line timestamps
    public static string FloatToTimeString(float startTime)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(startTime);
        return timeSpan.ToString(@"hh\:mm\:ss\.fff");
    }

    public static float TimeStringToFloat(string timeString)
    { 
        string[] formats = { @"mm\:ss\.fff", @"hh\:mm\:ss\.fff", @"mm\:ss\,fff", @"hh\:mm\:ss\,fff" };  

        foreach (var format in formats)
        {
            if (TimeSpan.TryParseExact(timeString, format, null, out TimeSpan timeSpan))
            {
                return (float)timeSpan.TotalSeconds;
            }
        }
        if (float.TryParse(timeString, out float seconds))
        {
            return seconds;
        }
        return -1;  
    }
     
    public static List<CueChar> DeepCopyCueCharList(List<CueChar> originalList)
    {
        // Creates a new instance (a deep copy) of the original CueChar list
        List<CueChar> copy = new List<CueChar>();
        foreach (CueChar cc in originalList)
        {
            copy.Add(cc.Copy());
        }
        return copy;
    }

    public static List<CueChar> FilteredCopyCueCharList(List<CueChar> originalList, List<string> list)
    {
        List<CueChar> copy = new List<CueChar>();
        foreach (CueChar cc in originalList)
        {
            copy.Add(cc.FilteredCopy(list));
        }
        return copy;
    } 

}