using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public List<Node> surroundingNodes = new List<Node>();

    private float H_Cost;
    private float G_Cost;
    public float F_Cost { get => H_Cost + G_Cost; }

    private Node Previous;
    public Color nodeColor = Color.yellow;
    private void Awake()
    {
        SetG_Cost(10000); 
    }
    public void ConfigSurroundingNodes()
    {
        NodeManager myNodeManager = NodeManager.Instance;
        List<Node> nodesInWorld = myNodeManager.GetNodesInWorld();
        for (int i = 0; i < nodesInWorld.Count; i++)
        {
            Node node = nodesInWorld[i];
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
            nodesInWorld.Remove(this);
            Destroy(this);
        }
        
    }

    public void SetPrevious(Node node)
    {
        Previous = node;
    }
    public Node GetPrevious()
    {
        return Previous;
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
            surroundingNodes[i].SetH_CostRelativeTo(targetNode);
            surroundingNodes[i].SetG_CostRealtiveTo(Previous);
        }
    }
    public void SetH_CostRelativeTo(Node targetNode)
    {
        float value = (targetNode.transform.position - transform.position).magnitude;
        H_Cost = value;
    }
    public void SetG_CostRealtiveTo(Node node)
    {
        
        float value = (node.transform.position - transform.position).magnitude;
        value += node.G_Cost;
        
        G_Cost = value;
    }
    public void SetG_Cost(float value)
    {
        G_Cost = value;
    }
    public void SetH_Cost(float value)
    {
        H_Cost = value;
    }
    public float GetG_Cost()
    {
        return G_Cost;
    }
    public float GetHCost()
    {
        return H_Cost;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = nodeColor;
        Gizmos.DrawSphere(transform.position, 0.1f);
    }
}
