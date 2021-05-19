using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyNavAgent : MonoBehaviour
{
    public Node closestNode;
    public Node ActiveNode;
    public Node nextNode;

    public bool Start;
    public bool justStarting = true;
    public void StartAgent()
    {
        GetClosestNode();
        closestNode.SetH_Cost();
        closestNode.SetSurroundingNodesCosts();
        ActiveNode = closestNode;
        ConfiguerNextNode();
    }

    private void GetClosestNode()
    {
        float closestDistance = 10000f;
        closestNode = null;
        for (int i = 0; i < NodeManager.Instance.m_nodes.Count; i++)
        {
            float distance = (NodeManager.Instance.m_nodes[i].position - transform.position).magnitude;
            if (distance < closestDistance)
            {
                closestNode = NodeManager.Instance.m_nodes[i];
                closestDistance = distance;
            }
        }
    }
    private void FixedUpdate()
    {
        
        if(Start)
        {
            if(justStarting)
            {
                StartAgent();
                justStarting = false;
            }
            if((nextNode.transform.position - transform.position).magnitude < 0.1f)
            {
                ActiveNode.visited = true;
                ActiveNode = nextNode;
                ConfiguerNextNode();
            }
            transform.position += (nextNode.position - transform.position).normalized * Time.deltaTime;
            //if ((ActiveNode.transform.position - NodeManager.Instance.TargetNode.position).magnitude > 0.1f)
            //{
            //}
        }
    }
    private void ConfiguerNextNode()
    {
        float lowestF_Cost = 10000;
        Node next = null;
        for (int i = 0; i < ActiveNode.surroundingNodes.Count; i++)
        {
            if(ActiveNode.surroundingNodes[i].F_Cost < lowestF_Cost)
            {
                lowestF_Cost = ActiveNode.surroundingNodes[i].h_Cost;
                next = ActiveNode.surroundingNodes[i];
            }
        }
        nextNode = next;
        nextNode.SetSurroundingNodesCosts();
    }
}
