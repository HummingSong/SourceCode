using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSUIObject : MonoBehaviour
{
    protected string key;
    private Transform originParentTr;

    public void SetOriginParent(Transform parent)
    {
        originParentTr = transform.parent;
    }

    public void SetKey(string name)
    {
        key = name;
    }

    // 이건 어차피 다시 재 상속 받아야 한다. 오브젝트 풀 속성과 상태머신 등등등
    public virtual void Spawn(Transform parent = null)
    {
        if (parent != null)
            transform.SetParent(parent);
    }

    public void Save()
    {
        transform.SetParent(originParentTr);
    }
}
