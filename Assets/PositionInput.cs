using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PositionInput : MonoBehaviour
{
    public TMP_InputField inputField_X;
    public TMP_InputField inputField_Y; 

    public void SetPosition(Position P)
    { 
        inputField_X.SetTextWithoutNotify(((int)(P.X)).ToString());
        inputField_Y.SetTextWithoutNotify(((int)(P.Y)).ToString()); 

    }

    public string GetPositionString()
    {
        return $"{inputField_X.text},{ inputField_Y.text}";
    }
        
}
