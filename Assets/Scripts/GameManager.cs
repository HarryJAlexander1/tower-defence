using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public GameObject LevelPrefab;
    private GameObject Level;
    private Transform Platform;
    private Transform Tower;
    private const int LevelDimension = 60;
    private Vector3 LevelSpawnPosition = new(0, 0, 0);

    private List<Square> BoundrySquares = new List<Square>();
    private static int AgentSpawnNumber = 1;
    public List<Graph.Vertex> AgentStartingVertices = new List<Graph.Vertex>();
    public GameObject AgentPrefab;
    public List<Square> Squares = new List<Square>();
 
    public GameObject PlayerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        CreateLevel();
        Graph graph = ScriptableObject.CreateInstance<Graph>();
        graph.GenerateGraph(Squares);

        for (int i = 0; i < AgentSpawnNumber; i++) 
        {
            int randomNumber = Random.Range(0, BoundrySquares.Count);
            Debug.Log($"Random number: {randomNumber} i: {i}");
            var square = BoundrySquares[randomNumber];
            foreach (Graph.Vertex v in graph.Vertices) 
            {
                if (v.Coordinates == square.CenterPoint) 
                {
                    AgentStartingVertices.Add(v);
                }
            }
        }

        // spawn agents
        Debug.Log("AgentStartingVertices count= " + AgentStartingVertices.Count);
        foreach (Graph.Vertex v in AgentStartingVertices)
        {
            var agent = SpawnEntity(v.Coordinates, AgentPrefab); // spawn enemy agent
            var agentPathFinding = agent.GetComponent<PathFinding>();
            agentPathFinding.StartingVertex = v;
            agentPathFinding.EndingVertex = graph.Center;
            Debug.Log("Starting vertex coordinates = " + agentPathFinding.StartingVertex.Coordinates);
            for (int i = 0; i < agentPathFinding.StartingVertex.Neighbours.Count; i++) 
            {
                Debug.Log("Neighbour = " + agentPathFinding.StartingVertex.Neighbours[i].Coordinates);
            }
            agentPathFinding.Vertices = graph.Vertices;
            agentPathFinding.FindShortestPath(agentPathFinding.StartingVertex, agentPathFinding.EndingVertex);
        }

        SpawnEntity(new(5, 0, 0), PlayerPrefab); // spawn player
    }

    // Update is called once per frame
    void Update()
    {

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
        Vector3 position = new(-(platform.localScale.x * 0.5f), 1, -(platform.localScale.z * 0.5f));
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
