using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State 
{
    public abstract void StartState();
    public abstract void RunState();
    public abstract void EndState();
}
