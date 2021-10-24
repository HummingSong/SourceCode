using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

/// <summary>
/// Chromer. 190219
/// 스테이지 이닛 및 관리
/// </summary>

public class Stage : MonoBehaviour
{
    public string key;

    // 초기 저장된 데이터
    public List<StageScene> sceneList = new List<StageScene>();
    
    // 접근하기 위한 데이터 ( 실행 후 만들어짐 )
    public Dictionary<string, StageScene> stageSceneList = new Dictionary<string, StageScene>();

    [HideInInspector]
    public StageScene currentStageScene = null;
    [HideInInspector]
    public StageScene prevStageScene = null;
    [HideInInspector]
    public Stack<StageScene> StageScenesStack = new Stack<StageScene>();

    [HideInInspector]
    public Dictionary<string, bool> miniGameChecker = new Dictionary<string, bool>();

    // 사운드가 자체적으로 나는 오브젝트을 위한 리스트
    public Dictionary<string, GameObject> soundObjectList = new Dictionary<string, GameObject>();

    [HideInInspector]
    public GameObject currentMiniGame;
    private TriggerObject currentTrigger;

    [HideInInspector]
    public bool stageClear = false;

    [HideInInspector]
    public List<DialogueList> stageDialogueList = new List<DialogueList>();

    [HideInInspector]
    public int hintStep = 0;

    // for autosave
    private GameObject objLastClicked;

    public List<string> otherDialogueKey = new List<string>();

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        // 인풋 초기화
        PSCore.instance.INPUT.input.SetTouch(true);
        PSCore.instance.INPUT.input.SetInputMode(EInputMode.Game);

        PSCore.instance.STATE.SetGameFlow(EGameFlow.Stage);
        PSCore.instance.STATE.SetCurrentStage(this);
        GameUI.instance.SetGameUI(EGameFlow.Stage);

        stageClear = false;
        hintStep = 0;

        LoadData();

        stageSceneList.Clear();
        miniGameChecker.Clear();

        // 초기 상태로 이닛
        for (int i = 0; i < sceneList.Count; ++i)
        {
            sceneList[i].Init();
            stageSceneList.Add(sceneList[i].key, sceneList[i]);

            sceneList[i].gameObject.SetActive(false);
        }


        LoadStage();

        //FirstGameStart(); <- LoadStage 에 합류

    }

    private void LoadData()
    {
        // 인벤토리 이닛
        PSCore.instance.STATE.userInven.Init();

        // 로컬라이즈 데이터도 읽고~
        PSCore.instance.RESOURCE.LoadStageData(key);

        // 다이얼로그 데이터도 읽고~
        PSCore.instance.RESOURCE.LoadDialouge(key, otherDialogueKey);

        // 구분한다음 캐릭터 아틀라스 읽고,
        PSCore.instance.RESOURCE.LoadCharacterAtlas();

        // 힌트 데이터도 읽고~
        PSCore.instance.RESOURCE.LoadHint(key);
    }

    private void LoadStage()
    {
        PSCore.instance.STATE.DlcNotSavaCheck = false;
        for (int i = 0; i < sceneList.Count; ++i)
        {
            if (sceneList[i].IsFirst)
            {
                sceneList[i].gameObject.SetActive(true);
                currentStageScene = sceneList[i];
                currentStageScene.ManualSetting();
                break;
            }
        }

        stageDialogueList.Clear();

        // 세이브 된것이 있으면 읽고 아니면 패스
        if (PSCore.instance.STATE.IsSavedStage(key) && PSCore.instance.STATE.LoadSavedData)
        {
            Debug.Log("Load Saved Stage");
            LoadAllSavedInteraction();
            LoadAllSavedSound();
            //LoadAllMiniGameClear();
        }
        else
        {
            if (key.Contains("Gallery"))
            {
                PSCore.instance.STATE.DlcNotSavaCheck = true;
            }
            else
            {
                // 기존 세이브파일 삭제!
                PSCore.instance.STATE.ClearStageData();

                // 저장 시작
                PSCore.instance.STATE.SaveStage(key);
            }       

            FirstGameStart();
        }
     
    }

    public void FirstGameStart()
    {
        currentStageScene.UpdateInteraction();
    }

    public void ChangeStageScene(string key, bool auto = false)
    {
        StartCoroutine(ChangeStageSceneRoutine(key, auto));
    }

    public IEnumerator ChangeStageSceneRoutine(string key, bool auto)
    {
        if(!auto)
            yield return GlobalUI.instance.OpenFadeIn(1.0f, 0.3f);

        StageScenesStack.Push(currentStageScene);
        currentStageScene.gameObject.SetActive(false);
        currentStageScene = stageSceneList[key];

        currentStageScene.ManualSetting();
        currentStageScene.gameObject.SetActive(true);

        if (!auto)
            yield return GlobalUI.instance.OpenFadeOut(0.0f, 0.5f);
    }

    public void ChangePrevScene(bool auto = false)
    {
        StartCoroutine(ChangePrevSceneRoutine(auto));
    }

    public IEnumerator ChangePrevSceneRoutine(bool auto)
    {
        if(!auto)
            yield return GlobalUI.instance.OpenFadeIn(1.0f, 0.3f);

        if (PSCore.instance.STATE.GetCurrentGameFlow() == EGameFlow.Stage)
        {
            currentStageScene.gameObject.SetActive(false);
            currentStageScene = StageScenesStack.Pop();

            currentStageScene.ManualSetting();
            currentStageScene.gameObject.SetActive(true);
        }
        else if (PSCore.instance.STATE.GetCurrentGameFlow() == EGameFlow.MiniGame)
        {
            PSCore.instance.INPUT.input.SetTouch(true);
            currentMiniGame.SetActive(false);

            currentStageScene.ManualSetting();
            currentStageScene.gameObject.SetActive(true);

            PSCore.instance.STATE.SetGameFlow(EGameFlow.Stage);

            if (IsMiniGameClear(currentMiniGame.GetComponent<MiniGame>().key) && !auto)
                currentTrigger.ClickObject();

            currentMiniGame = null;
        }

        if (!auto)
            yield return GlobalUI.instance.OpenFadeOut(0.0f, 0.5f);
    }

    public void ChangeMiniGame(GameObject miniObject, TriggerObject trigger, bool auto = false)
    {
        StartCoroutine(ChangeMiniGameRoutine(miniObject, trigger, auto));
    }

    public IEnumerator ChangeMiniGameRoutine(GameObject miniObject, TriggerObject trigger, bool auto)
    {
        if(!auto)
            yield return GlobalUI.instance.OpenFadeIn(1.0f, 0.3f);

        if(trigger != null)
            currentTrigger = trigger;

        GameUI.instance.ResetMiniGameUI();

        PSCore.instance.INPUT.input.SetTouch(false);
        currentMiniGame = miniObject;
        currentStageScene.gameObject.SetActive(false);
        miniObject.GetComponent<MiniGame>().ManualSetting();
        miniObject.SetActive(true);

        PSCore.instance.STATE.SetGameFlow(EGameFlow.MiniGame);

        if (!auto)
            yield return GlobalUI.instance.OpenFadeOut(0.0f, 0.5f);
    }

    //  Dictionary를 읽음 접근하기 위한 데이터(실행 후 만들어짐 )
    public StageScene GetStageScene(string key)
    {
        if(!stageSceneList.ContainsKey(key))
            return null;

        return stageSceneList[key];
    }

    public StageScene GetStageSceneList(string key)
    {
        for(int i = 0; i < sceneList.Count; i++)
        {
            if (sceneList[i].name == key)
                return sceneList[i];
        }
        return null;
    }

    public bool IsMiniGameClear(string key)
    {
        if (miniGameChecker.ContainsKey(key))
            return miniGameChecker[key];

        return false;
    }

    public void SetMiniGameClear(string key, bool clear)
    {
        if (miniGameChecker.ContainsKey(key))
            miniGameChecker[key] = clear;
        else
        {
            miniGameChecker[key] = clear;
        }

        // 자동 저장
        if(clear && PSCore.instance.STATE.StageLoadComplete)
        {
            PSCore.instance.STATE.SaveInteraction("MiniGameClear," + key);
        }
    }

    public void RegisterSoundObject(string name, GameObject obj)
    {
        if (!soundObjectList.ContainsKey(name))
            soundObjectList.Add(name, obj);
    }

    public void RemoveSoundObject(string name, GameObject obj)
    {
        if (!soundObjectList.ContainsKey(name))
            return;

        obj.GetComponent<AudioSource>().Stop();
        soundObjectList.Remove(name);
    }

    public void ClearSoundObject()
    {
        foreach(KeyValuePair<string,GameObject> obj in soundObjectList)
        {
            obj.Value.GetComponent<AudioSource>().Stop();
        }

        soundObjectList.Clear();
    }

    public void FixedUpdate()
    {
        if (currentStageScene != null)
        {
            currentStageScene.UpdateStageScene();
        }
    }

    public void Update()
    {
        if(currentStageScene != null)
        {
            currentStageScene.UpdateCamera();
        }
    }

    public void StageClearRoutine()
    {
#if UNITY_EDITOR

#else
        PSCore.instance.STATE.SetVideoAdsState(false);
#endif
        if (PSCore.instance.STATE.DlcNotSavaCheck)
        {
            GlobalUI.instance.RemoveDialContainer();
            PSCore.instance.Get<SceneLoadManager>().SceneLoadNormal("Gallery", EGameFlow.Gallery, ESceneFadeType.FadeInOut);
        }
        else
        {
            GlobalUI.instance.RemoveDialContainer();
            PSCore.instance.Get<SceneLoadManager>().SceneLoadNormal("StageSelect", EGameFlow.StageSelect, ESceneFadeType.FadeInOut);
        }
    }

    public void StageClear()
    {
        //stageClear = true;

        if (!PSCore.instance.STATE.DlcNotSavaCheck)
        {
            stageClear = true;
            // 저장해주세요~~~~ 호출도 해야함
            PSCore.instance.STATE.SaveStageClear(key);
            PSCore.instance.STATE.SaveStageClearSignal(); // 클리어 했다고 저장
        }       

        PSCore.instance.Get<TriggerManager>().StopReaction();

        /*
         */  
        
        GlobalUI.instance.CloseAllPopUP();
    }

    public void HintStepLevelUp()
    {
        hintStep += 1;
    }

    #region [ Review or History Dialogue ]
    public void SetDialogueText(List<DialogueList> list)
    {
        for (int i = 0; i < list.Count; ++i)
        {
            if(list[i].dialougeType != EDialogueType.None)
            stageDialogueList.Add(list[i]);
        }

        for (int i = 0; i < list.Count; ++i)
        {
            if(list[i].dialougeType == EDialogueType.Dialogue)
            {
                GlobalUI.instance.AddDialogue(list[i].name, list[i].text);
            }
            else if (list[i].dialougeType == EDialogueType.Narration)
            {
                GlobalUI.instance.AddNarration(list[i].text);
            }
        }
    }
    #endregion

    // 로드 스테이지를 위한 부분

    #region [ Action by InterationKeyName ]
    public void LoadAllSavedInteraction()
    {
        InteractionObject[] obj = PSHelper.FindObjectInScene<InteractionObject>();
        List<Interaction> tempInteracion = new List<Interaction>();

        for(int i = 0; i < PSCore.instance.STATE.GetSavedStageData().clearedInteraction.Count; ++i)
        {
            if( PSCore.instance.STATE.GetSavedStageData().clearedInteraction[i].Contains("MiniGameClear,"))
            {
                string[] spt = PSCore.instance.STATE.GetSavedStageData().clearedInteraction[i].Split(',');
                Interaction tempInt = new Interaction();
                Reaction tempRe = new Reaction();
                tempRe.reactionType = EReactionType.MiniGameClear;
                tempRe.key = spt[1];
                tempInt.ReactionList.Add(tempRe);
                tempInt.IsDisposable = false;
                tempInt.IsUsed = false;
                tempInteracion.Add(tempInt);
            }
            else if (PSCore.instance.STATE.GetSavedStageData().clearedInteraction[i] == "ChangePrevScene")
            {
                Interaction tempInt = new Interaction();
                Reaction tempRe = new Reaction();
                tempRe.reactionType = EReactionType.ChangePrevScene;
                tempInt.ReactionList.Add(tempRe);
                tempInt.IsDisposable = false;
                tempInt.IsUsed = false;
                tempInteracion.Add(tempInt);
            }
            else
                tempInteracion.Add(FindInteractionInTrigger(obj, PSCore.instance.STATE.GetSavedStageData().clearedInteraction[i]));
        }

        StartCoroutine(AutoInteractionPlay(tempInteracion));
    }

    public void LoadAllSavedSound()
    {
        if(PSCore.instance.STATE.GetSavedStageData().savedBgm.on)
        {
            AudioClip clip = PSHelper.FindObjectByName<AudioClip>(PSCore.instance.STATE.GetSavedStageData().savedBgm.audioClip);
            AudioMixerGroup mixer = PSHelper.FindObjectByName<AudioMixerGroup>(PSCore.instance.STATE.GetSavedStageData().savedBgm.audioMixer);

            PSCore.instance.SOUND.PlayBGM(clip, mixer, PSCore.instance.STATE.GetSavedStageData().savedBgm.volume, ESoundPlayType.Loop, false);
        }
    }

    public void LoadAllMiniGameClear()
    {
        for(int i = 0; i < PSCore.instance.STATE.GetSavedStageData().clearedMiniGames.Count; ++i)
        {
            miniGameChecker[PSCore.instance.STATE.GetSavedStageData().clearedMiniGames[i]] = true;
        }
    }

    public IEnumerator AutoInteractionPlay(List<Interaction> interaciotnList)
    {
        PSCore.instance.Get<TriggerManager>().SetTempObject(objLastClicked);
        currentTrigger = objLastClicked.GetComponent<TriggerObject>();

        for (int i = 0; i < interaciotnList.Count; ++i)
        {
            yield return StartCoroutine(PSCore.instance.Get<TriggerManager>().AutoInteractionAction(interaciotnList[i]));
        }

        PSCore.instance.STATE.StageLoadComplete = true;
    }

    public Interaction FindInteractionInTrigger(InteractionObject[] obj, string key)
    {
        for(int i = 0; i < obj.Length; ++i)
        {
            for(int j = 0; j < obj[i].InteractionList.Count; ++j)
            {
                if(obj[i].InteractionList[j].key == key)
                {
                    objLastClicked = obj[i].gameObject;
                    return obj[i].InteractionList[j];
                }
            }
        }

        return null;
    }
#endregion

#region [ Action by GameObject Click ]
    public void LoadAllSavedObject()
    {
        TriggerObject[] obj = PSHelper.FindObjectInScene<TriggerObject>();

        List<Interaction> tempInteracion = new List<Interaction>();

        for (int i = 0; i < PSCore.instance.STATE.GetSavedStageData().clickedObject.Count; ++i)
        {
            tempInteracion.Add(FindInteractionInTrigger(obj, PSCore.instance.STATE.GetSavedStageData().clearedInteraction[i]));
        }
    }

    public TriggerObject FindTriggerInGameObject(TriggerObject[] obj, string key)
    {
        for (int i = 0; i < obj.Length; ++i)
        {
            if (obj[i].name == key)
                return obj[i];
        }

        return null;
    }
#endregion
}



