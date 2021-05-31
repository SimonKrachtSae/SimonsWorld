using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyNodeManager : MonoBehaviour
{
    private TerrainData terrainData;
    private PerlinNoise perlinNoise;

    public static MyNodeManager Instance;
    public List<Node> m_nodes = new List<Node>();
    [SerializeField] private MyWorld world;
    [SerializeField] private GameObject nodePref;

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
        perlinNoise = GetComponent<PerlinNoise>();
        terrainData = perlinNoise.terrainData;
    }
    public void StartNodeManager()
    {
        for(int x = 0; x < perlinNoise.scale; x++)
        {
            for (int z = 0; z < perlinNoise.scale; z++)
            {
                GameObject node = Instantiate(nodePref, new Vector3(x, world.heights[x, z] + 1, z), Quaternion.identity);
                m_nodes.Add(node.GetComponent<Node>());
            }
        }

        for (int i = 0; i < m_nodes.Count; i++)
        {
            m_nodes[i].ConfigSurroundingNodes();
        }
    }
    public float GetDistanceBetween(Node a, Node b)
    {
        return (b.transform.position - a.transform.position).magnitude;
    }
}
