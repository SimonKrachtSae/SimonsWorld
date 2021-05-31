﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetObject : MonoBehaviour
{
    
    void Start()
    {
        
    }
    void Update()
    {
        
    }
    public Node ClosestNode()
    {
        float closestDistance = 10000f;
        Node closestNode = null;
        for (int i = 0; i < MyNodeManager.Instance.m_nodes.Count; i++)
        {
            float distance = (MyNodeManager.Instance.m_nodes[i].transform.position - transform.position).magnitude;
            if (distance < closestDistance)
            {
                closestNode = MyNodeManager.Instance.m_nodes[i];
                closestDistance = distance;
            }
        }
        return closestNode;
    }
}
