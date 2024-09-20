using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
    public enum MenuStates { File, CueGroup, Elements, Features, Styles, COUNT }
[ExecuteInEditMode]
public class MenuManager : MonoBehaviour
{
    public static MenuStates CurrentMenuState;
    public static Action<MenuStates> CurrentMenuStateChanged;
    public float CurrentVideoScale;
    public static float TargetVideoScale = 1; 
    public RectTransform VideoPanel; //
    public RectTransform TopBar;
    public RectTransform MainWindow;
    public RectTransform CueWindow;
    Vector2 ScreenSize = Vector2.zero;
    public bool MainMenuFullscreen;

    // Start is called before the first frame update
    void Start()
    {
        TargetVideoScale = 1;
        CurrentMenuStateChanged?.Invoke(CurrentMenuState);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 currentScreenSize = new Vector2(Screen.width, Screen.height);
        if (currentScreenSize != ScreenSize)
        {
            ScreenSize = currentScreenSize;
            SetPanelPositions();
        }
        if (CurrentVideoScale!= TargetVideoScale)
        {
            CurrentVideoScale = Mathf.Lerp(CurrentVideoScale, TargetVideoScale, Time.unscaledDeltaTime * 5);
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
    public void ToggleFullscreen() //****************** TODO SAVE PLAY/PAUSE STATE > PAUSE ON MAX, RESTORE STATE ON MIN
    {
        MainMenuFullscreen = !MainMenuFullscreen;
        SetPanelPositions();
    }
    private void SetPanelPositions()
    {
        ClampVideoScale();
        VideoPanel.localScale = MainMenuFullscreen ? Vector2.zero: new Vector2(CurrentVideoScale, CurrentVideoScale);
        Vector2 VideoPanelSize = new Vector2(660, 560) * CurrentVideoScale;

        TopBar.sizeDelta = MainMenuFullscreen ? new Vector2(ScreenSize.x-5, 55) : new Vector2(ScreenSize.x- (VideoPanelSize.x+15), 55);
        MainWindow.sizeDelta = MainMenuFullscreen?new Vector2(ScreenSize.x-5,ScreenSize.y-65): new Vector2(ScreenSize.x - (VideoPanelSize.x + 15), VideoPanelSize.y - 60);
        CueWindow.sizeDelta = MainMenuFullscreen ? Vector2.zero:new Vector2(ScreenSize.x - 10, ScreenSize.y - (VideoPanelSize.y +15));
    }

    private void ClampVideoScale()
    {
        float maxScale = ScreenSize.y / 560;
        CurrentVideoScale = Mathf.Clamp(CurrentVideoScale, .5f, maxScale);
        TargetVideoScale = Mathf.Clamp(TargetVideoScale, .5f, maxScale);
    }

    private void OnRectTransformDimensionsChange()
    {
        SetPanelPositions();
    }
}
