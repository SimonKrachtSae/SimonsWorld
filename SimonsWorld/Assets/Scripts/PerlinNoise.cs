using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Inspired by Luca Martinelli (OCE Practice)
public class PerlinNoise : MonoBehaviour
{
    public static PerlinNoise Instance;

    [SerializeField] private TerrainData terrainData;
    private int worldSize;
    private float frequency = 1f;
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else if (Instance == null)
        {
            Instance = this;
        }

        GenerateIsland();
        //Fbm(1);
    }
    public TerrainData GetTerrainData()
    {
        return terrainData;
    }
    void GenerateIsland()
    {
        int seed = (int)System.DateTime.Now.Ticks;

        UnityEngine.Random.InitState(seed);
        float offsetX = UnityEngine.Random.Range(-2, 2);
        float offsetY = UnityEngine.Random.Range(-2, 2);
        Vector2 offset = new Vector2(offsetX, offsetY);

        worldSize = Random.Range(25,40);

        float[,] heights = new float[worldSize, worldSize];

        float xHalf = (float)worldSize / 2;
        float yHalf = (float)worldSize / 2;
        Vector2 middle = new Vector2(xHalf, yHalf);

        for (int x = 0; x < worldSize; ++x)
        {
            for (int y = 0; y < worldSize; ++y)
            {
                float percentX = (float)x / worldSize;
                float percentY = (float)y / worldSize;

                heights[x, y] = Mathf.PerlinNoise(percentX * worldSize * offsetX * frequency, percentY * worldSize * offsetY * frequency) * 0.05f;

                float distante = Vector2.Distance(new Vector2(0.5f, 0.5f), new Vector2(percentX, percentY)) * 2;

                heights[x, y] *= 1 - distante;
            }
        }
        terrainData.SetHeights(0, 0, heights);
    }
    public int GetWorldSize()
    {
        return worldSize;
    }
}
