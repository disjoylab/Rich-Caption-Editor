using System;
using System.Collections.Generic;
using UnityEngine;

internal class VTTHeaderParser
{
    internal static string ProcessHeaderBlock(List<string> lines)
    {        
        string Details = "";
        if (lines.Count >0 &&lines[0].StartsWith("WEBVTT"))
        {
            lines[0] = lines[0].Substring(6);
        }
        foreach (var line in lines)
        {
           Details += line +"\n";
        }
        return Details;
    }
}