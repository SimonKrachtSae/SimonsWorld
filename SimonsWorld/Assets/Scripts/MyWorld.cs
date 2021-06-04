using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyWorld : MonoBehaviour
{
    public static MyWorld Instance;
    [SerializeField] private GameObject CreeperPref;
    private float spawnTimer = 0;
    private List<Vector3> SpawnPoints;

    [SerializeField] private GameObject PlayerPref;

    [SerializeField] private GameObject m_EarthBlock;
    [SerializeField] private GameObject m_WaterBlock;
    [SerializeField] private GameObject m_SandBlock;
    [SerializeField] private GameObject m_StoneBlock;
    [SerializeField] private GameObject m_SpawnerBlock;

    [SerializeField] private int waterLevel = 3;

    [SerializeField, Range(0, 100)] private int StoneBlockFillPercent;

    private int[,] heights;
    public int[,,] world;
    public MyCube[,,] WorldCubes;
    public MyCube[,,] waterWorldCubes;
    private List<MyCube>blocks = new List<MyCube>();
    private List<GameObject> boxes = new List<GameObject>();
    public int scale;
    private List<MyWaterCube> waterCubes;
    private TerrainData terrainData;
    private int StoneLevel;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else if (Instance == null)
        {
            Instance = this;
        }
    }
    private void Start()
    {

        int seed = (int)System.DateTime.Now.Ticks;

        UnityEngine.Random.InitState(seed);
        waterLevel = UnityEngine.Random.Range(2, 5);
        StoneLevel = UnityEngine.Random.Range(10, 14);


        PerlinNoise perliNoise = PerlinNoise.Instance;
        waterCubes = new List<MyWaterCube>();
        terrainData = perliNoise.GetTerrainData();
        scale = perliNoise.WorldSize;
        WorldCubes = new MyCube[scale,scale,scale];
        waterWorldCubes = new MyCube[scale,scale,scale];

        ConfigureWorld();

        for(int x = 0; x < scale; x++)
        {
            for(int z = 0; z < scale; z++)
            {
                for(int y = 0; y < scale; y++)
                {
                    //0 = air
                    //1 = node
                    //2 = earthBlock
                    //3 = stoneBlock
                    //4 = sandBlock
                    //5 = waterBlock 
                    //6 = spawner
                    if (y == Mathf.RoundToInt(terrainData.GetHeight(x, z)))
                    {
                        if (y > waterLevel && y < waterLevel + StoneLevel)
                        {
                            CreateCube(m_EarthBlock, x, y, z);
                            world[x, y, z] = 2;
                        }
                        else if (y <= waterLevel)
                        { 
                            CreateCube(m_SandBlock, x, y, z);
                            world[x, y, z] = 4;
                        }
                        else if(y >= waterLevel + StoneLevel)
                        {
                            CreateCube(m_StoneBlock, x, y, z);
                            world[x, y, z] = 3;
                        }

                        for (int i = 0; i < y; i++)
                        {
                            if(i > waterLevel && i < waterLevel + StoneLevel)
                            {
                                world[x, i, z] = 2;
                            }
                            else if(i <= waterLevel)
                            {
                                world[x, i, z] = 4;
                            }
                            else if(i >= waterLevel + StoneLevel)
                            {
                                world[x, i, z] = 3;
                            }
                        }
                    }
                    else if(y == Mathf.RoundToInt(terrainData.GetHeight(x, z)) + 1)
                    {
                            world[x,y,z] = 1;
                    }
                    else
                    {
                        world[x, y, z] = 0;
                    }
                }
            }
        }
        FillHoles();
        GenerateWaterCubes(terrainData);
        ConfigSpawnPoints();
        for (int x = 0; x < scale; x++)
        {
            for (int y = 0; y < scale; y++)
            {
                for (int z = 0; z < scale; z++)
                {
                    ConfigSurroundingCells(x,y,z);
                }
            }
        }
        for (int x = 0; x < scale; x++)
        {
            for (int y = 0; y < scale; y++)
            {
                for (int z = 0; z < scale; z++)
                {
                    if(WorldCubes[x,y,z] != null)
                        WorldCubes[x, y, z].GenerateMesh();

                    if (waterWorldCubes[x, y, z] != null)
                        waterWorldCubes[x, y, z].GenerateMesh(); 
                }
            }
        }

        MyNodeManager.Instance.StartNodeManager();
        StaticBatchingUtility.Combine(boxes.ToArray(), gameObject);
        SpawnPlayer();
    }
    private void Update()
    {
        SpawnCreepers();
    }
    void ConfigSpawnPoints()
    {
        SpawnPoints = new List<Vector3>();
        SpawnPoints.Add(new Vector3(0,Mathf.RoundToInt(terrainData.GetHeight(0,0)) + 2,0));
        SpawnPoints.Add(new Vector3(scale - 1,Mathf.RoundToInt(terrainData.GetHeight(scale -1,scale - 1)) + 2, scale - 1));
        SpawnPoints.Add(new Vector3(scale - 1,Mathf.RoundToInt(terrainData.GetHeight(scale - 1,0)) + 2,0));
        SpawnPoints.Add(new Vector3(0,Mathf.RoundToInt(terrainData.GetHeight(0,scale - 1)) + 2,scale - 1));

        world[0, Mathf.RoundToInt(terrainData.GetHeight(0, 0))+1,0] = 6;
        world[scale - 1, Mathf.RoundToInt(terrainData.GetHeight(scale - 1, 0)) + 1,0] = 6;
        world[scale - 1, Mathf.RoundToInt(terrainData.GetHeight(scale - 1, scale - 1)) + 1,scale - 1] = 6;
        world[0, Mathf.RoundToInt(terrainData.GetHeight(0, scale - 1)) + 1,scale - 1] = 6;

        CreateCube(m_SpawnerBlock, 0, Mathf.RoundToInt(terrainData.GetHeight(0, 0)) + 1, 0);
        CreateCube(m_SpawnerBlock, scale - 1, Mathf.RoundToInt(terrainData.GetHeight(scale - 1, 0)) + 1, 0);
        CreateCube(m_SpawnerBlock, scale - 1, Mathf.RoundToInt(terrainData.GetHeight(scale - 1,  scale - 1)) + 1,  scale - 1);
        CreateCube(m_SpawnerBlock, 0, Mathf.RoundToInt(terrainData.GetHeight(0, scale - 1)) + 1, scale - 1);
    }
    void SpawnPlayer()
    {
        int halfScale = Mathf.RoundToInt(scale / 2.0f);
        Vector3 spawnPoint = new Vector3(halfScale, terrainData.GetHeight(halfScale, halfScale) + 2, halfScale);
        Instantiate(PlayerPref, spawnPoint, Quaternion.identity);
    }
    void SpawnCreepers()
    {
        spawnTimer -= Time.fixedDeltaTime;

        if(spawnTimer <= 0.0f)
        {
            int randIndex = Random.Range(0, 3);
            Instantiate(CreeperPref, SpawnPoints[randIndex], Quaternion.identity);
            spawnTimer = 7;
        }
        Debug.Log(spawnTimer);
    }
    void ConfigSurroundingCells(int _x, int _y, int _z)
    {
        if (WorldCubes[_x, _y, _z] != null)
        {
            WorldCubes[_x, _y, _z].surroundingCells = new List<MyCube>();
        }
        if (waterWorldCubes[_x, _y, _z] != null)
        {
            waterWorldCubes[_x, _y, _z].surroundingCells = new List<MyCube>();
        }

        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if(!(x == 0 && y == 0&& z == 0))
                    {
                        int xVal = _x + x;
                        int yVal = _y + y;
                        int zVal = _z + z;
                        if ((xVal >= 0 && xVal < scale) 
                            && (yVal >= 0 && yVal < scale)
                            && (zVal >= 0 && zVal < scale))
                        {
                            if(world[xVal, yVal, zVal] == 2 || world[xVal, yVal, zVal] == 3 || world[xVal, yVal, zVal] == 4)
                            {
                                if(WorldCubes[xVal, yVal, zVal] != null && WorldCubes[_x,_y,_z] != null)
                                WorldCubes[_x, _y, _z].surroundingCells.Add(WorldCubes[xVal, yVal, zVal]);
                            }
                            if(world[xVal, yVal, zVal] == 5 && waterWorldCubes[_x,_y,_z]!= null)
                            {
                                if(waterWorldCubes[xVal, yVal, zVal] != null)
                                    waterWorldCubes[_x, _y, _z].surroundingCells.Add(waterWorldCubes[xVal, yVal, zVal]);
                            }
                        }
                    }
                }
            }
        }
    }
    public int GetHeight(int x, int z)
    {
        return heights[x, z];
    }

    private void FillHoles()
    {
        for(int x = 0; x < scale; x++)
        {
            for (int z = 0; z < scale; z++)
            {
                int y = Mathf.RoundToInt( terrainData.GetHeight(x, z));
                if(world[x,y,z] == 2|| world[x, y, z] == 3|| world[x, y, z] == 4)
                {
                    int distance = LowestNeighbourCubeDistance(x,y,z);
                    for(int i = 1; i <= distance; i++)
                    {
                        if (world[x,y-i,z] == 2)
                        {
                            //world[x, y - i, z] = 2;
                            CreateCube(m_EarthBlock, x, y-i, z);
                        }
                        else if (world[x, y - i, z] == 4)
                        {
                            //world[x, y-i, z] = 4;
                            CreateCube(m_SandBlock, x, y-i, z);
                        }
                        else if (world[x, y - i, z] == 3)
                        {
                            //world[x, y-i, z] = 4;
                            CreateCube(m_StoneBlock, x, y - i, z);
                        }
                    }
                }
                
            }
        }
    }
    
    private void ConfigureWorld()
    {
        world = new int[scale,scale,scale];

        for ( int x = 0; x < scale; x++)
        {
            for (int y = 0; y < scale;y++)
            {
                for (int z = 0; z < scale; z++)
                {
                    world[x, y, z] = 0;
                    WorldCubes[x, y, z] = null;
                    waterWorldCubes[x, y, z] = null;
                }
            }
        }
    }
    private int LowestNeighbourCubeDistance(int _x, int _y, int _z)
    {
        int offset = 100;
        for (int x = -1; x <= 1; x++)
        {
            if (x != 0)
            {
                int y = Mathf.RoundToInt(terrainData.GetHeight(x, _z));
                if(y < _y - 1)
                {
                    int value = _y - y;
                    if(value < offset)
                    {
                        offset = value;
                    }
                }
            }
        }
        for (int z = -1; z <= 1; z++)
        {
            if (z != 0)
            {
                int y = Mathf.RoundToInt(terrainData.GetHeight(_x, z));
                if (y < _y - 1)
                {
                    int value = _y - y;
                    if (value < offset)
                    {
                        offset = value;
                    }
                }
            }
        }
        if (offset == 100)
        {
            offset = 0;
            return offset;
        }
        return offset;
    }

    private void GenerateWaterCubes(TerrainData terrainData)
    {
        for(int x = 0; x < scale; x++)
        {
            for (int z = 0; z < scale; z++)
            {
                int height = Mathf.RoundToInt(terrainData.GetHeight(x, z));
                if(height < waterLevel)
                {
                    for(int y = height + 1; y <= waterLevel; y++)
                    {
                        world[x, y, z] = 5;
                        CreateWaterCube(m_WaterBlock,x,y,z);
                        if(y == waterLevel)
                        {
                            world[x, y + 1, z] = 1;
                        }
                    }
                }
            }
        }
    }
    private void CreateCube(GameObject cube, int x, int y, int z)
    {
        GameObject block = Instantiate(cube, new Vector3(x, y, z), Quaternion.identity, this.transform);
        MyCube myCube = block.GetComponent<MyCube>();
        myCube.X_Index = x;
        myCube.Y_Index = y;
        myCube.Z_Index = z;
        WorldCubes[x, y, z] = myCube;
        //blocks.Add(myCube);
        block.isStatic = true;
        boxes.Add(block);
    }
    private void CreateWaterCube(GameObject waterCube, int x, int y, int z)
    {
        GameObject block = Instantiate(waterCube, new Vector3(x, y, z), Quaternion.identity, this.transform);
        MyWaterCube myCube = block.GetComponent<MyWaterCube>();
        myCube.X_Index = x;
        myCube.Y_Index = y;
        myCube.Z_Index = z;
        waterWorldCubes[x, y, z] = myCube;
        //waterCubes.Add(myCube);
        block.isStatic = true;
        boxes.Add(block);
    }

    public void DestroyBlock(MyCube cube)
    {
        MyNodeManager nodeManager = MyNodeManager.Instance;
        int _x = Mathf.RoundToInt(cube.transform.position.x);
        int _y = Mathf.RoundToInt(cube.transform.position.y);
        int _z = Mathf.RoundToInt(cube.transform.position.z);
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    if(x != 0 && y != 0 && z != 0)
                    {

                        int xVal = _x + x;
                        int yVal = _y + y;
                        int zVal = _z + z;
                        if ((xVal >= 0 && xVal < scale)
                            && (yVal >= 0 && yVal < scale)
                            && (zVal >= 0 && zVal < scale))
                        {
                            if (WorldCubes[xVal, yVal, zVal] == null)
                            {
                                if (world[xVal, yVal, zVal] == 2)
                                {
                                    CreateCube(m_EarthBlock,xVal,yVal,zVal);
                                }
                                else if (world[xVal, yVal, zVal] == 3)
                                {
                                    CreateCube(m_StoneBlock, xVal, yVal, zVal);
                                }
                                else if (world[xVal, yVal, zVal] == 4)
                                {
                                    CreateCube(m_SandBlock, xVal, yVal, zVal);
                                }
                            }
                        }
                    }
                }
            }
        }
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    int xVal = _x + x;
                    int yVal = _y + y;
                    int zVal = _z + z;
                    if ((xVal >= 0 && xVal < scale)
                        && (yVal >= 0 && yVal < scale)
                        && (zVal >= 0 && zVal < scale))
                    {
                        if(WorldCubes[xVal, yVal, zVal] != null)
                        {
                            ConfigSurroundingCells(xVal, yVal, zVal);
                            WorldCubes[xVal, yVal, zVal].GenerateMesh();
                        }
                    }

                }
            }
        }

        if(world[_x,_y + 1,_z] == 1)
        {
            world[_x, _y + 1, _z] = 0;
        }

        if(world[_x,_y + 1, _z] == 0 && world[_x, _y - 1, _z] != 0)
        {
            world[_x, _y, _z] = 1;
        }
        else
        {
            world[_x, _y, _z] = 0;
        }

        if(_y <= waterLevel)
        {
            world[_x, _y, _z] = 5;
            CreateWaterCube(m_WaterBlock,_x,_y,_z);
            ConfigSurroundingCells(_x,_y, _z);
            waterWorldCubes[_x, _y, _z].GenerateMesh();
        }
        //world[_x, _y,_z] = 0;
        Destroy(cube.gameObject);
        nodeManager.StartNodeManager();
    }
    public void CreateBlock(int _x, int _y, int _z)
    {
        if(world[_x,_y,_z] == 0 || world[_x, _y, _z] == 1)
        {
            world[_x, _y, _z] = 2;
            CreateCube(m_EarthBlock,_x,_y,_z);
            ConfigSurroundingCells(_x, _y, _z);
            WorldCubes[_x, _y, _z].GenerateMesh();
        }
        if(_y + 1 < scale)
        {
            if(world[_x,_y+1,_z] == 0)
            {
                world[_x, _y + 1, _z] = 1;
            }
        }
        MyNodeManager.Instance.StartNodeManager();
    }
    public MyCube GetCubeAtPosition(Vector3 position)
    {
        MyCube cube = null;
        for(int i = 0; i < blocks.Count; i++)
        {
            if((blocks[i].transform.position - position).magnitude < 0.1f)
            {
                cube = blocks[i];
            }
        }
        return cube;
    }
}
