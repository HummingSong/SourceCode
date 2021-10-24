using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSObjectCategory : MonoBehaviour
{
    public ObjectInfo[] PoolTable;

    public int GetTableCount()
    {
        return PoolTable.Length;
    }
}
