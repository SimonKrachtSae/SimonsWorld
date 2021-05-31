using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public bool Closed = false;
    public List<Node> surroundingNodes = new List<Node>();

    public float H_Cost;
    public float G_Cost = 10000;
    public float F_Cost { get => H_Cost + G_Cost; }

    public Node Previous;
    public Color nodeColor = Color.yellow;

    public bool isWaterNode;
    private void Awake()
    {
        G_Cost = 10000; 
    }
    private void Start()
    {
    }
    public void ConfigSurroundingNodes()
    {
        MyNodeManager myNodeManager = MyNodeManager.Instance;
        for (int i = 0; i < myNodeManager.m_nodes.Count; i++)
        {
            Node node = myNodeManager.m_nodes[i];
            if (node != this )
            {
                if((node.transform.position - this.transform.position).magnitude < 1.8f)
                {
                    surroundingNodes.Add(node);
                }
            }
        }
        if(surroundingNodes == null)
        {
            myNodeManager.m_nodes.Remove(this);
            Destroy(this);
        }
        
    }

    public void SetH_Cost(Node targetNode)
    {
        float value = (targetNode.transform.position - transform.position).magnitude;
        H_Cost = value;
    }
    public void SetG_Cost(Node node)
    {
        
        float value = (node.transform.position - transform.position).magnitude;
        value += node.G_Cost;
        
        G_Cost = value;
    }
    public void SetPrevious(Node node)
    {
        Previous = node;
    }
   
    public float GetPotentialG_Cost(Node potPrevious)
    {
        float value;
        value = (potPrevious.transform.position - transform.position).magnitude;
        value += potPrevious.G_Cost;
        return value;
    }

    public void SetSurroundingNodesCosts(Node targetNode)
    {
        for(int i = 0; i < surroundingNodes.Count; i++)
        {
            surroundingNodes[i].SetH_Cost(targetNode);
            surroundingNodes[i].SetG_Cost(Previous);
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
