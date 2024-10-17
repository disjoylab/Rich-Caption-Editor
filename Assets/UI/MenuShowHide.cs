using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuShowHide : MonoBehaviour
{
    public MenuStates myMenuState; 
    void Awake()
    {
        MenuManager.CurrentMenuStateChanged += OnMenuStateChanged;
    }
    void OnDestroy()
    {
        MenuManager.CurrentMenuStateChanged -= OnMenuStateChanged;
    }

    private void OnMenuStateChanged(MenuStates _currentMenuState)
    {
        gameObject.SetActive(myMenuState == _currentMenuState);
    }     
}
