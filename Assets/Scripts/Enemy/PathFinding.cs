using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class PathFinding : MonoBehaviour
{
    public Graph.Vertex StartingVertex;
    public Graph.Vertex EndingVertex;
    public List<Graph.Vertex> Vertices;
    private float Speed;
    private int CurrentStep;
    private List<Graph.Vertex> Path;
    private bool BobbingUp;
    private GameObject GameManagerObject;
    private GameManager GameManagerScript;

    private void Awake()
    {
        BobbingUp = true;
        Path = new List<Graph.Vertex>();
        CurrentStep = 0;
        Speed = 2.0f;
        GameManagerObject = GameObject.FindGameObjectWithTag("GameManager");
        GameManagerScript = GameManagerObject.GetComponent<GameManager>();
    }
    void Update()
    {
        TraverseGraph();
    }

    private void TraverseGraph() 
    {
        if (Path.Count > 0)
        {
            if (IsApproximatelyAtNode(Path[CurrentStep], 0.1f) && CurrentStep < Path.Count - 1)
            {
                CurrentStep++;
            }
            MoveToNode(Path[CurrentStep]);

            if (CurrentStep == Path.Count - 1) 
            {
                GameManagerScript.CheckAgentsExist();
                Destroy(gameObject);
            }
           
            BobUpAndDown(2f);
        }
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
                return;
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
    private bool IsApproximatelyAtNode(Graph.Vertex node, float threshold)
    {
        if (Vector3.Distance(new(node.Coordinates.x, 0, node.Coordinates.z), new(transform.position.x, 0, transform.position.z)) < threshold)
        {
            return true;
        }
        return false;
    }

    private void MoveToNode(Graph.Vertex node)
    {
        Vector3 direction = node.Coordinates - transform.position;

        // Normalize the direction to get a unit vector
        direction.Normalize();

        // Move towards the target
        transform.position += direction * Speed * Time.deltaTime;
    }

    private void BobUpAndDown(float speed)
    {
        float maxY = 1.5f;
        float minY = 1f;

        float moveSpeed = speed;

        // Check if the object is moving up
        if (BobbingUp)
        {
            // Move the object up
            transform.Translate(moveSpeed * Time.deltaTime * Vector3.up);

            // Check if the object has reached or exceeded the maximum y position
            if (transform.localPosition.y >= maxY)
            {
                BobbingUp = false; // Switch to moving down
            }
        }
        else
        {
            // Move the object down
            transform.Translate(moveSpeed * Time.deltaTime * Vector3.down);

            // Check if the object has reached or gone below the minimum y position
            if (transform.localPosition.y <= minY)
            {
                BobbingUp = true; // Switch to moving up
            }
        }
    }
}
