using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyWorld : MonoBehaviour
{
    private TerrainData terrainData;
    private PerlinNoise perlinNoise;

    [SerializeField] private GameObject m_EarthBlock;
    [SerializeField] private GameObject m_WaterBlock;
    [SerializeField] private GameObject m_SandBlock;
    [SerializeField] private GameObject m_StoneBlock;
    //[SerializeField] private GameObject m_EmptyBlock;
    private List<MyCube> m_cubes;
    private List<MyWaterCube> waterCubes;
    private int scale;
    public float waterLevel = 3;

    public int[,] heights;
    [SerializeField, Range(0, 100)] private int StoneBlockFillPercent;

    private void Start()
    {
        waterCubes = new List<MyWaterCube>();
        perlinNoise = GetComponent<PerlinNoise>();
        terrainData = perlinNoise.terrainData;
        scale = perlinNoise.scale;

        m_cubes = new List<MyCube>();

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
                        CreateCube(m_EarthBlock, x, heights[x,z], z);
                    }
                    else if (value < StoneBlockFillPercent)
                    {
                        CreateCube(m_StoneBlock, x, heights[x, z], z);
                    }
                }
                if(heights[x,z] <= waterLevel)
                {
                    CreateCube(m_SandBlock, x, heights[x, z], z);
                }
            }
        }
        FillHoles();
        GenerateWaterCubes();

        for(int i=0; i < m_cubes.Count; i++)
        {
            SetSurroundingCells(m_cubes[i]);
            m_cubes[i].GenerateMesh();
        }
        for (int i = 0; i < waterCubes.Count; i++)
        {
            SetSurroundingWaterCells(waterCubes[i]);
            waterCubes[i].GenerateMesh();
        }
        MyNodeManager.Instance.StartNodeManager();
    }
    private void FillHoles()
    {
        for (int x = 0; x < scale; x++)
        {
            for(int z = 0; z < scale; z++)
            {
                if (heights[x, z] > 0)
                {
                    int offset = GetLowestNeighbourCubeDistance(x,z);
                    if(offset >= 1)
                    {
                        for(int y = 1; y <= offset; y++)
                        {
                            if(heights[x,z] - y >= 0)
                            CreateCube(m_EarthBlock, x, heights[x, z] - y, z);
                        }
                    }
                }

            }
        }
    }
    private int GetLowestNeighbourCubeDistance(int _x, int _z)
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
    private void GenerateWaterCubes()
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
                        CreateWaterCube(m_WaterBlock,x,y,z);
                    }
                }
            }
        }
    }
    private void CreateCube(GameObject cube, int x, int y, int z)
    {
        GameObject block = Instantiate(cube, new Vector3(x, y, z), Quaternion.identity);
        MyCube myCube = block.GetComponent<MyCube>();
        myCube.X_Index = x;
        myCube.Y_Index = y;
        myCube.Z_Index = z;
        m_cubes.Add(myCube);
    }
    private void CreateWaterCube(GameObject waterCube, int x, int y, int z)
    {
        GameObject block = Instantiate(waterCube, new Vector3(x, y, z), Quaternion.identity);
        MyWaterCube myCube = block.GetComponent<MyWaterCube>();
        myCube.X_Index = x;
        myCube.Y_Index = y;
        myCube.Z_Index = z;
        waterCubes.Add(myCube);
    }
    private void SetSurroundingCells( MyCube cube)
    {
        cube.surroundingCells = new List<MyCube>();
        for(int i = 0; i < m_cubes.Count; i++)
        {
            if((m_cubes[i].transform.position - cube.transform.position).magnitude < 1.8f)
            {
                if(cube != m_cubes[i])
                {
                    cube.surroundingCells.Add(m_cubes[i]);
                }
            }
        }
    }
    private void SetSurroundingWaterCells(MyWaterCube cube)
    {
        cube.surroundingWaterCubes = new List<MyWaterCube>();
        for (int i = 0; i < waterCubes.Count; i++)
        {
            if ((waterCubes[i].transform.position - cube.transform.position).magnitude < 1.8f)
            {
                if (cube != m_cubes[i])
                {
                    cube.surroundingWaterCubes.Add(waterCubes[i]);
                }
            }
        }
    }
}
