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

    [SerializeField] private float waterLevel = 3;

    [SerializeField, Range(0, 100)] private int StoneBlockFillPercent;

    private int[,] heights;
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
        List<MyWaterCube> waterCubes = new List<MyWaterCube>();
        TerrainData terrainData = perliNoise.GetTerrainData();
        int scale = perliNoise.WorldSize;

        List<MyCube>myCubes = new List<MyCube>();

        heights = new int[scale, scale];

        for(int x = 0; x < scale; x++)
        {
            for(int z = 0; z < scale; z++)
            {
                heights[x,z] = Mathf.RoundToInt(terrainData.GetHeight(x, z));


                if(heights[x,z] > waterLevel)
                {
                    int value = Random.Range(0, 100);

                    if(value >= StoneBlockFillPercent)
                    {
                        CreateCube(m_EarthBlock, x, heights[x,z], z,myCubes);
                    }
                    else if (value < StoneBlockFillPercent)
                    {
                        CreateCube(m_StoneBlock, x, heights[x, z], z, myCubes);
                    }
                }
                if(heights[x,z] <= waterLevel)
                {
                    CreateCube(m_SandBlock, x, heights[x, z], z, myCubes);
                }
            }
        }
        FillHoles(scale, myCubes);
        GenerateWaterCubes(scale, terrainData, waterCubes);

        for(int i=0; i < myCubes.Count; i++)
        {
            SetSurroundingCells(myCubes[i], myCubes);
            myCubes[i].GenerateMesh();
        }
        for (int i = 0; i < waterCubes.Count; i++)
        {
            SetSurroundingWaterCells(waterCubes[i], waterCubes);
            waterCubes[i].GenerateMesh();
        }
        MyNodeManager.Instance.StartNodeManager();
    }
    public int GetHeight(int x, int z)
    {
        return heights[x, z];
    }

    private void FillHoles(int scale, List<MyCube> myCubes)
    {
        for (int x = 0; x < scale; x++)
        {
            for(int z = 0; z < scale; z++)
            {
                if (heights[x, z] > 0)
                {
                    int offset = GetLowestNeighbourCubeDistance(x,z, scale);
                    if(offset >= 1)
                    {
                        for(int y = 1; y <= offset; y++)
                        {
                            if(heights[x,z] - y >= 0)
                            CreateCube(m_EarthBlock, x, heights[x, z] - y, z, myCubes);
                        }
                    }
                }

            }
        }
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
    private void GenerateWaterCubes(int scale, TerrainData terrainData, List<MyWaterCube> myWaterCubes)
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
                        CreateWaterCube(m_WaterBlock,x,y,z, myWaterCubes);
                    }
                }
            }
        }
    }
    private void CreateCube(GameObject cube, int x, int y, int z, List<MyCube> myCubes)
    {
        GameObject block = Instantiate(cube, new Vector3(x, y, z), Quaternion.identity);
        MyCube myCube = block.GetComponent<MyCube>();
        myCube.X_Index = x;
        myCube.Y_Index = y;
        myCube.Z_Index = z;
        myCubes.Add(myCube);
    }
    private void CreateWaterCube(GameObject waterCube, int x, int y, int z, List<MyWaterCube> myWaterCubes)
    {
        GameObject block = Instantiate(waterCube, new Vector3(x, y, z), Quaternion.identity);
        MyWaterCube myCube = block.GetComponent<MyWaterCube>();
        myCube.X_Index = x;
        myCube.Y_Index = y;
        myCube.Z_Index = z;
        myWaterCubes.Add(myCube);
    }
    private void SetSurroundingCells(MyCube cube, List<MyCube>myCubes)
    {
        cube.surroundingCells = new List<MyCube>();
        for(int i = 0; i < myCubes.Count; i++)
        {
            if((myCubes[i].transform.position - cube.transform.position).magnitude < 1.8f)
            {
                if(cube != myCubes[i])
                {
                    cube.surroundingCells.Add(myCubes[i]);
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
}
