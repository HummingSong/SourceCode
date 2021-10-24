using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T>
{
	public State<T> CurrentSstate { get; private set; }
    public State<T> PreviousSstate { get; private set; }
    public State<T> GlobalSstate { get; private set; }
    public T Owner;

    public StateMachine(T _owner)
    {
        Owner = _owner;
        CurrentSstate = null;
    }

    public void ChangeState(State<T> nextState)
    {
        if(CurrentSstate != null)
            CurrentSstate.ExitState(Owner);

        CurrentSstate = nextState;
        CurrentSstate.EnterState(Owner);
    }

    public void Update()
    {
        if (CurrentSstate != null)
            CurrentSstate.UpdateState(Owner);
    }
}

public abstract class State<T>
{
    public abstract void EnterState(T _owner);
    public abstract void ExitState(T _owner);
    public abstract void UpdateState(T _owner);
}
