/// 
/// --------------------------- 기본 규약 ------------------------------------
/// [ 명명법 ]
/// * int, float, bool 등 일반 변수의 네이밍은 반드시 소문자로 시작한다.
/// * GameObject : obj*** 로 시작하여 명명한다.
/// * Transform : tr*** 로 시작하여 명명한다.
/// * enum : e*** 로 시작하여 명명한다.
/// * struct : st*** 로 시작하여 명명한다.
/// * class : 대문자로 시작하여 명명한다.
/// 추후 추가
/// ----------------------------------------------------------------------------


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEngine : MonoBehaviour
{
    public static GameEngine instance;
    private Core gameCore;

    public int targetFrameRate = 60;

    private static float deltaTime = 0.0f;
    public static float fps { get; private set; }

    [HideInInspector]
    public string errMsg;

    public bool showFPS;
    public bool showLog;

    [HideInInspector]
    public bool isDev = false;

    public string clientVersion;

    public bool tutorialStart = false;
    public bool linkDataBase = false;
    public bool usingLog = false;

    public void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        StartCoroutine(EngineInit());
    }

    public IEnumerator EngineInit()
    {
        yield return StartCoroutine(FlowCheck());

        yield return StartCoroutine(Init());
    }

    // 귀찮게 하는 부분이 있어서 개선해야할 필요가 있긴허다.
    public IEnumerator FlowCheck()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("GameEngine");

        if (obj != null)
        {
            if (obj != gameObject)
            {
                Destroy(gameObject);
                yield break;
            }
        }

        if (tag == "GameEngineDev")
            isDev = true;
        else
            isDev = false;

        yield return null;
    }

    public IEnumerator Init()
    {
        DontDestroyOnLoad(this);

        Application.targetFrameRate = targetFrameRate;

        gameCore = GetComponent<Core>();
        gameCore.Init();

        yield return null;
    }

#if DEV_LOG
    public void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        fps = 1.0f / deltaTime;
    }

    private void OnGUI()
    {
        var boxstyle = GUI.skin.GetStyle("Box");
        boxstyle.alignment = TextAnchor.MiddleLeft;
        boxstyle.fontSize = 30;
        boxstyle.normal.textColor = Color.cyan;

        GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));

        GUILayout.FlexibleSpace();

        if (showFPS)
        {
            GUILayout.BeginHorizontal();
            fps = Mathf.Round(fps * 100.0f) / 100.0f;
            var fpsmsg = "fps : " + fps.ToString("N2");
            GUILayout.Box(fpsmsg, boxstyle, GUILayout.ExpandWidth(false));
            GUILayout.EndHorizontal();
        }

        if (showLog)
        {
            var message = "err : " + errMsg;
            GUILayout.Box(message, boxstyle, GUILayout.ExpandWidth(false));
        }
        GUILayout.EndArea();
    }
#endif

    public void SetLog(string log, bool clear = false)
    {
        if (clear)
            errMsg = "";

        errMsg += log + ".";
    }
}
