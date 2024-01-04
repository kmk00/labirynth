using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SharedData : MonoBehaviour
{
    SharedData() { }

    public static SharedData _instance;

    static int _x = 3;
    static int _y = 3;

    public static SharedData GetInstance()
    {
        if (_instance == null)
        {
            _instance = new SharedData();
        }
        return _instance;
    }

    public static void SetX(int x)
    {
        _x = x;
        Debug.Log(x);
    }

    public static void SetY(int y)
    {
        _y = y;
        Debug.Log(y);
    }
}
