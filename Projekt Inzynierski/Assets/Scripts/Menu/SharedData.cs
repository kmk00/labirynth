using UnityEngine;

public class SharedData : MonoBehaviour
{
    SharedData() { }

    public static SharedData _instance;

    public static int X
    {
        get => _x; 
        set { _x = value; }
    }
    static int _x = 3;
    public static int Y
    {
        get => _y;
        set { _y = value; }
    }
    static int _y = 3;

    public static bool IsMinimumOn
    {
        get => _isMinimumOn;
        set { _isMinimumOn = value; }
    }
    static bool _isMinimumOn;

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
}
