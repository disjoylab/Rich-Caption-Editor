
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class VTTParser
{
    static RCEProject rceProject = new RCEProject("Speech");
    public static RCEProject LoadVTT(string filePath)
    {
        rceProject = new RCEProject("Speech");
        var currentBlock = new List<string>();

        using (var reader = new StreamReader(filePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    currentBlock.Add(line);
                }
                else
                {
                    if (currentBlock.Count > 0)
                    { 
                        ProcessBlock(currentBlock);
                    }
                    currentBlock.Clear();
                }
            }
            if (currentBlock.Count > 0)
            {
                ProcessBlock(currentBlock);
            }
        }
        return rceProject;
    }

    public static void ProcessBlock(List<string> Lines)
    { 
        if (Lines.Count == 0) return;

        if (Lines[0].StartsWith("WEBVTT"))
        {
            
          rceProject.Details = VTTHeaderParser.ProcessHeaderBlock(Lines);
        }
        else if (Lines[0].StartsWith("NOTE"))
        {
            foreach (var line in Lines)
            {
                //Debug.Log("Process NOTE: " + line);
            }
        }
        else if (Lines[0].StartsWith("STYLE"))
        {
            string allStyles = string.Join(" ", Lines); 
            string[] styleRules = allStyles.Split(new string[] { "::cue" }, StringSplitOptions.RemoveEmptyEntries); 
            foreach (var rule in styleRules)
            {
                if (rule.Contains("{"))
                {
                    Style style = VTTStyleParser.ProcessStyleBlock("::cue" + rule);
                    if (style!=null && !style.IsEmpty())
                    {
                        ElementManager.CreateElementGroup(style.element.Signature.Name);
                        rceProject.GetCurrentStyleGroup().Styles.Add(style);
                    }                    
                }                
            }            
        }
        else if (Lines[0].StartsWith("REGION"))
        {
            Style RegionStyle = VttRegionParser.ProcessRegionBlock(Lines);
            if (RegionStyle.GetFeatureName()!= "Not Found" && !RegionStyle.IsEmpty()) ///****************************************************** todo
            {
                ElementManager.CreateElementGroup(RegionStyle.element.Signature.Name);
                rceProject.GetCurrentStyleGroup().Styles.Add(RegionStyle);
            }
        }
        else
        {
            (Cue newCue, Style cueStyle) = VTTCueParser.ProcessCueBlock(Lines);
            if (newCue != null)
            {
                rceProject.CueGroups[0].Cues.Add(newCue);                 
                ElementManager.CreateElementGroup(cueStyle.element.Signature.Name);        
                if (cueStyle!=null&&cueStyle.GetFeatureName()!=null && !cueStyle.IsEmpty())
                {
                    rceProject.GetCurrentStyleGroup().Styles.Add(cueStyle);//maybe check if it exists?
                }
            }
        }
    }             
}
