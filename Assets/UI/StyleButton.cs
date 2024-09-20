using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StyleButton : MonoBehaviour
{ 
    public Style myStyle;
    public TextMeshProUGUI buttonText;
    
    internal void Configure(Style _style)
    {
        myStyle = _style;
        if (myStyle == null)
        {
            buttonText.text = "Add Style";
        }
        else
        {
            buttonText.text = myStyle.ToButtonText();
        }
    }
    public void SetCurrentStyle()
    {
        
            if (myStyle ==null)
            {
                StylesMenu.AddStyle();
                StylesMenu.StylesHaveChanges = true;
            }
            else
            {
                StylesMenu.SetCurrentStyle(myStyle);
            } 
    }

}
