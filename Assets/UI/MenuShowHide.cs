using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuShowHide : MonoBehaviour
{
    public MenuStates myMenuState;
    // Start is called before the first frame update
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
