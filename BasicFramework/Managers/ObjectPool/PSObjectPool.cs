using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSObjectPool
{
    private string keyName;
    private GameObject Obj;
    private GameObject ParentObj;
    private Stack<GameObject> ObjectPool = null;

    public PSObjectPool(string key, GameObject obj, int count, GameObject parent, bool addCount = false)
    {
        keyName = key;
        Obj = obj;
        ParentObj = parent;

        MakeObjectPool(count);
    }

    void MakeObjectPool(int count)
    {
        GameObject obj = null;
        ObjectPool = new Stack<GameObject>(count);

        for (int i = 0; i < count; ++i)
        {
            if (Obj == null)
                continue;

            obj = Object.Instantiate(Obj, ParentObj.transform);

            obj.GetComponent<PSObject>().SetOriginParent(ParentObj.transform);
            obj.GetComponent<PSObject>().SetKey(keyName);

            obj.SetActive(false);
            obj.name = obj.GetInstanceID().ToString();

            ObjectPool.Push(obj);
        }
    }

    public GameObject SpawnObject(Vector3 pos, Quaternion qt, Transform parent = null, int level = 1)
    {
        if (ObjectPool.Count == 0)
            MakeObjectPool(1);

        GameObject obj = ObjectPool.Pop();

        obj.transform.position = pos;
        if (qt != Quaternion.identity)
            obj.transform.localRotation = qt;

        obj.SetActive(true);

        if (parent == null)
            obj.GetComponent<PSObject>().Spawn(null, level);
        else
            obj.GetComponent<PSObject>().Spawn(parent);

        return obj;
    }

    public void SaveObject(GameObject obj)
    {
        if (CheckSave(obj))
            return;

        obj.SetActive(false);

        obj.transform.position = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;

        obj.GetComponent<PSObject>().Save();

        ObjectPool.Push(obj);
    }

    public bool CheckSave(GameObject obj)
    {
        if (ObjectPool.Contains(obj))
            return true;

        return false;
    }
}
