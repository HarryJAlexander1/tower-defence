using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : ScriptableObject
{
    public List<Vertex> Vertices { get; set; }

    public void GenerateGraph(List<GameManager.Square> squares) 
    {
        Vertices = new List<Vertex>();  
        // construct vertices from squares
        foreach (GameManager.Square square in squares) 
        {
            var vertex = ConstructVertex(square);
            Vertices.Add(vertex);
        }

        // populate neighbours for each vertex in vertices
        for (int i = 0; i < Vertices.Count; i++) 
        {
            for (int j = 0; j < Vertices.Count; j++) 
            {
                if (i == j) { continue; }

                if (Vertices[i].IsNeighbour(Vertices[j])) // if Vertices[j] is a neighbour to vertices[i], add them as neighbours to eachother
                {
                    Vertices[i].AddEdge(Vertices[j]);
                }
            }
        }
    }

    private Vertex ConstructVertex(GameManager.Square square) 
    {
        return new Vertex(square.CenterPoint);
    }

    public class Vertex 
    {
        public Vector3 Coordinates { get; private set; }
        public List<Vertex> Neighbours { get; private set; }
        public Vertex(Vector3 coordinates) 
        {
            Neighbours = new List<Vertex>();
            Coordinates = coordinates;
        }
        public void AddEdge(Vertex vertexB) 
        {
            Neighbours.Add(vertexB);
            vertexB.Neighbours.Add(this);
        }

        public bool IsNeighbour(Vertex vertexB) 
        {
            if (vertexB.Coordinates.x == Coordinates.x + 1 || vertexB.Coordinates.x == Coordinates.x - 1 
                || vertexB.Coordinates.z == Coordinates.z + 1 || vertexB.Coordinates.z == Coordinates.z - 1) 
            {
                return true;
            }
            return false;
        }
    }
}
