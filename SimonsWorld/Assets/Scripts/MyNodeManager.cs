using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyNodeManager : MonoBehaviour
{
    public static MyNodeManager Instance;
    private List<Node> m_nodes;
    private List<GameObject> m_nodeObjs;
    [SerializeField] private GameObject nodePref;

    MyWorld world;

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

    public void StartNodeManager()
    {
        if(m_nodeObjs != null)
        {
            for(int i = 0; i < m_nodeObjs.Count; i++)
            {
                Destroy(m_nodeObjs[i]);
            }
        }

        m_nodes = new List<Node>();
        m_nodeObjs = new List<GameObject>();
        world = MyWorld.Instance;
        PerlinNoise perliNoise = PerlinNoise.Instance;
        TerrainData terrainData = perliNoise.GetTerrainData();
        int scale = perliNoise.WorldSize;

        for(int x = 0; x < scale; x++)
        {
            for (int z = 0; z < scale; z++)
            {
                for (int y = 0; y < scale; y++)
                {
                    if (world.world[x, y, z] == 1)
                    {
                        CreateNode(new Vector3(x,y,z));
                    }
                }
            }
        }
        

        for (int i = 0; i < m_nodes.Count; i++)
        {
            m_nodes[i].ConfigSurroundingNodes();
        }
        AiManager aiManager = AiManager.Instance;
        List<PathFinding> creepersInGame = aiManager.GetCreepersInGame();
        for(int i = 0; i < creepersInGame.Count; i++)
        {
            creepersInGame[i].RecalculatePath();
        }
    }
    public float GetDistanceBetween(Node a, Node b)
    {
        return (b.transform.position - a.transform.position).magnitude;
    }
    public List<Node> GetNodesInWorld()
    {
        return m_nodes;
    }
    public void CreateNode(Vector3 position)
    {
        GameObject nodeObj = Instantiate(nodePref, position, Quaternion.identity);
        Node node = nodeObj.GetComponent<Node>();
        m_nodes.Add(node);
        m_nodeObjs.Add(nodeObj);
    }
    public Node GetNodeAtPosition(Vector3 position)
    {
        Node node = null;
        for (int i = 0; i < m_nodes.Count; i++)
        {
            if ((m_nodes[i].transform.position - position).magnitude < 0.1f)
            {
                node = m_nodes[i];
            }
        }
        return node;
    }
}
