using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public class ToolTipManager : MonoBehaviour
{
    public TextMeshProUGUI tooltipTextBox;          
    public float MouseMoveTime = 1;     
    Vector3 lastMousePosition;
    float timer = 0f;

    void Start()
    {
        lastMousePosition = Input.mousePosition;
        tooltipTextBox.text = ""; 
    }

    void Update()
    {
        if (Input.mousePosition == lastMousePosition)
        {
            timer += Time.deltaTime;
            if (timer >= MouseMoveTime)
            {
                ShowToolTip();
                timer = 0;
            }
        }
        else
        {
            timer = 0f;
            tooltipTextBox.text = ""; 
        }
        lastMousePosition = Input.mousePosition;
    }

    void ShowToolTip()
    { 
        if (EventSystem.current.IsPointerOverGameObject())
        { 
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
            foreach (var result in results)
            {  
                GameObject uiElement = result.gameObject;
                 
                ToolTip toolTip = uiElement.GetComponent<ToolTip>();
                if (toolTip != null)
                {
                    tooltipTextBox.text = toolTip.Tip;
                    return;
                }
                CharToolTip charToolTip = uiElement.GetComponent<CharToolTip>();
                if (charToolTip != null)
                {
                    tooltipTextBox.text = charToolTip.GetTip();
                    return;
                }

            }
        }
        tooltipTextBox.text = "";         
    }
}
