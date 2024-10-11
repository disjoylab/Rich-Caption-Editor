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

    public List<ElementTool> ElementTools;
    public GameObject ElementToolPrefab;
    public Transform ElementToolContainer;
    
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
        }
    }
    private void Start()
    {
        ElementTools = new List<ElementTool>();
        textInputField.onValueChanged.AddListener(OnTextChanged);
        textBoxMouseClickHandler.onMouseUp += OnEndEdit;
        VideoManager.CurrentTimeChanged += OnCurrentTimeChanged;
        ElementTool.ElementToolsChanged += OnElementToolsChanged;
    }
    private void OnDestroy()
    {         
        textBoxMouseClickHandler.onMouseUp -= OnEndEdit;
        VideoManager.CurrentTimeChanged -= OnCurrentTimeChanged;
        ElementTool.ElementToolsChanged -= OnElementToolsChanged;
    }

    private void OnElementToolsChanged()
    {
        DisplayTextCue();
    }

    private void OnEnable()
    {
        SetElmentTools();
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
        if (ElementTool.SelectedElementTool != null)
        {
            return ElementTool.SelectedElementTool.GetElement();
        }
        return new Element("");
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
    private void SetElmentTools()
    { 
        foreach (var elementTool in ElementTools)
        {
            Destroy(elementTool.gameObject);
        }
        //move tools to correct postions
        //resize tool container to fit tools
        List<string> textElementNames = new List<string>(); 
        textElementNames.AddRange( ElementManager.GetTextLevelElementGroupNames());
        float xPostion = 5;
        foreach (string textElementName in textElementNames)
        {
            GameObject go = Instantiate(ElementToolPrefab, ElementToolContainer);
            ElementTool elementTool = go.GetComponent<ElementTool>();
            elementTool.Configure(textElementName );
            ElementTools.Add(elementTool);
            RectTransform rt = elementTool.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(xPostion, -5);
            xPostion += rt.sizeDelta.x + 10;
        }
        ElementToolContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(xPostion + 10, 100);

          
        
    }
    string RemoveMarkTagsFromString(string input)
    {
        return System.Text.RegularExpressions.Regex.Replace(input, "<color=.*?>|</color>|<u>|</u>", "");
    }

}