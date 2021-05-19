using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    public static NodeManager Instance;
    public GameObject Node;
    public List<Node> m_nodes = new List<Node>();

    public TargetObject TargetObject;
    public Node TargetNode;

    public MyNavAgent navAgent;
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
        int m_WorldSizeX = MyCellularWorld.Instance.m_WorldSizeX;
        int m_WorldSizeY = MyCellularWorld.Instance.m_WorldSizeY;
        int m_WorldSizeZ = MyCellularWorld.Instance.m_WorldSizeZ;

        int[,,] Nodes = new int[m_WorldSizeX, m_WorldSizeY, m_WorldSizeZ];

        for (int x = 0; x < m_WorldSizeX; x++)
        {
            for (int y = 0; y <= m_WorldSizeY; y++)
            {
                for (int z = 0; z < m_WorldSizeZ; z++)
                {
                    if (y < m_WorldSizeY)
                    {
                        if(MyCellularWorld.Instance.M_World[x, y, z] == 1 || MyCellularWorld.Instance.M_World[x, y, z] == 3)
                        {
                            if (MyCellularWorld.Instance.M_World[x, y - 1, z] != 1)
                            {
                                if(!(y + 1 >= m_WorldSizeY))
                                {
                                    if(MyCellularWorld.Instance.M_World[x,y+1,z] != 0)
                                    {
                                        GameObject nodeObj = Instantiate(Node, new Vector3(x, y, z), Quaternion.identity);
                                        Node node = nodeObj.GetComponent<Node>();
                                        m_nodes.Add(node);
                                    }
                                }
                                else if(y + 1 >= m_WorldSizeY)
                                {
                                    GameObject node = Instantiate(Node, new Vector3(x, y, z), Quaternion.identity);
                                    m_nodes.Add(node.GetComponent<Node>());
                                }
                            }
                        }
                    }
                    if (y == m_WorldSizeY)
                    {
                        if (MyCellularWorld.Instance.M_World[x, y - 1, z] == 0 || MyCellularWorld.Instance.M_World[x, y - 1, z] == 2)
                        {
                            GameObject node = Instantiate(Node, new Vector3(x, y, z), Quaternion.identity);
                            m_nodes.Add(node.GetComponent<Node>());
                        }
                    }
                }
            }
        }


        SetWaterNodes(m_WorldSizeX,m_WorldSizeY,m_WorldSizeZ);

        for (int j = 0; j < m_nodes.Count; j++)
        {
            m_nodes[j].NodeStart();
        }

        for (int i = 0; i < m_nodes.Count; i++)
        {
            m_nodes[i].ConfigSurroundingNodes();
        }
        TargetNode = m_nodes[1];
    }
    void SetWaterNodes(int worldX, int worldY, int worldZ)
    {
        for (int x = 0; x < worldX; x++)
        {
            for (int y = 0; y < worldY; y++)
            {
                for (int z = 0; z < worldZ; z++)
                {
                    if(MyCellularWorld.Instance.M_World[x,y,z] == 1)
                    {
                        for(int i = 0; i <  m_nodes.Count; i++)
                        {
                            if (m_nodes[i].position == MyCellularWorld.Instance.m_Cubes[x, y, z].Position)
                            {
                                m_nodes[i].NodeIsInWater = true;
                            }
                        }
                    }
                }
            }
        }
    }
    void FixedUpdate()
    {
        if(TargetObject.ClosestNode() != null)
        {
            if(TargetObject.ClosestNode() != TargetNode)
            {
                TargetNode = TargetObject.ClosestNode();
            }
        }
    }
}
