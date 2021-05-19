using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public bool visited;
    public bool NodeIsInWater;
    public List<Node> surroundingNodes = new List<Node>();
    private int m_WorldSizeX;
    private int m_WorldSizeY;
    private int m_WorldSizeZ;

    public Vector3 position;

    public float h_Cost;
    public float G_Cost;
    public float F_Cost { get => h_Cost + G_Cost; }
    public void NodeStart()
    {
        position = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), Mathf.RoundToInt(transform.position.z));
        m_WorldSizeX = MyCellularWorld.Instance.m_WorldSizeX;
        m_WorldSizeY = MyCellularWorld.Instance.m_WorldSizeY;
        m_WorldSizeZ = MyCellularWorld.Instance.m_WorldSizeZ;
    }
    public void ConfigSurroundingNodes()
    {
        position = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), Mathf.RoundToInt(transform.position.z));
        for (int _x = -1; _x <= 1; _x++)
        {
            for (int _y = -1; _y <= 1; _y++)
            {
                for (int _z = -1; _z <= 1; _z++)
                {
                    if (!(_x == 0 && _y == 0 && _z == 0))
                    {
                        for (int i = 0; i < NodeManager.Instance.m_nodes.Count; i++)
                        {
                            Node node = NodeManager.Instance.m_nodes[i];
                            if (node.position == position + new Vector3(_x, _y, _z))
                            {
                                surroundingNodes.Add(node);
                            }
                        }
                    }
                }
            }
        }
        
    }
    public void SetH_Cost()
    {
        float value = (NodeManager.Instance.TargetNode.position - position).magnitude;
        if(NodeIsInWater)
        {
            h_Cost = value * 2;
        }
        else
        {
            h_Cost = value;
        }
    }
    public void SetG_Cost(Node node)
    {
        float value = (node.position - position).magnitude;
        
        if (NodeIsInWater)
        {
            G_Cost = value * 2;
        }
        else
        {
            G_Cost = value;
        }
    }

    public void SetSurroundingNodesCosts()
    {
        for(int i = 0; i < surroundingNodes.Count; i++)
        {
            surroundingNodes[i].SetH_Cost();
            surroundingNodes[i].SetG_Cost(surroundingNodes[i]);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.1f);
    }
}
