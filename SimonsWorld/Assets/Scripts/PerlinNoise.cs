using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoise : MonoBehaviour
{
    public TerrainData terrainData;
    public int scale;
    public float frequency = 1f;
    public float[,] heights;
    void Awake()
    {
        GenerateIsland();
        //Fbm(1);
    }
    void GenerateIsland()
    {
        int size = 40;

        heights = new float[size, size];

        float xHalf = (float)scale / 2;
        float yHalf = (float)scale / 2;
        Vector2 middle = new Vector2(xHalf, yHalf);
        float maxDistance = (middle - new Vector2(0, 0)).magnitude;

        for (int x = 0; x < size; ++x)
        {
            for (int y = 0; y < size; ++y)
            {
                float percentX = (float)x / size;
                float percentY = (float)y / size;

                heights[x, y] = Mathf.PerlinNoise(percentX * scale * frequency, percentY * scale * frequency) * 0.05f;

                float distante = Vector2.Distance(new Vector2(0.5f, 0.5f), new Vector2(percentX, percentY)) * 2;

                heights[x, y] *= 1 - distante;
            }
        }
        terrainData.SetHeights(0, 0, heights);
    }
    private void Fbm(int numOctaves, float scale = 25.0f, float lacunarity = 2.20f, float persistance = 0.5f)
    {
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        int size = terrainData.heightmapResolution;

        heights = new float[size, size];

        for (int x = 0; x < size; ++x)
        {
            for (int y = 0; y < size; ++y)
            {
                float value = 0;

                float amplitude = 0.5f;
                float frequency = 1.0f;

                for (int i = 0; i < numOctaves; i++)
                {
                    float sampleX = x / scale * frequency;
                    float sampleY = y / scale * frequency;

                    heights[x,y] = (Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1) * amplitude;

                    frequency *= lacunarity;
                    amplitude *= persistance;

                }
            }
        }
        terrainData.SetHeights(0, 0, heights);
    }
}
