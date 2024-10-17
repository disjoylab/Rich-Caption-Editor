using System.Collections.Generic;
using UnityEngine;

public class ButtonContainer : MonoBehaviour
{
    public List<GameObject> buttons = new List<GameObject>();
    public RectTransform contentView;
    public RectTransform RightSideScrollbar;

    private void Awake()
    {
        MenuManager.MenuLayoutUpdated += OnMenuLayoutUpdated;
    }

    private void OnDestroy()
    {
        MenuManager.MenuLayoutUpdated -= OnMenuLayoutUpdated;
    }

    private void OnMenuLayoutUpdated()
    {
        UpdateLayout();
    }

    public void Configure(GameObject buttonPrefab, int count)
    {

        // Create new buttons if needed
        for (int i = buttons.Count; i < count; i++)
        {
            GameObject newButton = Instantiate(buttonPrefab, contentView);
            buttons.Add(newButton);
        }

        // Destroy extra buttons
        for (int i = buttons.Count - 1; i >= count; i--)
        {
            Destroy(buttons[i]);
            buttons.RemoveAt(i);
        }
        UpdateLayout();
    }

    public void UpdateLayout()
    {
        //ADJUSTS TO DIFFERENT SIZED BUTTONS (currently no buttons are different sizes though)
        //This would need to be called if the buttons change sizes like settings buttons that would resize based on the content.
        if (buttons.Count == 0)
        {
            return;
        }

        RectTransform rt = GetComponent<RectTransform>();
        float totalHeight = 0f;
        List<float> buttonHeights = new List<float>();

        foreach (var button in buttons)
        {
            float buttonHeight = button.GetComponent<RectTransform>().sizeDelta.y;
            buttonHeights.Add(buttonHeight);
            totalHeight += buttonHeight;
        }

        bool heightFits = rt.rect.height >= totalHeight;
        float width = rt.rect.width - (heightFits ? 0 : RightSideScrollbar.rect.width);

        contentView.sizeDelta = new Vector2(width, totalHeight);

        float yOffset = 0f;

        for (int i = 0; i < buttons.Count; i++)
        {
            RectTransform buttonRt = buttons[i].GetComponent<RectTransform>();
            float buttonHeight = buttonHeights[i];
            buttonRt.sizeDelta = new Vector2(width, buttonHeight);
            buttonRt.anchoredPosition = new Vector2(0, -yOffset);
            yOffset += buttonHeight;
        }
    }
}

