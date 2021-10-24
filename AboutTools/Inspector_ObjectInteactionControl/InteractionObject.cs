using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionObject : MonoBehaviour
{
    public string key;

    [HideInInspector]
    public List<Interaction> InteractionList = new List<Interaction>();
}
