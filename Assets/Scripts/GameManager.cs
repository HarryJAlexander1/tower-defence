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
    private const int LevelDimension = 75;
    private Vector3 LevelSpawnPosition = new(0, 0, 0);

    private List<Vector3> BoundrySquareCenterPoints = new List<Vector3>();
    private static int AgentSpawnNumber = 1;
    public List<Vector3> AgentSpawnPositions = new List<Vector3>();
    public GameObject AgentPrefab;
    public List<Square> Squares = new List<Square>();
    private List<Vector3> PositionsList = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        CreateLevel();
        // spawn agents in positions
        foreach (Vector3 position in AgentSpawnPositions)
        {
            SpawnAgent(position);
        }
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
        Tower.localScale = new Vector3(1, 5, 1);
        Tower.transform.position += Vector3.up * ((Tower.transform.localScale.y * 0.5f) - 0.5f);

        // generate grid of squares using platform transform
        GenerateGrid(Platform);

        // select agent spawn positions
        for (int i = 0; i < AgentSpawnNumber; i++)
        {
            int randomNumber = Random.Range(0, BoundrySquareCenterPoints.Count);
            Debug.Log($"Random number: {randomNumber} i: {i}");
            AgentSpawnPositions.Add(BoundrySquareCenterPoints[randomNumber]);
        }
    }

    private void SpawnAgent(Vector3 location) 
    {
        var agent = Instantiate(AgentPrefab, location, Quaternion.identity);
        agent.transform.position = new(agent.transform.position.x, agent.transform.position.y + 0.5f, agent.transform.position.z); // adjust position of agent to account for its height.
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
                PositionsList.Add(position);
                Square square = GenerateSquareFromPosition(position);
                Squares.Add(square);

                if (position.x == minimum || position.x == maximum
                    || position.z == minimum || position.z == maximum) // get all positions on the outside of the level 
                {
                    BoundrySquareCenterPoints.Add(square.CenterPoint);
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
