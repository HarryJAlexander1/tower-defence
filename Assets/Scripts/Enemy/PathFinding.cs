using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    public Graph.Vertex StartingVertex;
    public List<Graph.Vertex> Vertices;
    // Start is called before the first frame update
    void Awake()
    {
        Vertices = new List<Graph.Vertex>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
