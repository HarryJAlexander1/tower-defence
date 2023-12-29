using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject LevelPrefab;
    private GameObject Level;
    private Transform Platform;
    private const int LevelDimension = 16;
    private Vector3 LevelSpawnPosition = new(0, 0, 0);
    
    public List<Square> Squares = new List<Square>();

    // Start is called before the first frame update
    void Start()
    {
        CreateLevel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CreateLevel() 
    {
        // instantiate level gameobject
        Level = Instantiate(LevelPrefab, LevelSpawnPosition, Quaternion.identity);

        // set dimensions of platform child game object
        Platform = Level.transform.Find("Platform");
        Platform.localScale = new Vector3(LevelDimension, transform.localScale.y, LevelDimension);

        // generate grid of squares using platform transform
        GenerateGrid(Platform);   
    }
    private void GenerateGrid(Transform platform) 
    {
        List<Vector3> positionsList = new List<Vector3>();
        Vector3 position = new(-(platform.localScale.x * 0.5f), 1, -(platform.localScale.z * 0.5f));

        for (int i = 0; i < LevelDimension; i++) 
        {
            for (int j = 0; j < LevelDimension; j++) 
            {
                positionsList.Add(position);
                GenerateSquareFromPosition(position);
                position.x++;
            }
            position.z++;
            position.x = -position.x; // reset position on x axis
        }
    }

    private void GenerateSquareFromPosition(Vector3 position) 
    {
        Vector3 VertexB = new(position.x + 1, position.y, position.z);
        Vector3 VertexC = new(position.x, position.y, position.z + 1);
        Vector3 VertexD = new(position.x + 1, position.y, position.z + 1);
        Square square = new Square(Squares.Count, position, VertexB, VertexC, VertexD);
        Squares.Add(square);
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
            float centerX = (VertexA.x + VertexB.x + VertexC.x + VertexD.x) / 4;
            float centerY = (VertexA.y + VertexB.y + VertexC.y + VertexD.y) / 4; // y axis for each vertex should always be 1
            float centerZ = (VertexA.z + VertexB.z + VertexC.z + VertexD.z) / 4;

            return new Vector3(centerX, centerY, centerZ);
        }
    }

}
