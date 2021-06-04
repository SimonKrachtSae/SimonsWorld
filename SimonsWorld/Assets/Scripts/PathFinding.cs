using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    private GameObject targetObject;
    private Node targetNode;
    private Node current;

    private List<Node> open;
    private List<Node> closed;
    private List<Node> Path;

    private NodeManager myNodeManager;

    private int pathCounter = 0;
    private float moveSpeed;
    private bool recalulate;

    private void Start()
    {
        PathfindingManager.Instance.Subscribe(this);
        myNodeManager = NodeManager.Instance;
    }

    private void Update()
    {
        if (targetObject == null)
            return;
        Node closestToTarget = GetClosestNodeToTarget();
        if(closestToTarget != targetNode || targetNode == null || recalulate)
        {
            recalulate = false;
            RefreshLists();
            pathCounter = 0;
            targetNode = closestToTarget;
            AStar();
            RefreshNodes();
        }

        if (Path.Count == 0)
            return;

        RotateTowards(transform, Path[pathCounter].transform, 2);

        transform.position += TargetDirection() * Time.deltaTime * moveSpeed;

        if ((Path[pathCounter].transform.position - targetNode.transform.position).magnitude < 0.1f)
            return;

        if((Path[pathCounter].transform.position - transform.position).magnitude < 0.2f)
        {
            pathCounter++;
        }
    }
    private void RefreshNodes()
    {
        List<Node> nodesInWorld = myNodeManager.GetNodesInWorld();
        for (int i = 0; i < nodesInWorld.Count; i++)
        {
            nodesInWorld[i].nodeColor = Color.yellow;
        }
        for (int i = 0; i < nodesInWorld.Count; i++)
        {
            Node node = nodesInWorld[i];
            node.SetG_Cost(10000);
            node.SetH_Cost(0);
        }
    }
    private void RefreshLists()
    {
        open = new List<Node>();
        closed = new List<Node>();
        Path = new List<Node>();
    }
    private void AStar()
    {
        Node startNode = ClosestNode();
        startNode.SetG_Cost(0);
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
                BackTrace(startNode);
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
        if (current.surroundingNodes == null || current.surroundingNodes.Count == 0)
            return;

        for( int i = 0; i < current.surroundingNodes.Count; i++)
        {
            if (!closed.Contains(current.surroundingNodes[i]))
            {
                SetPrevious(current.surroundingNodes[i]);

                current.surroundingNodes[i].SetH_CostRelativeTo(targetNode);
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
        if (node.GetPotentialG_Cost(current) < node.GetG_Cost())
        {
           node.SetPrevious(current);
           node.SetG_CostRealtiveTo(current);
        }
    }
    public Node ClosestNode()
    {
        float closestDistance = Mathf.Infinity;
        Node closestNode = null;
        List<Node> nodesInWorld = myNodeManager.GetNodesInWorld();
        for (int i = 0; i < nodesInWorld.Count; i++)
        {
            float distance = (nodesInWorld[i].transform.position - transform.position).magnitude;
            if (distance < closestDistance)
            {
                closestNode = nodesInWorld[i];
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
    private void BackTrace(Node startNode)
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
            lastNode = lastNode.GetPrevious();

        }
        Path.Add(startNode);
    }
    public void SetMoveSpeed(float _moveSpeed)
    {
        moveSpeed = _moveSpeed;
    }
    public void SetDestination(GameObject target)
    {
        targetObject = target;
    }
    public GameObject GetDestination()
    {
        return targetObject;
    }
    private Node GetClosestNodeToTarget()
    {
        float closestDistance = 10000f;
        Node closestNode = null;
        List<Node> nodesInWorld = myNodeManager.GetNodesInWorld();
        for (int i = 0; i < nodesInWorld.Count; i++)
        {
            float distance = (nodesInWorld[i].transform.position - targetObject.transform.position).magnitude;
            if (distance < closestDistance)
            {
                closestNode = nodesInWorld[i];
                closestDistance = distance;
            }
        }
        return closestNode;
    }
    private float YTargetDistance()
    {
        return Path[pathCounter].transform.position.y - transform.position.y;
    }
    private Vector3 TargetDirection()
    {
        Vector3 direction = Vector3.zero;

        if (YTargetDistance() > 0.1f)
        {
            direction = Vector3.up;
        }
        else if(XZTargetDirection().magnitude >= 0.1f)
        {
            direction = XZTargetDirection().normalized;
        }
        else if (YTargetDistance() < -0.1f)
        {
            direction = Vector3.down;

        }
        return direction;
    }
    private Vector3 XZTargetDirection()
    {
        return new Vector3(Path[pathCounter].transform.position.x, 0, Path[pathCounter].transform.position.z) - new Vector3(transform.position.x, 0, transform.position.z);
    }
    private protected void RotateTowards(Transform transform, Transform target, float speed)
    {
        Vector3 targetDirection = new Vector3(target.position.x - transform.position.x,0, target.position.z - transform.position.z);
        float singleStep = speed * Time.deltaTime;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
        Debug.DrawRay(transform.position, newDirection, Color.red);
        transform.rotation = Quaternion.LookRotation(newDirection);
    }
    public void RecalculatePath()
    {
        recalulate = true;
    }

    private void OnDestroy()
    {
        PathfindingManager.Instance.UnSubscribe(this);
    }
}
