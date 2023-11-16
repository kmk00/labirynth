using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{

    [SerializeField]
    private MazeCell _cellPref;
    [SerializeField]
    private int _width;
    [SerializeField]
    private int _height;

    private MazeCell[,] _mazeGrid;

    // Start is called before the first frame update
    void Start()
    {
        _mazeGrid = new MazeCell[_width, _height];

        for(int i = 0;  i < _width; i++)
        {
            for(int j = 0; j< _height; j++)
            {
                _mazeGrid[i,j] = Instantiate(_cellPref, new Vector3(i,0,j), Quaternion.identity);
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
