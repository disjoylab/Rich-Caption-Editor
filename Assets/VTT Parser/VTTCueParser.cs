using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

internal class VTTCueParser
{
    public static (Cue, Style) ProcessCueBlock(List<string> lines)
    {
        if (lines.Count == 0)
        {
            return (null, null);
        }
        int startLine = 1;
        string identifier = "";
        string header = "";
        if (lines[0].Contains("-->"))
        {
            header = lines[0];
        }
        else if (lines.Count > 1 && lines[1].Contains("-->"))
        {
            identifier = lines[0];
            header = lines[1];
            startLine = 2;
        }
        else
        {
            return (null, null);
        }
        (Cue cue, Style style) = ParseHeader(identifier, header);

        for (int i = startLine; i < lines.Count; i++)
        {
            TextSegment textSegment = cue.GetCurrentTextSegment();
            textSegment.Content.AddRange(GetCueCharList(lines[i]));
            textSegment.Content.Add(new CueChar('\n'));
        }
        return (cue, style);
    }

    static List<CueChar> GetCueCharList(string textElementString)
    {
        List<CueChar> cueChars = new List<CueChar>();
        List<Element> activeElements = new List<Element>();
        int length = textElementString.Length;

        for (int i = 0; i < length; i++)
        {
            if (textElementString[i] == '<')
            {
                int tagEnd = textElementString.IndexOf('>', i);
                if (tagEnd == -1)
                {
                    break; // Malformed tag, stop processing
                }

                string tag = textElementString.Substring(i, tagEnd - i + 1);

                if (tag.StartsWith("</"))
                {
                    // End tag
                    string elementName = tag.Substring(2, tag.IndexOf('>') - 2).Trim();
                    activeElements.RemoveAll(e => e.Name == elementName);
                }
                else
                {
                    Element element = GetElement(tag);
                    if (element.Name.ToLower() == "v" || element.Name.ToLower() == "voice" || element.Name == Common.ElementName_TimeStamp)//THINGS THERE CAN BE ONLY ONE OF (AND DOES NOT OFTEN HAVE AN END TAG </v>
                    {
                        activeElements.RemoveAll(e => e.Name == element.Name);
                    }
                    activeElements.Add(element);
                }

                i = tagEnd; 
            }
            else
            {
                cueChars.Add(new CueChar(textElementString[i], activeElements.Select(e => e.Copy()).ToList()));
            }
        }

        return cueChars;
    }

    static Element GetElement(string webVTTString)
    {
        Element element = Element.NewElement();
        string trimmed = webVTTString.Trim('<', '>').Trim();
        string[] parts = trimmed.Split(new char[] { ' ' }, 2);

        if (parts.Length > 0)
        {
            string name = "";
            string value = "";
            float timeStamp = Common.TimeStringToFloat(parts[0]);
            if (timeStamp > -1)
            {
                name = Common.ElementName_TimeStamp;
                value = timeStamp.ToString();
            }
            else
            {
                 
                name = parts[0];
                value = (parts.Length > 1) ? parts[1] : "";
                 
            }
            element.Name = name;
            element.Value = value;
        } 
        return element;
    }

    static (Cue, Style) ParseHeader(string identifier, string WebVttHeader)
    {
        Cue cue = new Cue(identifier);
        Style style = new Style();
        if (WebVttHeader.Contains("-->"))
        {
            WebVttHeader = WebVttHeader.Replace("-->", " ");
            string[] parts = WebVttHeader.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            cue.StartTime = Common.TimeStringToFloat(parts[0]);
            cue.EndTime = Common.TimeStringToFloat(parts[1]); // Adjust to only take the time part before settings
            FeatureGroup featureGroup = new FeatureGroup(FeatureManager.GetUniqueName("Caption Box "));
            Feature feature = new Feature("Imported cue");
            for (int i = 2; i < parts.Length; i++)
            {
                string[] settingParts = parts[i].Split(':');
                if (settingParts.Length == 2)
                {
                    if (settingParts[0].ToLower() == "region")
                    { 
                        cue.RegionFeature =  "Region " + settingParts[1]; 
                    }
                    else
                    {
                        feature.Settings.Add(new Setting("Cue_" + settingParts[0], settingParts[1]));
                    }
                }
            }
            if (feature.Settings.Count > 0)
            {
                FeatureGroup ExistinFeatureGroup = FeatureManager.FindMatch(feature);
                
                if (ExistinFeatureGroup == null)
                {
                    featureGroup.AddFeature(feature, false);
                    featureGroup.Name = FeatureManager.GetUniqueName("Caption Box " + (string.IsNullOrWhiteSpace(identifier) ? "":$"_{identifier}"));                   
                    FeatureManager.AddFeatureGroup(featureGroup);                    
                }
                else
                {
                    featureGroup = ExistinFeatureGroup;
                }
                string elementName  = (string.IsNullOrWhiteSpace(identifier))? featureGroup.Name:identifier;            
                Element e = new Element(elementName); 
                cue.CueElement = e;
                style.element = e ;
                style.SetFeature(featureGroup.Name); 
            }
        }
        return (cue, style);
    }
}