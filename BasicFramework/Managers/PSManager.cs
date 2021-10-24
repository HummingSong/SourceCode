using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSManager : MonoBehaviour
{
    protected bool isReady = false;

    public virtual IEnumerator InitManager()
    {
        yield return null;
    }

    public virtual void ResetManager()
    {

    }

    public virtual IEnumerator ManagerInitProcessing()
    {
        isReady = true;

        Debug.Log(gameObject.name + " Init Complete");

        yield return null;
    }
}
