using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvasController : MonoBehaviour
{
    public TMP_InputField xInputField;
    public TMP_InputField yInputField;

    private void Start()
    {
        xInputField.onEndEdit.AddListener(UpdateXValue);
        yInputField.onEndEdit.AddListener(UpdateYValue);
    }

    private void UpdateXValue(string newXValue)
    {
        if (int.TryParse(newXValue, out int x))
        {
            SharedData.SetX(x);
        }
        else
        {
            SharedData.SetX(3);
            xInputField.text = "3";
        }
    }

    private void UpdateYValue(string newYValue)
    {
        if (int.TryParse(newYValue, out int y))
        {
            SharedData.SetY(y);
        }
        else
        {
            SharedData.SetY(3);
            yInputField.text = "3";
        }
    }
}
