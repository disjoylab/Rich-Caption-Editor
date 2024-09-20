using System; 
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class CueUI : MonoBehaviour
{ 
    public static Cue CurrentCue;
    public TMP_InputField textInputField;
    public static Action CurrentCueChanged;
    public static bool CurrentCueHasChanges;
   static bool ChangedCue;
    public TMP_Dropdown Text_Element_Dropdown;//  TODO ************************** REPLACE WITH ELEMENT EDITING OBJECT  
    public List<Element> TextElements;
    public MouseClickHandler textBoxMouseClickHandler;
    private void Update()
    {
        if (CurrentCueHasChanges)
        {
            CurrentCueHasChanges = false;
            CurrentCueChanged?.Invoke();
            DisplayTextCue();
        }
        if (ChangedCue)
        {
            ChangedCue = false;
            SetTextDropdown();
        }
    }
    private void Start()
    {
        textInputField.onValueChanged.AddListener(OnTextChanged);
        textBoxMouseClickHandler.onMouseUp += OnEndEdit;
        VideoManager.CurrentTimeChanged += OnCurrentTimeChanged;

    }
    private void OnDestroy()
    {
        VideoManager.CurrentTimeChanged -= OnCurrentTimeChanged;
    }

    private void OnCurrentTimeChanged(double obj)
    {
        DisplayTextCue();
    }

    void OnTextChanged(string newText)
    {
        if (CurrentCue==null)
        {
            return;
        }
        newText = RemoveMarkTagsFromString(newText);
        string currentText =  CurrentCue.GetCurrentTextSegment().RawText(); // Get the current text
        if (newText==currentText)
        {
            return;
        }
        List<CueChar> cueCharList = CurrentCue.GetCurrentTextSegment().Content; // Get the current cue chars
                
        // Find the start index where the text differs
        int startIndex = 0;
        while (startIndex < currentText.Length && startIndex < newText.Length && currentText[startIndex] == newText[startIndex])
        {
            startIndex++;
        }

        // Find the end index where the text differs
        int endIndexCurrent = currentText.Length - 1;
        int endIndexNew = newText.Length - 1;
        while (endIndexCurrent >= startIndex && endIndexNew >= startIndex && currentText[endIndexCurrent] == newText[endIndexNew])
        {
            endIndexCurrent--;
            endIndexNew--;
        }
         
        if (endIndexCurrent >= startIndex)
        {
           cueCharList.RemoveRange(startIndex, endIndexCurrent - startIndex + 1);
           CurrentCueHasChanges = true;
        }
         
        if (endIndexNew >= startIndex)
        {
            CueChar referenceCueChar = startIndex > 0 ? cueCharList[startIndex - 1] : new CueChar(' '); 

            for (int i = startIndex; i <= endIndexNew; i++)
            {
                CueChar newCueChar = referenceCueChar.Duplicate(newText[i]); 
                cueCharList.Insert(i, newCueChar);
                CurrentCueHasChanges = true;
            }
        }
    }
    void OnEndEdit() //triggered from mouseup gives start and end positions of highlighted text
    {
        int focusPos = textInputField.selectionFocusPosition;
        int anchorPos = textInputField.selectionAnchorPosition;
        int selectionStart = Mathf.Min(anchorPos, focusPos);
        int selectionEnd = Mathf.Max(anchorPos, focusPos);
        int firstClickIndex = Mathf.Clamp( anchorPos,selectionStart,selectionEnd-1);
         

        if (selectionStart != selectionEnd)
        {
            bool inRange = selectionStart >= 0 && selectionEnd <= CurrentCue.GetCurrentTextSegment().Content.Count;
            if (inRange)
            { 
                Element element = GetSelectedElement();
                bool remove = CurrentCue.GetCurrentTextSegment().Content[firstClickIndex].elements.Exists(e => e.IsEqual(element));
                for (int i = selectionStart; i < selectionEnd; i++)
                {
                    if (remove)
                    {
                        CurrentCue.GetCurrentTextSegment().Content[i].RemoveElement(element);
                    }
                    else
                    {
                        CurrentCue.GetCurrentTextSegment().Content[i].AddElement(element);
                    }
                }
            }
        }         
        CurrentCueHasChanges = true;
    }

    public void DisplayTextCue()
    {
        if (CurrentCue!=null)
        {
            Element e = GetSelectedElement();

            textInputField.SetTextWithoutNotify(CurrentCue.GetCurrentTextSegment().HighlightedText(e)); //NEED TO CONVERT THIS TO HIGHLIGHT SPECIFIC CUECHARs LIKE ONLY ELEMENT B OR ONLY ATTRIBUTE .GREEN    
        }
        else
        {
            textInputField.text = "";
        }
    }

    private Element GetSelectedElement()
    {
        Element e = new Element("");
        int index = Text_Element_Dropdown.value;
        if (index < TextElements.Count)
        {
            e = TextElements[index];
        }

        return e;
    }

    public static void SetCurrentCue(Cue _cue)
    {
        if (CurrentCue != _cue)
        {
            CurrentCue = _cue;
            CurrentCueHasChanges = true;
            ChangedCue = true;
        }
    }
    private void SetTextDropdown()
    {
        List<Style> Styles = ProjectManager.Instance.CurrentRCEProject.GetCurrentStyleGroup().GetStylesByType(FeatureFilter.Text);
        TextElements.Clear();
        TextElements.Add(new Element(""));
        foreach (var style in Styles)
        {
            TextElements.Add(style.element);
        }
        List<string> options = new List<string>();
        foreach (var element in TextElements)
        {
            options.Add(element.Signature.ToString());
        }
        int index = Text_Element_Dropdown.value;
        Text_Element_Dropdown.ClearOptions();
        Text_Element_Dropdown.AddOptions(options);
        if (index >= 0 && index < options.Count)
        {
            Text_Element_Dropdown.SetValueWithoutNotify(index);
        }
        else
        { 
            Text_Element_Dropdown.SetValueWithoutNotify(0);   
        }
    }
    string RemoveMarkTagsFromString(string input)
    {
        return System.Text.RegularExpressions.Regex.Replace(input, "<color=.*?>|</color>|<u>|</u>", "");
    }

}