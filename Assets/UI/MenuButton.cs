
    using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    public MenuStates myMenuState; // Assume MenuStates enum is defined elsewhere

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => MenuManager.SetCurrentMenuState(myMenuState));
    }
}

