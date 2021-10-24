using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSUIObjectPoolManager : MonoBehaviour
{
    public static PSUIObjectPoolManager instance = null;

    [System.Serializable]
    public struct ObjectInfo
    {
        public string KeyName;
        public GameObject Prefab;
        public GameObject Parent;
        public int Count;
    }

    public ObjectInfo[] PoolTable;

    private Dictionary<string, PSUIObjectPool> Pools = new Dictionary<string, PSUIObjectPool>();

    private Dictionary<string, List<GameObject>> ActiveObjects = new Dictionary<string, List<GameObject>>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void MakeObjectPool()
    {
        for (int i = 0; i < PoolTable.Length; ++i)
        {
            PSUIObjectPool pool = null;
            if (PoolTable[i].Parent != null)
                pool = new PSUIObjectPool(PoolTable[i].KeyName, PoolTable[i].Prefab, PoolTable[i].Count, PoolTable[i].Parent);
            else
                pool = new PSUIObjectPool(PoolTable[i].KeyName, PoolTable[i].Prefab, PoolTable[i].Count, this.gameObject);

            Pools.Add(PoolTable[i].KeyName, pool);
        }
    }

    public GameObject SpawnObject(string key, Vector3 pos, Quaternion qt, Transform parent = null)
    {
        PSUIObjectPool pool = null;
        Pools.TryGetValue(key, out pool);

        GameObject obj = pool.SpawnObject(pos, qt, parent);

        if (!ActiveObjects.ContainsKey(key))
        {
            ActiveObjects[key] = new List<GameObject>();
        }

        ActiveObjects[key].Add(obj);

        return obj;
    }

    public void SaveObject(string key, GameObject obj, bool saveAll = false)
    {
        PSUIObjectPool pool = null;
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
        foreach (KeyValuePair<string, List<GameObject>> item in ActiveObjects)
        {
            Debug.Log(item.Value.Count);
            for (int i = 0; i < item.Value.Count; ++i)
            {
                SaveObject(item.Key, item.Value[i], true);
            }

            item.Value.Clear();
        }

        ActiveObjects.Clear();
    }
}
