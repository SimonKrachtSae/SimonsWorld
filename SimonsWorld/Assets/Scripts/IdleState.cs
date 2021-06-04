using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    private GameObject creeper;
    private CreeperAi creeperAi;
    float coolDownTime = 4;
    public IdleState(GameObject _creeper, CreeperAi _creeperAi)
    {
        creeperAi = _creeperAi;
        creeper = _creeper;
    }
    public override void StartState()
    {
    }
    public override void RunState()
    {
        Vector3 direction = Vector3.Lerp(Vector3.up, Vector3.down,1 );
        creeper.transform.Rotate(direction);
        coolDownTime -= Time.fixedDeltaTime;
        if(coolDownTime <= 0.0f)
        {
            creeperAi.SetDestinationReached(false);
            coolDownTime = 4;
        }
    }
    public override void EndState()
    {
        coolDownTime = 4;
        creeperAi.SetDestinationReached(false);
    }
}
