using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public struct ObjectInfo
{
    public string KeyName;
    public GameObject Prefab;
    public GameObject OriginParent;
    public int Count;
}

public class PSObjectPoolManager : MonoBehaviour
{
    public static PSObjectPoolManager instance = null;

    [HideInInspector]
    public PSObjectCategory[] PoolCategoies;

    public ObjectInfo[] PoolTable;

    private Dictionary<string, PSObjectPool> Pools = new Dictionary<string, PSObjectPool>();

    private Dictionary<string, List<GameObject>> ActiveObjects = new Dictionary<string, List<GameObject>>();


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        PoolCategoies = GetComponentsInChildren<PSObjectCategory>();

        int maxCount = 0;
        for (int i = 0; i < PoolCategoies.Length; ++i)
        {
            maxCount += PoolCategoies[i].GetTableCount();
        }

        if (PoolCategoies.Length > 0)
        {
            PoolTable = null;
            PoolTable = new ObjectInfo[maxCount];

            int index = 0;
            for (int i = 0; i < PoolCategoies.Length; ++i)
            {
                Array.Copy(PoolCategoies[i].PoolTable, 0, PoolTable, index, PoolCategoies[i].PoolTable.Length);
                index += PoolCategoies[i].PoolTable.Length;
            }
        }
    }

    public void MakeObjectPool()
    {
        for (int i = 0; i < PoolTable.Length; ++i)
        {
            if (PoolTable[i].Prefab == null)
                continue;

            PSObjectPool pool = null;
            if (PoolTable[i].OriginParent != null)
                pool = new PSObjectPool(PoolTable[i].KeyName, PoolTable[i].Prefab, PoolTable[i].Count, PoolTable[i].OriginParent);
            else
                pool = new PSObjectPool(PoolTable[i].KeyName, PoolTable[i].Prefab, PoolTable[i].Count, this.gameObject);

            Pools.Add(PoolTable[i].KeyName, pool);
        }
    }

    public GameObject SpawnObject(string key, Vector3 pos, Quaternion qt, Transform parent = null, int level = 1, float scale = 1, bool setScale = false)
    {
        PSObjectPool pool = null;

        Pools.TryGetValue(key, out pool);

        if (pool == null)
            return null;

        GameObject obj = pool.SpawnObject(pos, qt, parent, level);

        if (setScale)
            obj.transform.localScale = new Vector3(scale, scale, 1);

        if (!ActiveObjects.ContainsKey(key))
        {
            ActiveObjects[key] = new List<GameObject>();
        }

        ActiveObjects[key].Add(obj);

        return obj;
    }
    
    public void SaveObject(string key, GameObject obj, bool saveAll = false)
    {
        PSObjectPool pool = null;
        Pools.TryGetValue(key, out pool);

        if (!saveAll)
        {
            if (ActiveObjects.ContainsKey(key))
            {
                ActiveObjects[key].Remove(obj);
            }
        }

        pool.SaveObject(obj);
    }

    public void SaveAllPools()
    {
        foreach(KeyValuePair<string, List<GameObject>> item in ActiveObjects)
        {
            Debug.Log(item.Value.Count);
            for(int i = 0; i < item.Value.Count; ++i)
            {
                SaveObject(item.Key, item.Value[i], true);
            }

            item.Value.Clear();
        }

        ActiveObjects.Clear();
    }

    public bool isInPool(string key)
    {
        return Pools.ContainsKey(key);
    }
}
