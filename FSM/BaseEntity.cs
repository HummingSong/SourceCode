using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEntity : PSObject
{
    protected int id;

    [HideInInspector]
    public bool isAction = false;

    [HideInInspector]
    public bool isLive = false;

    [HideInInspector]
    public int entityLevel = 1;

    [HideInInspector]
    public int rewardExp = 0;

    public void EntityInit()
    {
        id = GetInstanceID();
    }
}
