using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyWorld : MonoBehaviour
{
    public static MyWorld Instance;

    [SerializeField] private GameObject m_EarthBlock;
    [SerializeField] private GameObject m_WaterBlock;
    [SerializeField] private GameObject m_SandBlock;
    [SerializeField] private GameObject m_StoneBlock;

    [SerializeField] private int waterLevel = 3;

    [SerializeField, Range(0, 100)] private int StoneBlockFillPercent;

    private int[,] heights;
    public int[,,] world;
    public MyCube[,,] WorldCubes;
    public MyCube[,,] waterWorldCubes;
    List<MyCube>blocks = new List<MyCube>();
    List<GameObject> boxes = new List<GameObject>();
    public int scale;
    List<MyWaterCube> waterCubes;
    TerrainData terrainData;
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
                    if (y == Mathf.RoundToInt(terrainData.GetHeight(x, z)))
                    {
                        if (y > waterLevel)
                        {
                            CreateCube(m_EarthBlock, x, y, z);
                            world[x, y, z] = 2;
                        }
                        if (y <= waterLevel)
                        { 
                            CreateCube(m_SandBlock, x, y, z);
                            world[x, y, z] = 4;
                        }
                        for (int i = 0; i < y; i++)
                        {
                            if(i > waterLevel)
                            {
                                world[x, i, z] = 2;
                            }
                            if(i <= waterLevel)
                            {
                                world[x, i, z] = 4;
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
                //heights[x,z] = Mathf.RoundToInt(terrainData.GetHeight(x, z));
                //
                //if(heights[x,z] > waterLevel)
                //{
                //    int value = Random.Range(0, 100);
                //
                //    if(value >= StoneBlockFillPercent)
                //    {
                //        CreateCube(m_EarthBlock, x, heights[x,z], z);
                //    }
                //    else if (value < StoneBlockFillPercent)
                //    {
                //        CreateCube(m_StoneBlock, x, heights[x, z], z);
                //    }
                //}
                //if(heights[x,z] <= waterLevel)
                //{
                //    CreateCube(m_SandBlock, x, heights[x, z], z);
                //}
                //for(int i = 0; i <= heights[x,z];i++)
                //{
                //    if(heights[x,z] < scale)
                //    world[x, i, z] = 1;
                //}
            }
        }
        FillHoles();
        //SpawnWaterCubes();
        //GenerateWaterWorld();
        GenerateWaterCubes(terrainData);
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

        //for(int i=0; i < blocks.Count; i++)
        //{
        //    SetSurroundingCells(blocks[i]);
        //    blocks[i].GenerateMesh();
        //}
        //for (int i = 0; i < waterCubes.Count; i++)
        //{
        //    SetSurroundingWaterCells(waterCubes[i], waterCubes);
        //    waterCubes[i].GenerateMesh();
        //}
        MyNodeManager.Instance.StartNodeManager();
        //StaticBatchingUtility.Combine(boxes.ToArray(), gameObject);
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

    void GenerateWaterWorld()
    {
        for (int x = 0; x < scale; x++)
        {
            for (int z = 0; z < scale; z++)
            {
                for (int y = 0; y < scale; y++)
                {
                    if (world[x, y, z] == 5)
                    {
                        CreateWaterCube(m_WaterBlock, x, y, z);
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
                        if (world[x, y - i, z] == 4)
                        {
                            //world[x, y-i, z] = 4;
                            CreateCube(m_SandBlock, x, y-i, z);
                        }
                    }
                }
                
            }
        }
        //for (int x = 0; x < scale; x++)
        //{
        //    for(int z = 0; z < scale; z++)
        //    {
        //        if (heights[x, z] > 0)
        //        {
        //            int offset = GetLowestNeighbourCubeDistance(x,z, scale);
        //            if(offset >= 1)
        //            {
        //                for(int y = 1; y <= offset; y++)
        //                {
        //                    if(heights[x,z] - y >= 0)
        //                    CreateCube(m_EarthBlock, x, heights[x, z] - y, z);
        //                }
        //            }
        //        }
        //
        //    }
        //}
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
    private int GetLowestNeighbourCubeDistance(int _x, int _z, int scale)
    {
        int offset = 100;
        int value = 0;
        for (int x = -1; x <= 1; x++)
        {
            if(x != 0)
            {
                if(_x + x >= 0 &&_x + x < scale)
                {
                    if (heights[_x, _z] > heights[_x + x, _z])
                    {
                        value = Mathf.Abs(heights[_x + x, _z] - heights[_x, _z]);
                        if(value < offset)
                        {
                            offset = value;
                        }
                    }
                }
            }
        }
        for (int z = -1; z <= 1; z++)
        {
            if (z != 0)
            {
                if (_z + z >= 0 && _z + z < scale)
                {
                    
                    if (heights[_x, _z] > heights[_x, _z + z])
                    {
                        value = Mathf.Abs(heights[_x, _z + z] - heights[_x, _z]);
                        if (value < offset)
                        {
                            offset = value;
                        }
                    }
                }
            }

        }
        if(offset == 100)
        {
            offset = 0;
            return offset;
        }
        return offset + 1;
    }
    private void SpawnWaterCubes()
    {
        for (int x = 0; x < scale; x++)
        {
            for (int z = 0; z < scale; z++)
            {
                for (int y = 0; y < scale; y++)
                {
                    if(world[x,y,z] == 2 || world[x, y, z] == 3 || world[x, y, z] == 4)
                    {
                        if(y < waterLevel)
                        {
                            for(int i = y; i <= waterLevel; y++)
                            {
                                CreateWaterCube(m_WaterBlock, x, y, z);
                            }
                        }
                    }
                }
            }
        }
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
    private MyCube CreateNewCube(GameObject cube, int x, int y, int z)
    {
        GameObject block = Instantiate(cube, new Vector3(x, y, z), Quaternion.identity, this.transform);
        MyCube myCube = block.GetComponent<MyCube>();
        myCube.X_Index = x;
        myCube.Y_Index = y;
        myCube.Z_Index = z;
        waterWorldCubes[x, y, z] = myCube;

        //blocks.Add(myCube);
        block.isStatic = true;
        boxes.Add(block);
        //myCube.GenerateMesh();
        return myCube;
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
    private void SetSurroundingCells(MyCube cube)
    {
        cube.surroundingCells = new List<MyCube>();
        for(int i = 0; i < blocks.Count; i++)
        {
            if((blocks[i].transform.position - cube.transform.position).magnitude < 1.8f)
            {
                if(cube != blocks[i])
                {
                    if(!cube.surroundingCells.Contains(blocks[i]))
                    {
                        cube.surroundingCells.Add(blocks[i]);
                    }
                }
            }
        }
    }
    private void SetSurroundingWaterCells(MyWaterCube cube, List<MyWaterCube> myWaterCubes)
    {
        cube.surroundingWaterCubes = new List<MyWaterCube>();
        for (int i = 0; i < myWaterCubes.Count; i++)
        {
            if ((myWaterCubes[i].transform.position - cube.transform.position).magnitude < 1.8f)
            {
                if (cube != myWaterCubes[i])
                {
                    cube.surroundingWaterCubes.Add(myWaterCubes[i]);
                }
            }
        }
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
    private int HighestCube(int _x, int _y, int _z)
    {
        for (int i = _y - 1 ; i >= 0; i++)
        {
            if(world[_x, i, _z] == 2 || world[_x, i, _z] == 3 || world[_x, i, _z] == 4)
            {
                return i;
            }
        }
        return 0;
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
