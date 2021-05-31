using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyWorld : MonoBehaviour
{
    private TerrainData terrainData;
    private PerlinNoise perlinNoise;

    [SerializeField] private GameObject m_EarthBlock;
    [SerializeField] private GameObject m_WaterBlock;
    //[SerializeField] private GameObject m_StoneBlock;
    //[SerializeField] private GameObject m_EmptyBlock;
    private List<MyCube> m_cubes;
    private float scale;
    public float waterLevel = 3;
    private void Start()
    {
        perlinNoise = GetComponent<PerlinNoise>();
        terrainData = perlinNoise.terrainData;
        scale = perlinNoise.scale;

        m_cubes = new List<MyCube>();

        for(int x = 0; x < scale; x++)
        {
            for(int z = 0; z < scale; z++)
            {
                //int height = Mathf.RoundToInt(terrainData.GetHeight(Mathf.RoundToInt(x * (terrainData.size.x/scale)), Mathf.RoundToInt(z * (terrainData.size.z / scale))));
                int height = Mathf.RoundToInt(terrainData.GetHeight(x, z));
                for(int y = 0; y <= height; y++)
                {
                    CreateCube(m_EarthBlock, x, y, z);
                }

            }
        }

        GenerateWaterCubes();

        for(int i=0; i < m_cubes.Count; i++)
        {
            SetSurroundingCells(m_cubes[i]);
            m_cubes[i].GenerateMesh();
        }
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
                        CreateCube(m_WaterBlock,x,y,z);
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

}
