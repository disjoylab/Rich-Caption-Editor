 
using System.Collections.Generic;
using UnityEngine; 

public class ButtonContainer : MonoBehaviour
{
    public List<GameObject> buttons = new List<GameObject>();
    public RectTransform contentView;
    public RectTransform RightSideScrollbar;

    public void Configure(GameObject _buttonPrefab, int _count)
    {        
        float height = _buttonPrefab.GetComponent<RectTransform>().sizeDelta.y;
        RectTransform rt = gameObject.GetComponent<RectTransform>();
        bool HeightFits = rt.rect.height >= height * _count;
        float width = rt.rect.width - (HeightFits ? 0 : RightSideScrollbar.rect.width); 
        Vector2 contentSize = new Vector2(width, height * _count);

        for (int i = 0; i < _count; i++)
        {
            if (buttons.Count <= i)
            {
                GameObject newButton = Instantiate(_buttonPrefab, contentView);
                buttons.Add(newButton);
            }
            RectTransform buttonRt = buttons[i].GetComponent<RectTransform>();
            buttonRt.sizeDelta = new Vector2(width, height);
            buttonRt.anchoredPosition = new Vector2(0, i * -height);
        }

        for (int i = _count; i < buttons.Count; i++)
        {
            Destroy(buttons[i]);
        }
        buttons.RemoveRange(_count, buttons.Count - _count);
        contentView.sizeDelta = contentSize;
    }
}

