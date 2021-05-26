using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyNavAgent : MonoBehaviour
{
    public List<Node> open;
    public List<Node> closed;
    public List<Node> Path;

    public Node startNode;
    public Node targetNode;

    public TargetObject targetObject;

    private Node current;

    int index = 0;

    private void FixedUpdate()
    {
        if (targetObject == null)
            return;

        targetNode = targetObject.ClosestNode();


        if (targetObject.ClosestNode().surroundingNodes.Count == 0)
            return;

        for(int i = 0; i < NodeManager.Instance.m_nodes.Count; i++)
        {
            NodeManager.Instance.m_nodes[i].nodeColor = Color.yellow;
        }

        AStar();

       if (Path.Count == 0)
           return;
              
       Vector3 direction = (ClosestPathNode().transform.position - transform.position).normalized;
       
       transform.position += direction * Time.deltaTime;
    }
    private void AStar()
    {
        open = new List<Node>();
        closed = new List<Node>();
        startNode = ClosestNode();
        Path = new List<Node>();

        startNode.SetG_Cost();
        startNode.SetH_Cost(targetNode);
        current = startNode;
        current.Previous = current;
        SetPreviousNode();

        open.Add(current);

        SetNextCurrent();
    }
    private void SetNextCurrent()
    {
        while(current != targetNode && current != null)
        {
            Node nextCurrentNode = null;
            float lowestF_Cost = Mathf.Infinity;
            current.SetSurroundingNodesCosts(targetNode);
            for (int j = 0; j < current.surroundingNodes.Count; j++)
            {
                AddToOpen(current.surroundingNodes[j]);
                if(current.surroundingNodes[j].Previous == null)
                {
                    current.surroundingNodes[j].Previous = current;
                    current.surroundingNodes[j].SetG_Cost();
                    current.surroundingNodes[j].SetH_Cost(targetNode);
                }
                
                if(current.surroundingNodes[j].GetPotentialG_Cost(current) < current.surroundingNodes[j].G_Cost)
                {
                    current.surroundingNodes[j].Previous = current;
                    current.surroundingNodes[j].SetG_Cost();
                    current.surroundingNodes[j].SetH_Cost(targetNode);
                }
            }
            for (int i = 0; i < open.Count; i++)
            {
                     
                    if(open[i].F_Cost < lowestF_Cost)
                    {
                        lowestF_Cost = open[i].F_Cost;
                        nextCurrentNode = open[i];
                    }
            }
            if(!closed.Contains(current))
            {
                current.nodeColor = Color.red;
                closed.Add(current);
            }
            open.Remove(current);
            current = nextCurrentNode;
        }
        closed.Add(targetNode);
        BackTrace();
        RewindPath();
    }
    private void RewindPath()
    {
        List<Node> rewindedPath = new List<Node>();
        for(int i = Path.Count -1; i >= 0; i--)
        {
            rewindedPath.Add(Path[i]);
        }
        
        Path = rewindedPath;
    }
    private void BackTrace()
    {
        Node lastNode = targetNode;
        while(lastNode != startNode)
        {
            lastNode.nodeColor = Color.blue;
            Path.Add(lastNode);
            lastNode = lastNode.Previous;
        }
    }
    private void AddToOpen(Node node)
    {
        if (!open.Contains(node) && !closed.Contains(node))
        {
            open.Add(node);
            node.nodeColor = Color.green;
        }
    }

    private void SetPreviousNode()
    {
        for (int i = 0; i < current.surroundingNodes.Count; i++)
        {
            current.surroundingNodes[i].Previous = current;
            current.surroundingNodes[i].SetG_Cost();
            current.surroundingNodes[i].SetH_Cost(targetNode);
        }
    }
    private Node ClosestNode()
    {
        float closestDistance = Mathf.Infinity;
        Node closestNode = null;
        for (int i = 0; i < NodeManager.Instance.m_nodes.Count; i++)
        {
            float distance = (NodeManager.Instance.m_nodes[i].transform.position - transform.position).magnitude;
            if (distance < closestDistance)
            {
                closestNode = NodeManager.Instance.m_nodes[i];
                closestDistance = distance;
            }
        }
        return closestNode;
    }
    private Node ClosestPathNode()
    {
        float closestDistance = Mathf.Infinity;
        Node closestNode = null;
        for(int i = 0; i < Path.Count; i++)
        {
            float distance = (Path[i].transform.position - transform.position).magnitude;
            if (distance < closestDistance)
            {
                if(i < Path.Count -1)
                {
                    closestNode = Path[i + 1];
                }
                else
                {
                    closestNode = Path[i];
                }
                closestDistance = distance;
            }
        }
        return closestNode;
    }
}