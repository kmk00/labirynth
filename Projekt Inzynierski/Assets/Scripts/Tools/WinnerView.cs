using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinnerView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _winner;

    private void Update()
    {
        _winner.text = SharedData.Winner.ToString();
    }
}
