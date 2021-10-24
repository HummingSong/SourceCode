using UnityEngine;
using System.Collections;

public class BtnPressState : MonoBehaviour {

    public float fCheckTime = 2.0f;

    private float fPressTime = 0.0f;

    [HideInInspector]
    public bool bPress = false;

    [HideInInspector]
    public bool bHaveAction = false;

    [HideInInspector]
    public delegate void ProcessAction(int slot);

    [HideInInspector]
    public event ProcessAction ActionFunc = null;

    [HideInInspector]
    public int slotindex = 0;

    private float fAddSpeed = 0.0f;
    private float fSpeed = 0.1f;

    public void OnPress (bool isPressed)
    {
        bPress = isPressed;
    }

    void Update()
    {
        if (bPress)
        {
            fPressTime += Time.deltaTime;
        }
        else
        {
            fPressTime = 0.0f;
            fAddSpeed = 0.0f;
            fSpeed = 0.1f;
        }

        if (fPressTime >= fCheckTime)
        {
            if (ActionFunc != null)
            {
                fPressTime -= fSpeed;
                ActionFunc(slotindex);
            }
        }
    }

    public void SetProcessing(ProcessAction Func, int slot)
    {
        ActionFunc = Func;
        slotindex = slot;
    }
}
