using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escape : MonoBehaviour
{
    public GameObject Canvas;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ChangeStateOfMenu();
    }
    public void ChangeStateOfMenu()
    {
        if (Canvas.activeSelf)
            Canvas.SetActive(false);
        else
            Canvas.SetActive(true);
    }
}
