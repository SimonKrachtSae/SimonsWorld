using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public static World Instance;
    [SerializeField] private GameObject creeperPref;
    

    [SerializeField] private GameObject playerPref;

    [SerializeField] private GameObject m_EarthBlock;
    [SerializeField] private GameObject m_WaterBlock;
    [SerializeField] private GameObject m_SandBlock;
    [SerializeField] private GameObject m_StoneBlock;
    [SerializeField] private GameObject m_SpawnerBlock;
    
    private int scale;
    private int waterLevel = 3;
    private int StoneLevel;
    private float spawnTimer = 0;

    private int[,,] worldMap;
    private Cube[,,] worldCubes;
    private Cube[,,] waterWorldCubes;

    private List<Vector3> SpawnPoints;
    private List<GameObject> blocksInWorld = new List<GameObject>();
    
    private TerrainData terrainData;
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
        terrainData = perliNoise.GetTerrainData();
        scale = perliNoise.GetWorldSize();
        worldCubes = new Cube[scale,scale,scale];
        waterWorldCubes = new Cube[scale,scale,scale];

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
                            worldMap[x, y, z] = 2;
                        }
                        else if (y <= waterLevel)
                        { 
                            CreateCube(m_SandBlock, x, y, z);
                            worldMap[x, y, z] = 4;
                        }
                        else if(y >= waterLevel + StoneLevel)
                        {
                            CreateCube(m_StoneBlock, x, y, z);
                            worldMap[x, y, z] = 3;
                        }

                        for (int i = 0; i < y; i++)
                        {
                            if(i > waterLevel && i < waterLevel + StoneLevel)
                            {
                                worldMap[x, i, z] = 2;
                            }
                            else if(i <= waterLevel)
                            {
                                worldMap[x, i, z] = 4;
                            }
                            else if(i >= waterLevel + StoneLevel)
                            {
                                worldMap[x, i, z] = 3;
                            }
                        }
                    }
                    else if(y == Mathf.RoundToInt(terrainData.GetHeight(x, z)) + 1)
                    {
                            worldMap[x,y,z] = 1;
                    }
                    else
                    {
                        worldMap[x, y, z] = 0;
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
                    if(worldCubes[x,y,z] != null)
                        worldCubes[x, y, z].GenerateMesh();

                    if (waterWorldCubes[x, y, z] != null)
                        waterWorldCubes[x, y, z].GenerateMesh(); 
                }
            }
        }

        NodeManager.Instance.StartNodeManager();
        StaticBatchingUtility.Combine(blocksInWorld.ToArray(), gameObject);
        SpawnPlayer();
    }
    private void Update()
    {
        SpawnCreepers();
    }
    private void ConfigureWorld()
    {
        worldMap = new int[scale,scale,scale];

        for ( int x = 0; x < scale; x++)
        {
            for (int y = 0; y < scale;y++)
            {
                for (int z = 0; z < scale; z++)
                {
                    worldMap[x, y, z] = 0;
                    worldCubes[x, y, z] = null;
                    waterWorldCubes[x, y, z] = null;
                }
            }
        }
    }
    private void FillHoles()
    {
        for(int x = 0; x < scale; x++)
        {
            for (int z = 0; z < scale; z++)
            {
                int y = Mathf.RoundToInt( terrainData.GetHeight(x, z));
                if(worldMap[x,y,z] == 2|| worldMap[x, y, z] == 3|| worldMap[x, y, z] == 4)
                {
                    int distance = LowestNeighbourCubeDistance(x,y,z);
                    for(int i = 1; i <= distance; i++)
                    {
                        if (worldMap[x,y-i,z] == 2)
                        {
                            //world[x, y - i, z] = 2;
                            CreateCube(m_EarthBlock, x, y-i, z);
                        }
                        else if (worldMap[x, y - i, z] == 4)
                        {
                            //world[x, y-i, z] = 4;
                            CreateCube(m_SandBlock, x, y-i, z);
                        }
                        else if (worldMap[x, y - i, z] == 3)
                        {
                            //world[x, y-i, z] = 4;
                            CreateCube(m_StoneBlock, x, y - i, z);
                        }
                    }
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
                        worldMap[x, y, z] = 5;
                        CreateWaterCube(m_WaterBlock,x,y,z);
                        if(y == waterLevel)
                        {
                            worldMap[x, y + 1, z] = 1;
                        }
                    }
                }
            }
        }
    }
    void ConfigSpawnPoints()
    {
        SpawnPoints = new List<Vector3>();
        SpawnPoints.Add(new Vector3(0,Mathf.RoundToInt(terrainData.GetHeight(0,0)) + 2,0));
        SpawnPoints.Add(new Vector3(scale - 1,Mathf.RoundToInt(terrainData.GetHeight(scale -1,scale - 1)) + 2, scale - 1));
        SpawnPoints.Add(new Vector3(scale - 1,Mathf.RoundToInt(terrainData.GetHeight(scale - 1,0)) + 2,0));
        SpawnPoints.Add(new Vector3(0,Mathf.RoundToInt(terrainData.GetHeight(0,scale - 1)) + 2,scale - 1));

        worldMap[0, Mathf.RoundToInt(terrainData.GetHeight(0, 0))+1,0] = 6;
        worldMap[scale - 1, Mathf.RoundToInt(terrainData.GetHeight(scale - 1, 0)) + 1,0] = 6;
        worldMap[scale - 1, Mathf.RoundToInt(terrainData.GetHeight(scale - 1, scale - 1)) + 1,scale - 1] = 6;
        worldMap[0, Mathf.RoundToInt(terrainData.GetHeight(0, scale - 1)) + 1,scale - 1] = 6;

        CreateCube(m_SpawnerBlock, 0, Mathf.RoundToInt(terrainData.GetHeight(0, 0)) + 1, 0);
        CreateCube(m_SpawnerBlock, scale - 1, Mathf.RoundToInt(terrainData.GetHeight(scale - 1, 0)) + 1, 0);
        CreateCube(m_SpawnerBlock, scale - 1, Mathf.RoundToInt(terrainData.GetHeight(scale - 1,  scale - 1)) + 1,  scale - 1);
        CreateCube(m_SpawnerBlock, 0, Mathf.RoundToInt(terrainData.GetHeight(0, scale - 1)) + 1, scale - 1);
    }
    void SpawnPlayer()
    {
        int halfScale = Mathf.RoundToInt(scale / 2.0f);
        Vector3 spawnPoint = new Vector3(halfScale, terrainData.GetHeight(halfScale, halfScale) + 2, halfScale);
        Instantiate(playerPref, spawnPoint, Quaternion.identity);
    }
    void SpawnCreepers()
    {
        spawnTimer -= Time.fixedDeltaTime;

        if(spawnTimer <= 0.0f)
        {
            int randIndex = Random.Range(0, 3);
            Instantiate(creeperPref, SpawnPoints[randIndex], Quaternion.identity);
            spawnTimer = 12;
        }
    }
    void ConfigSurroundingCells(int _x, int _y, int _z)
    {
        if (worldCubes[_x, _y, _z] != null)
        {
            worldCubes[_x, _y, _z].surroundingCells = new List<Cube>();
        }
        if (waterWorldCubes[_x, _y, _z] != null)
        {
            waterWorldCubes[_x, _y, _z].surroundingCells = new List<Cube>();
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
                            if(worldMap[xVal, yVal, zVal] == 2 || worldMap[xVal, yVal, zVal] == 3 || worldMap[xVal, yVal, zVal] == 4)
                            {
                                if(worldCubes[xVal, yVal, zVal] != null && worldCubes[_x,_y,_z] != null)
                                worldCubes[_x, _y, _z].surroundingCells.Add(worldCubes[xVal, yVal, zVal]);
                            }
                            if(worldMap[xVal, yVal, zVal] == 5 && waterWorldCubes[_x,_y,_z]!= null)
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

    

    private void CreateCube(GameObject cube, int x, int y, int z)
    {
        GameObject block = Instantiate(cube, new Vector3(x, y, z), Quaternion.identity, this.transform);
        Cube myCube = block.GetComponent<Cube>();
        myCube.SetIndexes(x, y, z);
        worldCubes[x, y, z] = myCube;
        block.isStatic = true;
        blocksInWorld.Add(block);
    }
    private void CreateWaterCube(GameObject waterCube, int x, int y, int z)
    {
        GameObject block = Instantiate(waterCube, new Vector3(x, y, z), Quaternion.identity, this.transform);
        WaterCube myCube = block.GetComponent<WaterCube>();
        myCube.SetIndexes(x, y, z);
        waterWorldCubes[x, y, z] = myCube;
        block.isStatic = true;
        blocksInWorld.Add(block);
    }

    public void DestroyBlock(Cube cube)
    {
        NodeManager nodeManager = NodeManager.Instance;
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
                            if (worldCubes[xVal, yVal, zVal] == null)
                            {
                                if (worldMap[xVal, yVal, zVal] == 2)
                                {
                                    CreateCube(m_EarthBlock,xVal,yVal,zVal);
                                }
                                else if (worldMap[xVal, yVal, zVal] == 3)
                                {
                                    CreateCube(m_StoneBlock, xVal, yVal, zVal);
                                }
                                else if (worldMap[xVal, yVal, zVal] == 4)
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
                        if(worldCubes[xVal, yVal, zVal] != null)
                        {
                            ConfigSurroundingCells(xVal, yVal, zVal);
                            worldCubes[xVal, yVal, zVal].GenerateMesh();
                        }
                    }

                }
            }
        }

        if(worldMap[_x,_y + 1,_z] == 1)
        {
            worldMap[_x, _y + 1, _z] = 0;
        }

        if(worldMap[_x,_y + 1, _z] == 0 && worldMap[_x, _y - 1, _z] != 0)
        {
            worldMap[_x, _y, _z] = 1;
        }
        else
        {
            worldMap[_x, _y, _z] = 0;
        }

        if(_y <= waterLevel)
        {
            worldMap[_x, _y, _z] = 5;
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
        if(worldMap[_x,_y,_z] == 0 || worldMap[_x, _y, _z] == 1)
        {
            worldMap[_x, _y, _z] = 2;
            CreateCube(m_EarthBlock,_x,_y,_z);
            ConfigSurroundingCells(_x, _y, _z);
            worldCubes[_x, _y, _z].GenerateMesh();
        }
        if(_y + 1 < scale)
        {
            if(worldMap[_x,_y+1,_z] == 0)
            {
                worldMap[_x, _y + 1, _z] = 1;
            }
        }
        NodeManager.Instance.StartNodeManager();
    }
    public int GetWorldMapAtPos(int x, int y, int z)
    {
        return worldMap[x, y, z];
    }
    public void SetWorldMapAtPos(int x, int y, int z, int value)
    {
        worldMap[x, y, z] = value;
    }
    public int GetScale()
    {
        return scale;
    }
    public Cube GetWorldCubeAtIndex(int x,int y, int z)
    {
        return worldCubes[x, y, z];
    }
}
