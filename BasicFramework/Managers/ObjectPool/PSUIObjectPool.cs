using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSUIObjectPool
{
    private string keyName;
    private GameObject Obj;
    private GameObject ParentObj;
    private Stack<GameObject> ObjectPool = null;

    public PSUIObjectPool(string key, GameObject obj, int count, GameObject parent, bool addCount = false)
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
            obj = Object.Instantiate(Obj, ParentObj.transform);

            obj.GetComponent<PSUIObject>().SetOriginParent(ParentObj.transform);
            obj.GetComponent<PSUIObject>().SetKey(keyName);

            obj.SetActive(false);
            obj.name = obj.GetInstanceID().ToString();

            ObjectPool.Push(obj);
        }
    }

    public GameObject SpawnObject(Vector3 pos, Quaternion qt, Transform parent = null)
    {
        if (ObjectPool.Count == 0)
            MakeObjectPool(1);

        GameObject obj = ObjectPool.Pop();
        //Debug.LogError(pos);
        obj.GetComponent<RectTransform>().anchoredPosition = pos;
        if (qt != Quaternion.identity)
            obj.transform.localRotation = qt;

        obj.SetActive(true);

        if (parent == null)
            obj.GetComponent<PSUIObject>().Spawn();
        else
            obj.GetComponent<PSUIObject>().Spawn(parent);

        return obj;
    }

    public void SaveObject(GameObject obj)
    {
        if (CheckSave(obj))
            return;

        obj.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;

        obj.SetActive(false);

        obj.GetComponent<PSUIObject>().Save();

        ObjectPool.Push(obj);
    }

    public bool CheckSave(GameObject obj)
    {
        if (ObjectPool.Contains(obj))
            return true;

        return false;
    }
}
