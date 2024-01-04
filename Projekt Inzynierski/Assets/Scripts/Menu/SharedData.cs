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

    public static SharedData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("SharedData").AddComponent<SharedData>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
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
