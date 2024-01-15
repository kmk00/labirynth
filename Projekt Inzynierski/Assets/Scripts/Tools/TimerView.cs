using System;
using TMPro;
using UnityEngine;

public class TimerView : MonoBehaviour
{
    [SerializeField]
    public TextMeshProUGUI text;

    private void Update()
    {
        UpdateScore();
    }

    void UpdateScore()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(SharedData.Time);
        text.text = timeSpan.ToString(@"mm\:ss\:ff");
    }

}

