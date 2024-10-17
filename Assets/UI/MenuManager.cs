using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
    public enum MenuStates { File, CueGroup, Elements, Features, Styles, COUNT }

public class MenuManager : MonoBehaviour
{
    public static MenuStates CurrentMenuState;
    public static Action<MenuStates> CurrentMenuStateChanged;
    public static Action MenuLayoutUpdated;
    public static float TargetVideoScale; 
    public RectTransform VideoPanel; //
    public RectTransform TopBar;
    public RectTransform MainWindow;
    public RectTransform CueWindow;
    public RectTransform BottomBar;//currently no responsive behavior
    Vector2 ScreenSize = Vector2.zero;
    public bool MainMenuFullscreen;
     
    const float TOP_BAR_HEIGHT = 60;
    const float BOTTOM_BAR_HEIGHT = 85;
    const float VIDEO_WIDTH = 660;
    const float VIDEO_HEIGHT = 500;


    private void Awake()
    {
        TargetVideoScale = 1.15f; 
        CurrentMenuStateChanged += OnCurrentMenuStateChanged;
    }
    private void OnDestroy()
    {
        CurrentMenuStateChanged -= OnCurrentMenuStateChanged;
    }

    private void OnCurrentMenuStateChanged(MenuStates _menuState)
    {
        SetPanelPositions();
    }

    void Start()
    {
        CurrentMenuStateChanged?.Invoke(CurrentMenuState); 
    }

    void Update()
    {
        Vector2 currentScreenSize = new Vector2(Screen.width, Screen.height);
        if (currentScreenSize != ScreenSize)
        {
            ScreenSize = currentScreenSize;
            SetPanelPositions();
        }
        if (VideoPanel.localScale.x != TargetVideoScale)
        {
            SetPanelPositions();
        } 
    } 
    public static void SetCurrentMenuState(MenuStates _menuState)
    {
        if (CurrentMenuState != _menuState)
        {
            CurrentMenuState = _menuState;
            CurrentMenuStateChanged?.Invoke(CurrentMenuState);            
        }
    }

    public void ToggleFullscreen()  
    {
        if (CurrentMenuState != MenuStates.File)
        {
            MainMenuFullscreen = !MainMenuFullscreen;
            SetPanelPositions();
        }
    }

    private void SetPanelPositions()   
    {
        MenuLayoutUpdated?.Invoke();
        if (MainMenuFullscreen||CurrentMenuState==MenuStates.File)
        {
            SetFullScreen();
        }
        else
        {
            SetStandardSize();
        }
    }

    private void SetFullScreen()
    {    

        TopBar.sizeDelta = new Vector2(ScreenSize.x , TOP_BAR_HEIGHT);
        TopBar.anchoredPosition = Vector2.zero;

        MainWindow.sizeDelta = new Vector2(ScreenSize.x, ScreenSize.y - (TOP_BAR_HEIGHT + BOTTOM_BAR_HEIGHT));
        MainWindow.anchoredPosition = new Vector2(0, -TOP_BAR_HEIGHT);        

        BottomBar.sizeDelta = new Vector2(ScreenSize.x, BOTTOM_BAR_HEIGHT);
        BottomBar.anchoredPosition = Vector2.zero;
    }

    private void SetStandardSize()
    {
        //set video scale and get video size
        ClampVideoScale();
        float CurrentVideoScale = Mathf.Lerp(VideoPanel.localScale.x, TargetVideoScale, Time.unscaledDeltaTime * 5); 
        VideoPanel.localScale =   new Vector2(CurrentVideoScale, CurrentVideoScale);
        VideoPanel.anchoredPosition = Vector2.zero;
        Vector2 VideoPanelSize = new Vector2(VIDEO_WIDTH, VIDEO_HEIGHT) * CurrentVideoScale;

        TopBar.sizeDelta =  new Vector2(ScreenSize.x - VideoPanelSize.x , TOP_BAR_HEIGHT);
        TopBar.anchoredPosition = Vector2.zero;

        MainWindow.sizeDelta =  new Vector2(ScreenSize.x - VideoPanelSize.x , VideoPanelSize.y - TOP_BAR_HEIGHT);
        MainWindow.anchoredPosition = new Vector2(0, -TOP_BAR_HEIGHT);

        CueWindow.sizeDelta =  new Vector2(ScreenSize.x, ScreenSize.y - (VideoPanelSize.y + BOTTOM_BAR_HEIGHT));
        CueWindow.anchoredPosition = new Vector2( 0, BOTTOM_BAR_HEIGHT);

        BottomBar.sizeDelta = new Vector2(ScreenSize.x, BOTTOM_BAR_HEIGHT);
        BottomBar.anchoredPosition = Vector2.zero;
    }

    private void ClampVideoScale()
    {
        float maxScale = (ScreenSize.y-BOTTOM_BAR_HEIGHT) / VIDEO_HEIGHT; 
        TargetVideoScale = Mathf.Clamp(TargetVideoScale, .5f, maxScale);
    }

    private void OnRectTransformDimensionsChange()
    {
        SetPanelPositions();
    }
}
