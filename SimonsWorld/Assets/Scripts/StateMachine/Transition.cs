using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition : MonoBehaviour
{
    private State from;
    private State to;
    
    public Transition(State _from, State _to)
    {
        from = _from;
        to = _to;
    }
    public bool ContainsState(State state)
    {
        if(from.GetType() == state.GetType() || to.GetType() == state.GetType())
        {
            return true;
        }
        return false;
    }
}
