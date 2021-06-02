using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    [SerializeField] private GameObject Player;

    private State currentState;

    private WanderState wanderState;
    private FollowState followState;


    private void Start()
    {
        PathFinding pathFinding = GetComponent<PathFinding>();
        wanderState = new WanderState(1, pathFinding);
        followState = new FollowState(2, Player,pathFinding);
        currentState = wanderState;
        currentState.StartState();
    }
    private void Update()
    {
        if(PlayerIsInRange())
        {
            if(currentState is WanderState)
            {
                SwitchCurrentState(followState);
            }
        }
        if(!PlayerIsInRange())
        {
            if(currentState is FollowState)
            {
                SwitchCurrentState(wanderState);
            }
        }
        currentState.RunState();
    }
    private void SwitchCurrentState(State nextState)
    {
        
        currentState.EndState();
        currentState = nextState;
        currentState.StartState();
    }
    private bool PlayerIsInRange()
    {
        if((Player.transform.position - transform.position).magnitude < 15)
        {
            return true;
        }
        return false;
    }
}
