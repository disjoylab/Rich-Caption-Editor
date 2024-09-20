using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class CaptionRenderer : MonoBehaviour
{
    public TextMeshProUGUI TMP_GUI;
   
    
    CueGroup myCueGroup;
 
    Cue CurrentCue;

    //region settings - THE BOX
    public float regionWidth;
    public int regionLines;
    public Vector2 regionAnchor;
    public Vector2 regionViewportAnchor;
    public bool regionScroll;
    public float MaxFontSize;//dynamically calculated based on font sizes

    //Cue Settings - INSIDE THE BOX
    public Cue_AlignTypes Cue_AlignType;
    public Cue_LineAlignTypes Cue_LineAlignType;
    public int CueLine = 1;//TBD  - moves box up or down where line is related region lines vs diplayed lines
    public float CuePostion = .5f;//top bottom margin location
    public float CueSize = 1f;//margins


    //Character Settings - THE TEXT
    public List<CharSetting> CharSettings; //list of CharSettins used to modify text attributes dynamically (scale, color, position)
    public List<CueChar> BIUS_CueChars; //list of CueChar used to create tagged strings that TMPro can display (bold, italic, underline, strikethrouh, font size)

    private TMP_TextInfo textInfo;

    private void Start()
    {
        SetDefaultRegion();
        CharSettings = new List<CharSetting>();
        VideoManager.CurrentTimeChanged += OnCurrentTimeChanged;
        ProjectManager.ProjectChanged += OnProjectChanged;
        StylesMenu.StyleChanged += OnStyleChanged;
        ElementManager.ElementsChanged += OnElementsChanged;
        FeatureManager.FeaturesChanged += OnFeaturesChanged;
        CueGroupsMenu.CueGroupsChanged += OnCueGroupsChanged;
        CueUI.CurrentCueChanged += OnCurrentCueChanged;
    }

    private void OnDestroy()
    {
        VideoManager.CurrentTimeChanged -= OnCurrentTimeChanged;
        ProjectManager.ProjectChanged -= OnProjectChanged;
        StylesMenu.StyleChanged -= OnStyleChanged;
        ElementManager.ElementsChanged -= OnElementsChanged;
        FeatureManager.FeaturesChanged -= OnFeaturesChanged;
        CueGroupsMenu.CueGroupsChanged -= OnCueGroupsChanged;
        CueUI.CurrentCueChanged += OnCurrentCueChanged;
    }

    private void OnCurrentCueChanged()
    {
        ResolveCue();
        DisplayAll();
    }

    private void OnFeaturesChanged()
    {
        ResolveCue();
        DisplayAll();
    }

    private void OnElementsChanged()
    {
        ResolveCue();
        DisplayAll();
    }

    private void OnStyleChanged()
    {
        ResolveCue();
        DisplayAll();
    }

    private void OnProjectChanged()
    {
        ResolveCue();
        DisplayAll();
    }

    private void OnCueGroupsChanged()
    {
        ResolveCue();
        DisplayAll();
    }
    public void Configure(CueGroup _cueGroup)
    {
        myCueGroup = _cueGroup; 
    }
    private void OnCurrentTimeChanged(double _currentTime)
    {
        
        if (myCueGroup == null)
        {
            TMP_GUI.rectTransform.sizeDelta = Vector2.zero;
            return;
        } 
        foreach (Cue cue in myCueGroup.Cues)
        {
            if (cue.StartTime <= _currentTime&& cue.EndTime > _currentTime)
            {                
                if (cue != CurrentCue)
                {
                    CurrentCue = cue;
                    ResolveCue();
                }
                DisplayAll();
                return;
            }
        }
        CurrentCue = null;
        TMP_GUI.rectTransform.sizeDelta = Vector2.zero;
    }

    private void ResolveCue()
    {
        CharSettings.Clear();
        SetDefaultCue();
        SetDefaultRegion();
        if (CurrentCue==null)
        {
            return;
        }
        ResolveCueElements();//<--- settings at the cue level 
        ResolveCueCharSettings();//<--- settings at the char level color and style of each character in the text
        GenerateBIUS(); //list of elements with b,i,u,s,size used to generate tags, resolves relevant settings like strikthrough not the projects element strings like 's' that could map to other things
    }

    private void DisplayAll()
    {
        TMP_GUI.text = "";
        if (CurrentCue == null || CharSettings == null || CharSettings.Count==0)
        {
            return;
        }
        textInfo = TMP_GUI.textInfo;
        DisplayCueSettings();//alignment of text in box
        DisplayTextMarkup();//BUIS/font size tags in TMP                
        DisplayRegion();  //Location and size of box, dependent on resolved text & size
        DisplayTextMesh();//colors, size, and positions of individual characters' mesh (for animations)
    }

    private void SetDefaultCue()
    {
        Cue_AlignType = Cue_AlignTypes.center;
        Cue_LineAlignType = Cue_LineAlignTypes.center;
        CueLine = 1;
        CuePostion = .5f;
        CueSize = 1f;
    }

    private void SetDefaultRegion()
    {
        regionWidth = .9f;
        regionLines = 3;
        regionAnchor = new Vector2(.5f, 1f);
        regionViewportAnchor = new Vector2(.5f, .95f);
        regionScroll = false;
    }
    private void ResolveCueElements()
    {
        foreach (Style style in ProjectManager.Instance.CurrentRCEProject.GetCurrentStyleGroup().Styles)
        {
            if (style.element.Match(CurrentCue.RegionElement))
            {
                Feature feature = style.GetFeature();
                if (feature != null)
                {
                    foreach (Setting setting in feature.Settings)
                    {
                        switch (setting.SettingType)
                        {
                             
                            case SettingTypes.Region_Width:
                                regionWidth = ((float)setting.IntValue_1) / 100;
                                break;
                            case SettingTypes.Region_Lines:
                                regionLines = setting.IntValue_1;
                                break;
                            case SettingTypes.Region_RegionAnchor:
                                regionAnchor = (Vector2)setting.PositionValue_1 / 100f;
                                break;
                            case SettingTypes.Region_ViewportAnchor:
                                regionViewportAnchor = (Vector2)setting.PositionValue_1 / 100f;
                                break;
                            case SettingTypes.Region_Scroll:
                                regionScroll = setting.Region_ScrollType == Region_ScrollTypes.Up;
                                break;
                            case SettingTypes.Region_RegionAnchorX:
                                regionAnchor.x = ((float)setting.IntValue_1) / 100;
                                break;
                            case SettingTypes.Region_RegionAnchorY:
                                regionAnchor.y = ((float)setting.IntValue_1) / 100;
                                break;
                            case SettingTypes.Region_ViewportAnchorX:
                                regionViewportAnchor.x = ((float)setting.IntValue_1) / 100;
                                break;
                            case SettingTypes.Region_ViewportAnchorY:
                                regionViewportAnchor.y = ((float)setting.IntValue_1) / 100;
                                break;                            
                        }
                    }
                }
            }
            if (style.element.Match(CurrentCue.CueElement))
            {
                Feature feature = style.GetFeature();
                if (feature != null)
                {
                    foreach (Setting setting in feature.Settings)
                    {
                        switch (setting.SettingType)
                        {

                            case SettingTypes.Cue_Align:
                                Cue_AlignType = setting.Cue_AlignType;
                                break;
                            case SettingTypes.Cue_Line:
                                CueLine = setting.IntValue_1;
                                break;
                            case SettingTypes.Cue_Position:
                                CuePostion = ((float)setting.IntValue_1) / 100;
                                break;
                            case SettingTypes.Cue_Size:
                                CueSize = ((float)setting.IntValue_1) / 100;
                                break;
                            case SettingTypes.Cue_Vertical:
                                break;
                            case SettingTypes.Cue_SnapToLines:
                                break;
                            case SettingTypes.Cue_LineAlign:
                                Cue_LineAlignType = setting.Cue_LineAlignType;
                                break;                             
                        }
                    }
                }
            }
        }
    }

    private void GenerateBIUS() //list of CueChar used to create tagged strings that TMPro can display
    {
        BIUS_CueChars = new List<CueChar>();
        foreach (CharSetting charSetting in CharSettings)
        {
            CueChar cc = new CueChar(charSetting.c);
            if (charSetting.Bold)
            {
                cc.elements.Add(new Element("b", ""));
            }
            if (charSetting.Italic)
            {
                cc.elements.Add(new Element("i", ""));
            }
            if (charSetting.Underline)
            {
                cc.elements.Add(new Element("u", ""));
            }
            if (charSetting.Strikethrough)
            {
                cc.elements.Add(new Element("s", ""));
            }
            cc.elements.Add(new Element("size", charSetting.FontSize.ToString())); //FWIW this is a unity TextMeshPro element, not an RCE element
            BIUS_CueChars.Add(cc);
        }
    }


    private void ResolveCueCharSettings()
    {
        MaxFontSize = 1;
        foreach (var cueChar in CurrentCue.GetCurrentTextSegment().Content)
        {
            CharSetting charSetting = new CharSetting(cueChar);
            foreach (Style cueGroupStyle in ProjectManager.Instance.CurrentRCEProject.GetCurrentStyleGroup().Styles)
            {
                List<Element> elements = new List<Element>(); 
                elements.Add(CurrentCue.TextElement);
                elements.AddRange(cueChar.elements);
                if (cueGroupStyle.element.Match(elements))
                {
                    Feature feature = cueGroupStyle.GetFeature();
                    if (feature != null)
                    {
                        charSetting.AddStyleSettings(feature.Settings);
                    }
                }
            }
            if (charSetting.FontSize > MaxFontSize)
            {
                MaxFontSize = charSetting.FontSize; //capture highest font size for line spacing
            }
            CharSettings.Add(charSetting);
        }
    }

    private void DisplayTextMarkup()
    {
        TMP_GUI.text = MarkupFormatter.GetTaggedString(BIUS_CueChars);//set the text markup        
    }


    private void DisplayTextMesh()
    {
        TMP_GUI.ForceMeshUpdate();
        for (int i = 0; i < CharSettings.Count; i++)
        {
            SetChar(i, 1f, CharSettings[i].TextColor);//set the size for animating (this is not font size) and color (no tmp tag for color, also for animation)... TODO: Add position
        }
        // TMP_GUI.canvasRenderer.SetMesh(TMP_GUI.mesh);
        //TMP_GUI.UpdateVertexData(TMP_VertexDataUpdateFlags.All);          
        TMP_GUI.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32 | TMP_VertexDataUpdateFlags.Vertices);

    }


    public void SetChar(int index, float size, Color32 color)
    {
        if (index < 0 || index >= textInfo.characterInfo.Length) return;

        TMP_CharacterInfo charInfo = textInfo.characterInfo[index];
        if (!charInfo.isVisible) return;

        int materialIndex = charInfo.materialReferenceIndex;
        int vertexIndex = charInfo.vertexIndex;

        // Set color
        Color32[] newVertexColors = textInfo.meshInfo[materialIndex].colors32;
        newVertexColors[vertexIndex + 0] = color;
        newVertexColors[vertexIndex + 1] = color;
        newVertexColors[vertexIndex + 2] = color;
        newVertexColors[vertexIndex + 3] = color;

        // Set size
        Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;
        Vector3 offset = (vertices[vertexIndex + 0] + vertices[vertexIndex + 2]) / 2;
        for (int i = 0; i < 4; i++)
        {
            vertices[vertexIndex + i] = (vertices[vertexIndex + i] - offset) * size + offset;
        }
    }

    private void DisplayCueSettings()
    {
        bool isNone = Cue_AlignType == Cue_AlignTypes.none;
        bool isLeft = Cue_AlignType == Cue_AlignTypes.start || Cue_AlignType == Cue_AlignTypes.left;
        bool isCenter = Cue_AlignType == Cue_AlignTypes.center;
        bool isRight = Cue_AlignType == Cue_AlignTypes.end || Cue_AlignType == Cue_AlignTypes.right;

        bool isTop = Cue_LineAlignType == Cue_LineAlignTypes.start;
        bool isCenterLine = Cue_LineAlignType == Cue_LineAlignTypes.center;
        bool isBottom = Cue_LineAlignType == Cue_LineAlignTypes.end;
        if (isNone && isTop) { TMP_GUI.alignment = TextAlignmentOptions.Top; }
        else if (isNone && isCenterLine) { TMP_GUI.alignment = TextAlignmentOptions.Center; }
        else if (isNone && isBottom) { TMP_GUI.alignment = TextAlignmentOptions.Bottom; }

        else if (isLeft && isTop) { TMP_GUI.alignment = TextAlignmentOptions.TopLeft; }
        else if (isLeft && isCenterLine) { TMP_GUI.alignment = TextAlignmentOptions.Left; }
        else if (isLeft && isBottom) { TMP_GUI.alignment = TextAlignmentOptions.BottomLeft; }

        else if (isCenter && isTop) { TMP_GUI.alignment = TextAlignmentOptions.Top; }
        else if (isCenter && isCenterLine) { TMP_GUI.alignment = TextAlignmentOptions.Center; }
        else if (isCenter && isBottom) { TMP_GUI.alignment = TextAlignmentOptions.Bottom; }

        else if (isRight && isTop) { TMP_GUI.alignment = TextAlignmentOptions.TopRight; }
        else if (isRight && isCenterLine) { TMP_GUI.alignment = TextAlignmentOptions.Right; }
        else if (isRight && isBottom) { TMP_GUI.alignment = TextAlignmentOptions.BottomRight; }

    }

    private void DisplayRegion()
    {
        if (CharSettings ==null || CharSettings.Count==0)
        {
            TMP_GUI.rectTransform.sizeDelta = Vector2.zero;//no text
            return;
        }

        float MinMargin = 7;

        //SET POSITION AND ANCHOR
        TMP_GUI.rectTransform.pivot = new Vector2(regionAnchor.x, 1 - regionAnchor.y);
        TMP_GUI.rectTransform.anchoredPosition = regionViewportAnchor * new Vector2(640f, -480f);

        // SET WIDTH
        Vector2 Size = new Vector2(regionWidth * 640, 480);
        TMP_GUI.rectTransform.sizeDelta = Size;

        //SET LR MARGINS
        Vector4 margins = TMP_GUI.margin;
        float lrMargins = (Size.x * (1 - CueSize));
        margins.x = MathF.Max(MinMargin, (lrMargins * .5f));
        margins.z = MathF.Max(MinMargin, (lrMargins * .5f));
        margins.y = 0;
        margins.w = 0;
        TMP_GUI.margin = margins;

        //GET EFFECTIVE SIZE OF THE TEXT
        Vector2 CurrentTextSize = GetTextSize();

        //RESET WIDTH  
        Size.x = Mathf.Min(CurrentTextSize.x + margins.x + margins.z, Size.x);//adding 10 so text does not resize 
        Size.y = CurrentTextSize.y;
        TMP_GUI.rectTransform.sizeDelta = Size;

        //GET EFFECTIVE SIZE OF THE TEXT
        CurrentTextSize = GetTextSize();

        //SET HEIGHT BASED ON THE NUMBER OF LINES 
        float targetHeight = CurrentTextSize.y;
        if (textInfo.lineCount > regionLines)
        {
            targetHeight = 0;
            for (int i = 0; i < regionLines; i++)
            {
                targetHeight += textInfo.lineInfo[i].lineHeight;
            }
        }

        float tbMargins = (targetHeight * (1 - CueSize)) / CueSize;
        tbMargins = MathF.Max(tbMargins, MinMargin * 2);//normal is t=minmargin b=minmargin but 0% or 100% would put that against the top/bottom
        margins.y = tbMargins * CuePostion;
        margins.w = tbMargins * (1 - CuePostion);
        TMP_GUI.margin = margins;

        Size.y = targetHeight + margins.y + margins.w;
        TMP_GUI.rectTransform.sizeDelta = Size;
    }

    private Vector2 GetTextSize()
    {
        TMP_GUI.ForceMeshUpdate();
        return TMP_GUI.textBounds.size;
    }

    public static Color GetRainbowColor(float c)
    {
        float t = c % 1;
        return Color.HSVToRGB(t, 1.0f, 1.0f);
    }
}
