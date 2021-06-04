using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderState : State
{
    private float moveSpeed;
    private PathFinding pathfinding;
    private CreeperAi creeperAi;
    public WanderState(float _moveSpeed, PathFinding _pathFinding, CreeperAi _creeperAi)
    {
        creeperAi = _creeperAi;
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
        if (pathfinding.GetDestination() == null)
        {
            pathfinding.SetDestination(RandomDestination());
        }
            
        if((pathfinding.transform.position - pathfinding.GetDestination().transform.position).magnitude < 0.2f)
        {
            creeperAi.SetDestinationReached(true);
        }
    }
    public override void EndState()
    {
        pathfinding.SetDestination(null);
        pathfinding.SetMoveSpeed(0);
    }
    private GameObject RandomDestination()
    {
        MyNodeManager nodeManager = MyNodeManager.Instance;
        List<Node> worldNodes = nodeManager.GetNodesInWorld();
        int randomIndex = Random.Range(0, worldNodes.Count - 1);
        return worldNodes[randomIndex].gameObject;
    }
}
