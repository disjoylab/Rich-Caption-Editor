using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ElementButton : MonoBehaviour
{
    public string myElement;
    public Image buttonImage;
    public TextMeshProUGUI buttonText;
    public Toggle ElementToggle;

    internal void Configure(string _name)
    {
        myElement = _name;
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(SetCurrentElement);
        }
        buttonText.text = myElement;

        ElementGroup elementGroup = ElementManager.GetElementGroup(myElement);
        if (elementGroup == null)
        {
            ElementToggle.isOn = false;
        }
        else
        {
            ElementToggle.isOn = elementGroup.Active;
        }

        buttonImage.color = ElementToggle.isOn ? Color.white : new Color(.8f, .8f, .8f);

    }

    public void SetCurrentElement()
    {
        if (myElement == "Add Element")
        {
            ElementGroup newElementGroup = new ElementGroup(ElementManager.GetUniqueName( "E"),true, true); 
            ElementManager.AddElementGroup(newElementGroup);
            myElement = newElementGroup.Name;
        }
        ElementMenu.SetCurrentElement(myElement);
    }
    public void SetFeatureGroupActive()
    {
        ElementGroup elementGroup = ElementManager.GetElementGroup(myElement);
        if (elementGroup != null)
        {
            elementGroup.Active = ElementToggle.isOn;
            ElementManager.SaveElementGroupToJSON(elementGroup);
            ElementMenu.SetCurrentElement(myElement);
        }
    }
}
