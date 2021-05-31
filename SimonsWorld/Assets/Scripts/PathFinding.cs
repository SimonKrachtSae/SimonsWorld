using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    public List<Node> open;
    public List<Node> closed;
    public List<Node> Path;

    public Node targetNode;

    public Node startNode;
    public Node current;

    public TargetObject targetObject;

    private void FixedUpdate()
    {
        if(targetObject.ClosestNode() != targetNode || targetNode == null)
        {
            targetNode = targetObject.ClosestNode();

            for (int i = 0; i < NodeManager.Instance.m_nodes.Count; i++)
            {
                NodeManager.Instance.m_nodes[i].nodeColor = Color.yellow;
            }

            RefreshLists();
            AStar();
        }
        if (Path.Count == 0)
            return;
        Vector3 direction = Vector3.zero;
        Node closestPathNode = ClosestPathNode();
        if (Mathf.Abs(transform.position.y - closestPathNode.transform.position.y) > 0.1f)
        {
            direction = new Vector3(0, closestPathNode.transform.position.y - transform.position.y,0).normalized;
        }
        else
        {
            direction = (closestPathNode.transform.position - transform.position).normalized;
        }

        transform.position += direction * Time.deltaTime;
    }
    private void AStar()
    {
        startNode = ClosestNode();
        startNode.G_Cost = 0;
        current = startNode;
        int counter = 1000;
        while(true)
        {
            counter--;
            if (counter <= 1)
                break;

            SetSurroundingNodesCostsAndPreviousNode();
            SetNextNodeToInspect();
            if (current == targetNode)
            {
                closed.Add(targetNode);
                BackTrace();
                RewindPath();

                break;
            }
        }
    }
    private void SetNextNodeToInspect()
    {
        float lowestF_Cost = Mathf.Infinity;
        Node nextNode = null;
        for(int i = 0; i < open.Count; i++)
        {
            if(open[i].F_Cost < lowestF_Cost)
            {
                lowestF_Cost = open[i].F_Cost;
                nextNode = open[i];
            }
        }

        AssignNewCurrent(nextNode);
    }
    private void AssignNewCurrent(Node nextCurrent)
    {
        if (open.Contains(current))
            open.Remove(current);

        if (!closed.Contains(current))
        {
            closed.Add(current);
            current.nodeColor = Color.red;
        }
        current = nextCurrent;
    }
    private void SetSurroundingNodesCostsAndPreviousNode()
    {
        for( int i = 0; i < current.surroundingNodes.Count; i++)
        {
            if (!closed.Contains(current.surroundingNodes[i]))
            {
                SetPrevious(current.surroundingNodes[i]);

                current.surroundingNodes[i].SetH_Cost(targetNode);
                AddToOpen(current.surroundingNodes[i]);
            }
        }
    }
    private void AddToOpen(Node node)
    {
        if (open.Contains(node))
            return;

        node.nodeColor = Color.green;

        open.Add(node);
    }
    private void SetPrevious(Node node)
    {

           node.Previous = current;
           node.SetG_Cost(current);
        
    }
    private void RefreshLists()
    {
        open = new List<Node>();
        closed = new List<Node>();
        Path = new List<Node>();
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
    private void RewindPath()
    {
        List<Node> rewindedPath = new List<Node>();
        for (int i = Path.Count - 1; i >= 0; i--)
        {
            rewindedPath.Add(Path[i]);
        }

        Path = rewindedPath;
    }
    private void BackTrace()
    {
        Node lastNode = targetNode;
        int counter = 1000;
        while (lastNode != startNode)
        {
            if (counter <= 1)
               break;
            counter--;
            lastNode.nodeColor = Color.blue;
            Path.Add(lastNode);
            lastNode = lastNode.Previous;
        }
    }
    private Node ClosestPathNode()
    {
        float closestDistance = Mathf.Infinity;
        Node closestNode = null;
        for (int i = 0; i < Path.Count; i++)
        {
            float distance = (Path[i].transform.position - transform.position).magnitude;
            if (distance < closestDistance)
            {
                if (i < Path.Count - 1)
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
