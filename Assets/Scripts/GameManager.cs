using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public GameObject LevelPrefab;
    private GameObject Level;
    private Transform Platform;
    private Transform Tower;
    public GameObject BlockPrefab;
    private const int LevelDimension = 60;
    private Vector3 LevelSpawnPosition;
    private List<Square> BoundrySquares;
    private static int AgentSpawnNumber;
    Graph Graph;
    public Graph.Vertex AgentStartingVertex;
    public GameObject AgentPrefab;
    public List<Square> Squares;
    public GameObject PlayerPrefab;
    public bool IsAttackSequence;
    private int LevelCount;
    public bool AgentExists;
    public int Fund;
    public int PlayerScore;
    public int PlayerHealth;
    // Start is called before the first frame update
    void Start()
    {
        PlayerScore = 0;
        Fund = 100000;
        AgentExists = false;
        IsAttackSequence = false;
        LevelCount = 1;
        LevelSpawnPosition = new(0, -0.5f, 0);
        BoundrySquares = new List<Square>();
        Squares = new List<Square>();
        CreateLevel();
        Graph = ScriptableObject.CreateInstance<Graph>();
        Graph.GenerateGraph(Squares);
        AgentSpawnNumber = 1;
        GenerateAgentSpawnPosition(Graph);
        SpawnEntity(new(5, 0, 0), PlayerPrefab); // spawn player
        PlayerHealth = 10;
    }

    private void Update()
    {
        if (!IsAttackSequence && Input.GetKeyDown(KeyCode.G) && !AgentExists) 
        {
            ExecuteAttackSequence();
        }
    }

    public void CheckAgentsExist() 
    {
        AgentExists = GameObject.FindGameObjectsWithTag("Agent").Length > 1;
    }

    private Graph.Vertex FindNearestVertex(Vector3 rayCastHitPosition, List<Graph.Vertex> VerticesList) 
    {
        float smallestDistance = Mathf.Infinity;
        Graph.Vertex nearestVertex = null;

        foreach (Graph.Vertex vertex in VerticesList)
        {
            float distance = Vector3.Distance(vertex.Coordinates, rayCastHitPosition);

            if (distance < smallestDistance)
            {
                smallestDistance = distance;
                nearestVertex = vertex;
            }
        }
        return nearestVertex;
    }
    public void PlaceBlockOnNearestEmptyVertex(Vector3 rayCastHitPosition)
    {
        Graph.Vertex nearestVertex = FindNearestVertex(rayCastHitPosition, Graph.Vertices);
        foreach (Graph.Vertex neighbour in nearestVertex.Neighbours)   // remove this vertex from its neighbours 'neighbours' list
        {
            neighbour.Neighbours.Remove(nearestVertex);
        }

        Graph.Vertices.Remove(nearestVertex);
        Graph.RemovedVertices.Add(nearestVertex);

        // hacky code, but lets GameManager compute if their is a valid path for enemies or not, before allowing player to place block.
        var agent = SpawnEntity(new(999, 999, 999), AgentPrefab);
        var agentBehaviour = agent.GetComponent<AgentBehaviour>();
        agentBehaviour.StartingVertex = AgentStartingVertex;
        agentBehaviour.EndingVertex = Graph.Center;
        agentBehaviour.Vertices = Graph.Vertices;
        agentBehaviour.FindShortestPath(agentBehaviour.StartingVertex, agentBehaviour.EndingVertex);

        if (agentBehaviour.Path != null)
        {
            Instantiate(BlockPrefab, nearestVertex.Coordinates - (Vector3.up * 0.5f), Quaternion.identity);
            Fund -= 10;
        }
        else 
        {
            Debug.Log("No valid path found to center.");
            // revert changes to graph
            foreach (Graph.Vertex neighbour in nearestVertex.Neighbours)   // remove this vertex from its neighbours 'neighbours' list
            {
                neighbour.Neighbours.Add(nearestVertex);
            }
            Graph.Vertices.Add(nearestVertex);
            Graph.RemovedVertices.Remove(nearestVertex);
        }

        Destroy(agent);
    }

    public void RemoveBlock(Vector3 rayCastHitPosition, GameObject block) 
    {
        Destroy(block); // remove block from level
        Graph.Vertex nearestVertex = FindNearestVertex(rayCastHitPosition, Graph.RemovedVertices); // find closest vertex in removed vertices list
        Graph.Vertices.Add(nearestVertex);
        Graph.RemovedVertices.Remove(nearestVertex);
        foreach (Graph.Vertex neighbour in nearestVertex.Neighbours)   // remove this vertex from its neighbours 'neighbours' list
        {
            neighbour.Neighbours.Add(nearestVertex);
        }
    }

    private void ExecuteAttackSequence() 
    {
        IsAttackSequence = true;
        AgentSpawnNumber = LevelCount * 10;
        StartExecution();
    }
    private void GenerateAgentSpawnPosition(Graph graph) 
    {
        int randomNumber = Random.Range(0, BoundrySquares.Count);
        var square = BoundrySquares[randomNumber];

        for (int i = 0; i < AgentSpawnNumber; i++)
        {
            foreach (Graph.Vertex v in graph.Vertices)
            {
                if (v.Coordinates == square.CenterPoint)
                {
                    AgentStartingVertex = v;
                    return;
                }
            }
        }
    }

    public void StartExecution()
    {
        StartCoroutine(SpawnEnemies());
    }
    private IEnumerator SpawnEnemies()
    {
        for (int agentNumber = 0; agentNumber < AgentSpawnNumber; agentNumber++)
        {
            var agent = SpawnEntity(AgentStartingVertex.Coordinates, AgentPrefab);
            var agentBehaviour = agent.GetComponent<AgentBehaviour>();
            agentBehaviour.StartingVertex = AgentStartingVertex;
            agentBehaviour.EndingVertex = Graph.Center;
            agentBehaviour.Vertices = Graph.Vertices;
            agentBehaviour.FindShortestPath(agentBehaviour.StartingVertex, agentBehaviour.EndingVertex);
            AgentExists = true;
            yield return new WaitForSeconds(3f);
        }
        IsAttackSequence = false;
        LevelCount++;
    }
    private void CreateLevel()
    {
        // instantiate level gameobject
        Level = Instantiate(LevelPrefab, LevelSpawnPosition, Quaternion.identity);

        // set dimensions of platform
        Platform = Level.transform.Find("Platform");
        Platform.localScale = new Vector3(LevelDimension, transform.localScale.y, LevelDimension);

        // set dimensions of tower
        Tower = Level.transform.Find("Tower");
        Tower.localScale = new Vector3(3, 5, 3);
        Tower.transform.position += Vector3.up * ((Tower.transform.localScale.y * 0.5f) - 0.5f);

        // generate grid of squares using platform transform
        GenerateGrid(Platform);
    }

    private GameObject SpawnEntity(Vector3 location, GameObject prefab) 
    {
        var entity = Instantiate(prefab, location, Quaternion.identity);
        entity.transform.position = new(entity.transform.position.x, entity.transform.position.y + 0.5f, entity.transform.position.z); // adjust position of agent to account for its height.
        return entity;
    }

    private void GenerateGrid(Transform platform)
    {
        Vector3 position = new(-(platform.localScale.x * 0.5f), 1.0f, -(platform.localScale.z * 0.5f));
        float minimum = -(Platform.localScale.x * 0.5f);
        float maximum = (Platform.localScale.x * 0.5f) - 1;

        for (int i = 0; i < LevelDimension; i++)
        {
            for (int j = 0; j < LevelDimension; j++)
            {
                Square square = GenerateSquareFromPosition(position);
                Squares.Add(square);

                if (position.x == minimum || position.x == maximum
                    || position.z == minimum || position.z == maximum) // get all positions on the outside of the level 
                {
                    BoundrySquares.Add(square);
                }

                position.x++;
            }
            position.z++;
            position.x = -position.x; // reset position on x axis
        }
    }
    private Square GenerateSquareFromPosition(Vector3 position)
    {
        Vector3 VertexB = new(position.x + 1, position.y, position.z);
        Vector3 VertexC = new(position.x, position.y, position.z + 1);
        Vector3 VertexD = new(position.x + 1, position.y, position.z + 1);
        return new Square(Squares.Count, position, VertexB, VertexC, VertexD);
    }

    public class Square // utility class representing a walkable square in the level
    {
        public int Id { get; set; }
        public Vector3 VertexA { get; set; }
        public Vector3 VertexB { get; set; }
        public Vector3 VertexC { get; set; }
        public Vector3 VertexD { get; set; }
        public Vector3 CenterPoint { get; private set; }
        public bool IsWalkable { get; set; }

        public Square(int id, Vector3 vertexA, Vector3 vertexB, Vector3 vertexC, Vector3 vertexD)
        {
            Id = id;
            VertexA = vertexA;
            VertexB = vertexB;
            VertexC = vertexC;
            VertexD = vertexD;
            CenterPoint = ComputeCenterPoint();
            IsWalkable = true;
        }
        private Vector3 ComputeCenterPoint()
        {
            float centerX = (VertexA.x + VertexB.x + VertexC.x + VertexD.x) * 0.25f;
            float centerY = (VertexA.y + VertexB.y + VertexC.y + VertexD.y) * 0.25f; // y axis for each vertex should always be 1
            float centerZ = (VertexA.z + VertexB.z + VertexC.z + VertexD.z) * 0.25f;

            return new Vector3(centerX, centerY, centerZ);
        }
    }
}
