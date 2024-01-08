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
        if (int.TryParse(newXValue, out int x) && x >= 0)
            SharedData.X = x;
        else
        {
            SharedData.X = 3;
            xInputField.text = "3";
        }
    }

    private void UpdateYValue(string newYValue)
    {
        if (int.TryParse(newYValue, out int y) && y >= 0)
            SharedData.Y = y;
        else
        {
            SharedData.Y = 3;
            yInputField.text = "3";
        }
    }
}
