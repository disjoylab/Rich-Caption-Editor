
using System.Collections.Generic;

// Provides methods to format and filter markup strings from a list of CueChar objects.

public class MarkupFormatter
{
    public static string GetFilteredTaggedString(List<CueChar> cueChars, List<string> list)
    {
        //cueChars: The list of CueChar objects to be formatted.
        //list: The list of tags to filter the CueChar objects.
        if (cueChars.Count == 0)
        {
            return "";
        }
        List<CueChar> cueCharsCopy = Common.FilteredCopyCueCharList(cueChars,list);

        return GetTaggedStringInternal(cueCharsCopy);
    }
    public static string GetTaggedString(List<CueChar> cueChars)
    {
        // Returns a tagged string from the list of CueChar objects.

        if (cueChars.Count == 0)
        {
            return "";
        }
        List<CueChar> cueCharsCopy = Common.DeepCopyCueCharList(cueChars);

        return GetTaggedStringInternal(cueCharsCopy);
    }

     static string GetTaggedStringInternal(List<CueChar> cueChars)
    {
        // Internal method to generate a tagged string from the list of CueChar objects.
        if (cueChars.Count == 0)
        {
            return "";
        }

        int splitOnIndex = 0;
        Element longestSpanElement = Element.NewElement();
        int longestSpanLength = -1;

        if (cueChars[0].elements.Count == 0)
        {
            splitOnIndex = 1;
            while (splitOnIndex < cueChars.Count && cueChars[splitOnIndex].elements.Count == 0)
            {
                splitOnIndex++;
            }
        }
        else
        {
            foreach (Element e in cueChars[0].elements)
            {
                int currentSpanLength = GetElementSpan(cueChars, e);
                if (currentSpanLength > longestSpanLength)
                {
                    longestSpanElement = e;
                    longestSpanLength = currentSpanLength;
                }
            }
            splitOnIndex = longestSpanLength;
        }

        List<CueChar> elementMatches = cueChars.GetRange(0, splitOnIndex);
        List<CueChar> remaining = cueChars.GetRange(splitOnIndex, cueChars.Count - splitOnIndex);
        if (longestSpanElement.IsEmpty())
        {
            return GetStringFromCueCharList(elementMatches) + GetTaggedStringInternal(remaining);
        }

        foreach (CueChar cc in elementMatches)
        {
            cc.elements.RemoveAll(e => e.IsEqual(longestSpanElement));
        }

        return longestSpanElement.StartTag() + GetTaggedString(elementMatches) + longestSpanElement.EndTag() + GetTaggedStringInternal(remaining);
    }

    public static int GetElementSpan(List<CueChar> cueChars, Element element)
    {
        int spanLength = 0;
        foreach (var cc in cueChars)
        {
            if (cc.elements.Exists(e => e.IsEqual(element)))
            {
                spanLength++;
            }
            else
            {
                break;
            }
        }
        return spanLength;
    }

    public static string GetStringFromCueCharList(List<CueChar> cueChars)
    {
        string result = "";
        for (int i = 0; i < cueChars.Count; i++)
        {
            result += cueChars[i].c;
        }
        return result;
    }
   

}
