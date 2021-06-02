using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowState : State
{
    private GameObject targetObject;
    private float moveSpeed;
    private PathFinding pathfinding;

    public FollowState(float _moveSpeed, GameObject _targetObject, PathFinding _pathFinding)
    {
        pathfinding = _pathFinding;
        targetObject = _targetObject;
        moveSpeed = _moveSpeed;
    }
    public override void StartState()
    {
        pathfinding.SetDestination(targetObject);
        pathfinding.SetMoveSpeed(moveSpeed);
    }
    public override void RunState()
    {
    }
    public override void EndState()
    {
        pathfinding.SetMoveSpeed(0);
        pathfinding.SetDestination(null);
    }
}
