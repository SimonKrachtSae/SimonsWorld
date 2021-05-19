using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCellularWorld : MonoBehaviour
{
    public static MyCellularWorld Instance;

    [SerializeField] private GameObject m_EarthBlock;
    [SerializeField] private GameObject m_WaterBlock;
    [SerializeField] private GameObject m_StoneBlock;
    [SerializeField] private GameObject m_EmptyBlock;
    public MyCube[,,] m_Cubes;

    [SerializeField, Range(0,100)] private float m_FillPercent;
    [SerializeField, Range(0,100)] private float m_ClimbPercent;


    public int[,,] M_World;
    public int m_WorldSizeX = 20;
    public int m_WorldSizeY = 4;
    public int m_WorldSizeZ = 20;

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

    void Start()
    {
        M_World = new int[m_WorldSizeX, m_WorldSizeY, m_WorldSizeZ];
        m_Cubes = new MyCube[m_WorldSizeX, m_WorldSizeY, m_WorldSizeZ];
        
        GenerateWorld();
        ConfigureSurroundingCells();
        NodeManager.Instance.StartNodeManager();
    }

    void GenerateWorld()
    {
        for (int x = 0; x < m_WorldSizeX; x++)
        {
            for (int y = 0; y < m_WorldSizeY; y++)
            {
                for (int z = 0; z < m_WorldSizeZ; z++)
                {
                    if(!(y == 0) && !(y==3))
                    {
                        M_World[x,y,z] = Random.Range(1, 101) <= m_FillPercent ? 0 : 1;


                        switch(M_World[x,y,z])
                        {
                            case 0:
                                CreateCube(m_EarthBlock, x, y, z);
                                break;
                            case 1:
                                CreateCube(m_WaterBlock, x, y, z);
                                break;
                                
                        }
                    }
                    else if(y == 0)
                    {
                        CreateCube(m_StoneBlock, x, y, z);
                        M_World[x, y, z] = 2;
                    }
                    else if(y == 3)
                    {

                        if(M_World[x,y-1,z] != 0)
                        {
                            CreateCube(m_EmptyBlock, x, y, z);
                            M_World[x, y, z] = 3;
                        }
                        if(M_World[x, y - 1, z] == 0)
                        {
                            M_World[x,y,z] = Random.Range(1, 101) <= m_ClimbPercent ? 0 : 3;
                            switch(M_World[x,y,z])
                            {
                                case 0:
                                    CreateCube(m_EarthBlock, x, y, z);
                                    break;
                                case 3:
                                    CreateCube(m_EmptyBlock, x, y, z);
                                    break;
                            }
                        }
                    }

                    m_Cubes[x, y, z].X_Index = x;
                    m_Cubes[x, y, z].Y_Index = y;
                    m_Cubes[x, y, z].Z_Index = z;
                    m_Cubes[x, y, z].GenerateMesh();
                }
            }
        }
    }
    private void CreateCube(GameObject cube, int x, int y, int z)
    {
        GameObject block = Instantiate(cube, new Vector3(x, y, z), Quaternion.identity);
        m_Cubes[x, y, z] = block.GetComponent<MyCube>();
    }
    private void ConfigureSurroundingCells()
    {
        for (int x = 0; x < m_WorldSizeX; x++)
        {
            for (int y = 0; y < m_WorldSizeY; y++)
            {
                for (int z = 0; z < m_WorldSizeZ; z++)
                { 
                    m_Cubes[x,y,z].surroundingCells = new List<MyCube>();

                    for (int _x = -1; _x <= 1; _x++)
                    {
                        for (int _y = -1; _y <= 1; _y++)
                        {
                            for (int _z = -1; _z <= 1; _z++)
                            {
                                if (!(_x == 0 && _y == 0 && _z == 0))
                                {
                                    if (x + _x >= 0 && x + _x < m_WorldSizeX)
                                    {
                                        if (y + _y >= 0 && y + _y < m_WorldSizeY)
                                        {
                                            if (z + _z >= 0 && z + _z < m_WorldSizeZ)
                                            {
                                                m_Cubes[x, y, z].surroundingCells.Add(m_Cubes[x + _x, y + _y, z + _z].GetComponent<MyCube>());
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
