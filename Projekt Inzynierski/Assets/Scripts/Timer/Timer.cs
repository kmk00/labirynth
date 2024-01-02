using UnityEngine;
using TMPro;
using System.Globalization;

public class Timer : MonoBehaviour
{
    private GameObject timer;
    private TextMeshProUGUI timerText;

    private bool isTiming = false;
    private float elapsedTime = 0f;

    private void Start()
    {
        timer = GameObject.Find("Timer");
        //timerText = timer.GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        if (isTiming)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimeText();
        }
    }

    void UpdateTimeText()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        string timeString = string.Format("{0:00}:{1:00}", minutes, seconds);
        timerText.text = timeString;
    }

    public void StartStopwatch()
    {
        isTiming = true;
    }

    public void StopStopwatch()
    {
        isTiming = false;
    }


    /*
    [Header("Timer Settings")]
    public static float currentTime;
    public bool countDown;

    

    // Start is called before the first frame update
    void Start()
    {
        timerText.text = "Time: 0.0";

    }

    // Update is called once per frame
    void Update()
    {
        currentTime = countDown ? currentTime -= Time.deltaTime : currentTime += Time.deltaTime;
        CultureInfo culture = new CultureInfo("en-US");
        timerText.text = "Time: " + currentTime.ToString("0.0", culture);

    }
     */







}
