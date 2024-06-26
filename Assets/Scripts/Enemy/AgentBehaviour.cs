using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class AgentBehaviour : MonoBehaviour
{
    public Graph.Vertex StartingVertex;
    public Graph.Vertex EndingVertex;
    public List<Graph.Vertex> Vertices;
    private float Speed;
    private int CurrentStep;
    public List<Graph.Vertex> Path;
    private bool BobbingUp;
    private GameObject GameManagerObject;
    private GameManager GameManagerScript;
    public int Hitpoints;
    public GameObject Player;
    private float RaycastDistance;
    private AgentGunEffects GunEffects;

    public float ShootCooldown = 0.5f; // Cooldown duration between shots
    private float NextShootTime = 0f; // Time when the enemy can shoot again
    private int ShootingRangeOffset;
    private void Awake()
    {
        Hitpoints = 10;
        BobbingUp = true;
        Path = new List<Graph.Vertex>();
        CurrentStep = 0;
        Speed = 2.0f;
        GameManagerObject = GameObject.FindGameObjectWithTag("GameManager");
        Player = GameObject.FindGameObjectWithTag("Player");
        GameManagerScript = GameManagerObject.GetComponent<GameManager>();
        RaycastDistance = 100f;
        GunEffects = GetComponent<AgentGunEffects>();  
        ShootingRangeOffset = Random.Range(0, 5);
    }
    void Update()
    { 
        if (IsWithinShootingDistance(Player.transform, 10f, ShootingRangeOffset))
        {
            // face player
            transform.LookAt(Player.transform);
            if (Time.time >= NextShootTime)
                ShootPlayer();
        }
        else 
        { 
            TraverseGraph(); 
        }
        
        CheckHitpoints();
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
                Die();
                GameManagerScript.TowerHealth--;
            }
           
           // BobUpAndDown(2f);
        }
    }

    private void Die() 
    {
        GameManagerScript.CheckAgentsExist();
        GameManagerScript.PlayerScore += 10;
        Destroy(gameObject);
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
            //Debug.Log($"Step {count} = " + v.Coordinates);
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
        transform.LookAt(node.Coordinates);
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

    private void ShootPlayer() 
    {
        // fire raycast
        FireRaycast();
        NextShootTime = Time.time + ShootCooldown;
    }

    private void FireRaycast()
    {
        Vector3 raycastOrigin = transform.position;
        Vector3 raycastDirection = transform.forward;

        // Create a Ray from the origin and direction
        Ray ray = new Ray(raycastOrigin, raycastDirection);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, RaycastDistance))
        {
            GunEffects.PlayGunEffects();
            GameObject hitGameObject = hitInfo.transform.gameObject;
            if (hitGameObject.CompareTag("Player")) 
            {
                GameManagerScript.PlayerHealth--;
            }
        }
    }

    private bool IsWithinShootingDistance(Transform comparatorTransform, float shootingDistance, int offset) 
    {
        if (Vector3.Distance(transform.position, comparatorTransform.position) < shootingDistance + offset)
            return true;
        else
            return false;
    }

    private void CheckHitpoints() 
    {
        if (Hitpoints == 0) 
        {
            GameManagerScript.Fund += 20;
            Die();
        }
    }
}
