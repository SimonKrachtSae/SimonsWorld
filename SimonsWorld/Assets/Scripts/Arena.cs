using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour
{
    public static Arena Instance;
    public GameObject[,] cubes;
    [SerializeField] private GameObject cube;
    [SerializeField] private int X_Size;
    [SerializeField] private int Y_Size;
    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
        }
        else if(Instance == null)
        {
            Instance = this;
        }
    }
    void Start()
    {
        cubes = new GameObject[X_Size,Y_Size];
        SpawnArena();
        ConfigureSurroundingCells();
    }

    void Update()
    {
       
    }
    private void SpawnArena()
    {
        for(int x = 0; x < X_Size; x++)
        {
            for (int y = 0; y < Y_Size; y++)
            {
                GameObject newCube = Instantiate(cube, new Vector3(x,y,0), Quaternion.identity);
                cubes[x, y] = newCube;
                cubes[x,y].GetComponent<MyCube>().X_Index = x;
                cubes[x,y].GetComponent<MyCube>().Y_Index = y;
            }
        }
    }
    private void ConfigureSurroundingCells()
    {
        for (int x = 0; x < X_Size; x++)
        {
            for (int y = 0; y < Y_Size; y++)
            {
                MyCube cube = cubes[x, y].GetComponent<MyCube>();

                cube.surroundingCells = new List<MyCube>();

                for (int _x = -1; _x <= 1; _x++)
                {
                    for (int _y = -1; _y <= 1; _y++)
                    {
                        if (!((_x == 0) && (_y == 0)))
                        {
                            if (x + _x >= 0 && x + _x <= X_Size - 1)
                            {
                                if (y + _y >= 0 && y + _y <= Y_Size - 1)
                                {
                                    cube.surroundingCells.Add(cubes[x + _x, y + _y].GetComponent<MyCube>());
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    
}
