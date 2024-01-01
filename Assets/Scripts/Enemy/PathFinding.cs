using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    GameObject GameManagerObject;
    GameManager GameManager;
    List<GameManager.Square> Squares;
    public GameManager.Square StartingSquare;
    // Start is called before the first frame update
    void Start()
    {
        GetSquareData();
    }

    private void GetSquareData() 
    {
        GameManager = GameManagerObject.GetComponent<GameManager>();
        Squares = GameManager.Squares; // get the squares data from Game Manager
    }

    // Update is called once per frame
    void Update()
    {

    }
}
