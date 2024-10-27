using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharToolTip : MonoBehaviour
{
    private TMP_InputField CueEditBox;

    private void Awake()
    {
        CueEditBox = gameObject.GetComponent<TMP_InputField>();
    }

    internal string GetTip()
    {
        TMP_Text textComponent = CueEditBox.textComponent;

        Canvas canvas = textComponent.canvas;
        Camera uiCamera = null;
        if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            uiCamera = canvas.worldCamera;
        }         
        Vector3 mousePosition = Input.mousePosition;         
        bool isOver = RectTransformUtility.RectangleContainsScreenPoint(textComponent.rectTransform, mousePosition, uiCamera);
        if (!isOver)
        {
            return null; 
        }

        // Find the index of the character under the mouse cursor
        int charIndex = TMP_TextUtilities.FindIntersectingCharacter(textComponent, mousePosition, uiCamera, true);

        if (charIndex != -1 && charIndex < textComponent.textInfo.characterCount)
        { 
            TMP_CharacterInfo charInfo = textComponent.textInfo.characterInfo[charIndex];
            char character = charInfo.character;
            CueChar cueChar = CueUI.GetCueChar(charIndex);
            string CharacterDetails = $"{cueChar.c}: ";
            if (cueChar.elements!=null)
            {
                foreach (var element in cueChar.elements)
                {
                    CharacterDetails += $" {element.ToButtonText()} |";
                }
            }
            CharacterDetails = CharacterDetails.Trim('|');
            return CharacterDetails ;
            
        }
        else
        {
            return null; // No character under the mouse
        }
    }
}
