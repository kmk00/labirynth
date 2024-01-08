using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    public TMP_InputField xInputField;
    public TMP_InputField yInputField;
    public Toggle MinimumToggle;

    private void Awake()
    {
        xInputField.text = SharedData.X.ToString();
        yInputField.text = SharedData.Y.ToString();
        if(SharedData.IsMinimumOn.Equals(true))
            MinimumToggle.isOn = true;
        else if(SharedData.IsMinimumOn.Equals(false))
            MinimumToggle.isOn = false;
    }

    private void Start()
    {
        xInputField.onEndEdit.AddListener(UpdateXValue);
        yInputField.onEndEdit.AddListener(UpdateYValue);
    }
    public void Update()
    {
        SharedData.IsMinimumOn = MinimumToggle.isOn;
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
