using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderState : State
{
    private float moveSpeed;
    private PathFinding pathfinding;
    public WanderState(float _moveSpeed, PathFinding _pathFinding)
    {
        pathfinding = _pathFinding;
        moveSpeed = _moveSpeed;
    }
    public override void StartState()
    {
        pathfinding.SetDestination(RandomDestination());
        pathfinding.SetMoveSpeed(moveSpeed);
    }
    public override void RunState()
    {
        if((pathfinding.transform.position - pathfinding.GetDestination().transform.position).magnitude < 0.2f)
        {
            pathfinding.SetDestination(RandomDestination()); 
        }
    }
    public override void EndState()
    {
        //pathfinding.SetDestination(this.gameObject);
        pathfinding.SetMoveSpeed(0);
    }
    private GameObject RandomDestination()
    {
        List<Node> worldNodes = MyNodeManager.Instance.GetNodesInWorld();
        int randomIndex = Random.Range(0, worldNodes.Count - 1);
        return worldNodes[randomIndex].gameObject;
    }
}
