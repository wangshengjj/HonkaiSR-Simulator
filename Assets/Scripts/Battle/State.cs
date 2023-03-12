using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State: ACountDownBehaviour
{
    public StateType state { get; protected set; }
    public State(StateType _s, int times): base("state", CountDownType.Turn, times, int.MaxValue)
    {
        state = _s;
    }
}
