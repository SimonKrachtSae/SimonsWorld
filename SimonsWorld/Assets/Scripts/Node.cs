using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public bool Closed = false;
    public List<Node> surroundingNodes = new List<Node>();

    public float H_Cost;
    public float G_Cost;
    public float F_Cost { get => H_Cost + G_Cost; }

    public Node Previous;
    public Color nodeColor = Color.yellow;

    public bool isWaterNode;
    private void Start()
    {
        CheckIfNodeIsInWater();
    }
    public void ConfigSurroundingNodes()
    {
        for (int i = 0; i < NodeManager.Instance.m_nodes.Count; i++)
        {
            Node node = NodeManager.Instance.m_nodes[i];
            if (node != this )
            {
                if((node.transform.position - this.transform.position).magnitude < 1.5f)
                {
                    surroundingNodes.Add(node);
                }
            }
        }
        if(surroundingNodes == null)
        {
            NodeManager.Instance.m_nodes.Remove(this);
            Destroy(this);
        }
        //for (int _x = -1; _x <= 1; _x++)
        //{
        //    for (int _y = -1; _y <= 1; _y++)
        //    {
        //        for (int _z = -1; _z <= 1; _z++)
        //        {
        //            if (!(_x == 0 && _y == 0 && _z == 0))
        //            {
        //                for (int i = 0; i < NodeManager.Instance.m_nodes.Count; i++)
        //                {
        //                    Node node = NodeManager.Instance.m_nodes[i];
        //                    if (node.transform.position == transform.position + new Vector3(_x, _y, _z))
        //                    {
        //                        surroundingNodes.Add(node);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
        //
    }

    public void SetH_Cost(Node targetNode)
    {
        float value = (targetNode.transform.position - transform.position).magnitude;
        H_Cost = value;
    }
    public void SetG_Cost()
    {
        float value = 0;
        if(Previous == null)
        {
            G_Cost = 0;
            return;
        }

        if(isWaterNode)
        {
            value = (Previous.transform.position - transform.position).magnitude * 2;
            value += Previous.G_Cost;
            
        }
        else
        {
            value = (Previous.transform.position - transform.position).magnitude;
            value += Previous.G_Cost;
        }
        G_Cost = value;
    }
    public void SetPrevious(Node node)
    {
        Previous = node;
    }
    public float GetPotentialG_Cost(Node potPrevious)
    {
        float value = 0;
        value = (potPrevious.transform.position - transform.position).magnitude;
        value += potPrevious.G_Cost;
        return value;
    }

    public void SetSurroundingNodesCosts(Node targetNode)
    {
        for(int i = 0; i < surroundingNodes.Count; i++)
        {
            surroundingNodes[i].SetH_Cost(targetNode);
            surroundingNodes[i].SetG_Cost();
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = nodeColor;
        Gizmos.DrawSphere(transform.position, 0.1f);
    }
    private void CheckIfNodeIsInWater()
    {
        for (int x = 0; x < MyCellularWorld.Instance.m_WorldSizeX; x++)
        {
            for (int y = 0; y < MyCellularWorld.Instance.m_WorldSizeY; y++)
            {
                for (int z = 0; z < MyCellularWorld.Instance.m_WorldSizeZ; z++)
                {
                    if(transform.position == MyCellularWorld.Instance.m_Cubes[x,y,z].transform.position)
                    {
                        if(MyCellularWorld.Instance.M_World[x,y,z] == 1)
                        {
                            isWaterNode = true;
                        }
                    }
                }
            }
        }
    }
}
