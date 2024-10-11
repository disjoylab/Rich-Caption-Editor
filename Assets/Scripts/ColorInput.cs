using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColorInput : MonoBehaviour
{
    //Utilities related to all things color related

    public TMP_InputField inputField_R;
    public TMP_InputField inputField_G;
    public TMP_InputField inputField_B;
    public Image colorImage;

    public void SetColor(Color C)
    {  
        colorImage.color = C;
        inputField_R.SetTextWithoutNotify( ((int)(C.r * 255)).ToString());
        inputField_G.SetTextWithoutNotify(((int)(C.g * 255)).ToString());
        inputField_B.SetTextWithoutNotify( ((int)(C.b * 255)).ToString());

    }

    public string GetColorHex()
    {
        if (int.TryParse(inputField_R.text, out int r) &&
            int.TryParse(inputField_G.text, out int g) &&
            int.TryParse(inputField_B.text, out int b))
        {
            r = Mathf.Clamp(r, 0, 255);
            g = Mathf.Clamp(g, 0, 255);
            b = Mathf.Clamp(b, 0, 255);

            return $"#{r:X2}{g:X2}{b:X2}";             
        }

        return "000000";
    }
}
