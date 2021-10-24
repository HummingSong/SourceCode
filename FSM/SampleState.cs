using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SampleState : State<BaseEntity>
{
    private static SampleState instance;

    private SampleState()
    {
        if(instance != null)
        {
            return;
        }

        instance = this;
    }

    public static SampleState Instance
    {
        get
        {
            if (instance == null)
            {
                new SampleState();
            }

            return instance;
        }
    }

    public override void EnterState(BaseEntity _owner)
    {
        Debug.Log("EnterState");
    }

    public override void ExitState(BaseEntity _owner)
    {
        Debug.Log("ExitState");
    }

    public override void UpdateState(BaseEntity _owner)
    {
        Debug.Log("UpdateState");
    }
}

public class IdleState : State<BaseEntity>
{
    private static IdleState instance;

    private IdleState()
    {
        if (instance != null)
        {
            return;
        }

        instance = this;
    }

    public static IdleState Instance
    {
        get
        {
            if (instance == null)
            {
                new IdleState();
            }

            return instance;
        }
    }

    public override void EnterState(BaseEntity _owner)
    {
        Debug.Log("EnterState");
    }

    public override void ExitState(BaseEntity _owner)
    {
        Debug.Log("ExitState");
    }

    public override void UpdateState(BaseEntity _owner)
    {
        Debug.Log("UpdateState");
    }
}
