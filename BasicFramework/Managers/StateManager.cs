using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : PSManager
{
    public override IEnumerator ManagerInitProcessing()
    {
        yield return StartCoroutine(InitManager());

        yield return StartCoroutine(base.ManagerInitProcessing());
    }

    public override IEnumerator InitManager()
    {
        yield return StartCoroutine(base.InitManager());
    }

    public void StateInit()
    {
        StartCoroutine(InitStateProcess());
    }

    public IEnumerator InitStateProcess()
    {
        yield return new WaitForSeconds(0.5f);

        LoadingUI.instance.SetLoadingGage(0.2f);

        yield return new WaitForSeconds(0.5f);

        LoadingUI.instance.SetLoadingGage(0.4f);

        yield return new WaitForSeconds(0.5f);

        LoadingUI.instance.SetLoadingGage(0.6f);

        yield return new WaitForSeconds(0.5f);

        LoadingUI.instance.SetLoadingGage(0.8f);

        yield return new WaitForSeconds(0.5f);

        LoadingUI.instance.SetLoadingGage(1.0f);

        Core.LOADING.isLoadingComplete = true;

        yield return true;
    }
}
