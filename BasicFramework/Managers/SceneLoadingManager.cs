using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadingManager : PSManager
{
    [HideInInspector]
    public bool isLoadingComplete = false;

    private string targetSceneName = "";

    private float currentProgress = 0.0f;
    private float targetProgress = 0.0f;

    public override IEnumerator ManagerInitProcessing()
    {
        yield return StartCoroutine(InitManager());

        yield return StartCoroutine(base.ManagerInitProcessing());
    }

    public override IEnumerator InitManager()
    {
        yield return StartCoroutine(base.InitManager());
    }

    public IEnumerator SceneLoadingWithAsync(string targetScene)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Single);

        async.allowSceneActivation = false;

        while (async.progress < 0.9f)
        {
            yield return null;
        }

        async.allowSceneActivation = true;

        while (!async.isDone)
        {
            yield return null;
        }

        yield return async;

        yield return new WaitForSeconds(1f);
        isLoadingComplete = true;

        yield return new WaitUntil(() => isLoadingComplete);

        StartCoroutine(SceneLoadingWithAsync("Game"));
    }

    public void SetTargetProgress(float target)
    {
        targetProgress = target;
    }

    public IEnumerator SceneLoadingWithAdditive(string targetScene)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive);

        async.allowSceneActivation = false;

        while (async.progress < 0.9f)
        {
            yield return null;
        }

        async.allowSceneActivation = true;

        while (!async.isDone)
        {
            yield return null;
        }

        yield return async;

    }

    public IEnumerator SceneUnLoad(string sceneName)
    {
        AsyncOperation async = SceneManager.UnloadSceneAsync(sceneName);

        while (!async.isDone)
        {
            yield return null;
        }

        yield return true;
    }
}
