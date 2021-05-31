using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoise : MonoBehaviour
{
    public TerrainData terrainData;
    public float scale;
    public float frequency = 0.1f;
    public float[,] heights;
    void Awake()
    {
        
        int size = terrainData.heightmapResolution;

        heights = new float[size, size];

        float xHalf = scale / 2;
        float yHalf = scale / 2;
        Vector2 middle = new Vector2(xHalf, yHalf);
        float maxDistance = (middle - new Vector2(0, 0)).magnitude;

        for (int x = 0; x < size; ++x)
        {
            for (int y = 0; y < size; ++y)
            {
                float percentX = (float)x / size;
                float percentY = (float)y / size;

                heights[x, y] = Mathf.PerlinNoise(percentX * scale * frequency, percentY * scale * frequency);

                float distante = Vector2.Distance(new Vector2(0.5f, 0.5f), new Vector2(percentX, percentY)) * 2;

                heights[x, y] *= 1 - distante;
            }
        }
        terrainData.SetHeights(0, 0, heights);
        
    }
}
