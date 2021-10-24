using UnityEngine;
using System.Collections;

public class PopUpPage : MonoBehaviour {

    public EPopUpMenu popName = EPopUpMenu.None;

    [HideInInspector]
    public delegate void ProcessAction();
    [HideInInspector]
    public ProcessAction ActionFunc;

    [HideInInspector]
    public bool bPopUpOpen = false;

    public virtual void EnterPopUp()
    {

    }

    public virtual void EnterPopUp(ProcessAction Func)
    {
        ActionFunc = Func;
    }

    public virtual void EnterPopUp(ProcessAction Func, int msgType)
    {
        ActionFunc = Func;
    }

    public virtual void EnterPopUp(ProcessAction Func, string type)
    {
        ActionFunc = Func;
    }

    public virtual void EnterPopUp<T>(T type)
    {

    }

    public virtual void EnterPopUp(string type)
    {

    }

    public virtual void EnterPopUp(int errcode)
    {

    }

	public virtual bool IsShow()
	{
		return gameObject.activeSelf;
	}

	public virtual void ClosePopUp()
    {
        if (GetComponent<TweenHelper>() != null)
            GetComponent<TweenHelper>().Reset();

		if( GlobalUI.instance != null )
	        GlobalUI.instance.PopUpClose();

    }
}
