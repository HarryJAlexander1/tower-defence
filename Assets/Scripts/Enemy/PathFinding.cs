using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PathFinding : MonoBehaviour
{
    public Graph.Vertex StartingVertex;
    public Graph.Vertex EndingVertex;
    public List<Graph.Vertex> Vertices;

    private List<Graph.Vertex> Path;
    // Start is called before the first frame update
    void Awake()
    {
    }

    public void FindShortestPath(Graph.Vertex source, Graph.Vertex destination)
    {
        // Initialize the queue for BFS
        Queue<Graph.Vertex> queue = new Queue<Graph.Vertex>();
        // Dictionary to store the parent of each Graph.Vertex for reconstructing the path
        Dictionary<Graph.Vertex, Graph.Vertex> parent = new Dictionary<Graph.Vertex, Graph.Vertex>();

        // Enqueue the source Graph.Vertex
        queue.Enqueue(source);
        parent[source] = null;

        while (queue.Count > 0)
        {
            Graph.Vertex current = queue.Dequeue();

            // Check if the current Graph.Vertex is the destination
            if (current == destination)
            {
                // Reconstruct the path from source to destination
                Path = ReconstructPath(parent, source, destination);
            }

            // Enqueue neighbors if not already visited
            foreach (Graph.Vertex neighbor in current.Neighbours)
            {
                if (!parent.ContainsKey(neighbor))
                {
                    queue.Enqueue(neighbor);
                    parent[neighbor] = current;
                }
            }
        }

        // If no path is found
        Path = null;
    }

    List<Graph.Vertex> ReconstructPath(Dictionary<Graph.Vertex, Graph.Vertex> parent, Graph.Vertex source, Graph.Vertex destination)
    {
        List<Graph.Vertex> path = new List<Graph.Vertex>();
        Graph.Vertex current = destination;

        // Reconstruct the path from destination to source
        while (current != null)
        {
            path.Insert(0, current); // Insert at the beginning to reverse the order
            current = parent[current];
        }

        var count = 0;
        foreach (Graph.Vertex v in path) 
        {
            count++;
            Debug.Log($"Step {count} = " + v.Coordinates);
        }
        return path;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
