using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class InformationWindowEditor : EditorWindow
{
    private Vector2 objectlistAreaScroll = Vector2.zero;
    private float listAreaWidth = 230.0f;

    private Vector2 interactionlistAreaScroll = Vector2.zero;

    private Dictionary<StageScene, List<InteractionObject>> stageSceneList = new Dictionary<StageScene, List<InteractionObject>>();

    private List<InteractionObject> InteractionObject = new List<InteractionObject>();

    private StageScene prevStageScene;
    private StageScene currentStageScene;

    private InteractionObject currentObject;

    private Reaction currentReaction;

    private InformationAsset infoData;

    private string dataPath = "**********";

    private List<string> stageKeyList = new List<string>();
    private List<string> garbageKeyList = new List<string>();

    private string currentKey;
    private string[] currentValue = new string[(int)EGameLanuage.Count];

    private bool reactionFocus = false;

    private bool stageSceneFold = false;

    [MenuItem("************")]
    static void Init()
    {
        InformationWindowEditor window = (InformationWindowEditor)EditorWindow.GetWindow(typeof(InformationWindowEditor));
        window.titleContent.text = "Information Editor";
    }

    public void OnEnable()
    {
        CheckInteractionObject();

        LoadInformationData();

        MakeSyncDataList();
    }

    public void LoadInformationData()
    {
        Stage[] currentStage = PSHelper.FindObjectInScene<Stage>();

        if (currentStage.Length == 0)
            return;

        infoData = AssetDatabase.LoadAssetAtPath(dataPath + currentStage[0].key + "_Info.Asset", typeof(InformationAsset)) as InformationAsset;

        if (infoData == null)
        {
            Debug.Log("NewAsset!");
            infoData = CreateInstance<InformationAsset>();
            AssetDatabase.CreateAsset(infoData, dataPath + currentStage[0].key + "_Info.Asset");
            AssetDatabase.SaveAssets();
        }

    }

    #region [Sync]
    public void MakeSyncDataList()
    {
        stageKeyList.Clear();

        for (int i = 0; i < InteractionObject.Count; ++i)
        {
            SearchInteraction(InteractionObject[i].InteractionList);
        }
    }

    public void SearchInteraction(List<Interaction> interaction)
    {
        for (int i = 0; i < interaction.Count; ++i)
        {
            for(int j = 0; j < interaction[i].ReactionList.Count; ++j)
            {
                if (interaction[i].ReactionList[j].reactionType == EReactionType.Information)
                {
                    string key = interaction[i].ReactionList[j].key;
                    stageKeyList.Add(key);
                }
            }
        }
    }

    public void SyncData()
    {
        garbageKeyList.Clear();
        for (int i = 0; i < infoData.dataList.Count; ++i)
        {
            if (!CheckMainKey(infoData.dataList[i].mainKey))
                garbageKeyList.Add(infoData.dataList[i].mainKey);
        }

        for (int i = 0; i < garbageKeyList.Count; ++i)
        {
            Debug.Log(garbageKeyList[i]);
            infoData.RemoveData(garbageKeyList[i]);
        }
    }

    public bool CheckMainKey(string key)
    {
        for (int i = 0; i < stageKeyList.Count; ++i)
        {
            if (stageKeyList[i] == key)
                return true;
        }
        return false;
    }
    #endregion

    public void OnGUI()
    {
        GUILayout.BeginHorizontal(GUIStyle.none);
        GUI.color = Color.green;
        GUILayout.Label("Information Editor. Ver.0.03", EditorStyles.boldLabel, GUILayout.Width(200));
        GUI.color = Color.white;
        GUILayout.Space(10);

        if (GUILayout.Button("Save All", GUILayout.Width(100)))
        {
            EditorUtility.SetDirty(infoData);
            AssetDatabase.SaveAssets();
        }

        GUILayout.Space(10);
        if (GUILayout.Button("Cleaner", GUILayout.Width(100)))
        {
            MakeSyncDataList();
            SyncData();
            EditorUtility.SetDirty(infoData);
            AssetDatabase.SaveAssets();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(GUIStyle.none);

        DrawInteractionList();

        DrawInteraction();

        DrawReaction();

        GUILayout.EndHorizontal();
    }

    public void CheckInteractionObject()
    {
        InteractionObject.Clear();
        currentObject = null;
        currentReaction = null;

        StageScene[] scenes = PSHelper.FindObjectInScene<StageScene>();

        foreach (StageScene scene in scenes)
        {
            List<InteractionObject> tempList = new List<InteractionObject>();
            foreach (InteractionObject obj in PSHelper.FindObjectInParent<InteractionObject>(scene.gameObject))
            {
                for (int i = 0; i < obj.InteractionList.Count; ++i)
                {
                    if (FindInformationReaction(obj, obj.InteractionList[i]))
                    {
                        InteractionObject.Add(obj);
                        tempList.Add(obj);
                        break;
                    }
                }
            }

            if (tempList.Count != 0)
            {
                stageSceneList[scene] = new List<InteractionObject>();
                stageSceneList[scene] = tempList;
            }
        }
    }

    public bool FindInformationReaction(InteractionObject obj, Interaction interaction)
    {
        for(int i = 0; i < interaction.ReactionList.Count; ++i)
        {
            if(interaction.ReactionList[i].reactionType == EReactionType.Information)
            {
                return true;
            }
        }

        return false;
    }

    void DrawInteractionList()
    {
        objectlistAreaScroll = GUILayout.BeginScrollView(objectlistAreaScroll, "box", GUILayout.Width(listAreaWidth));
        int tempKey = 0;
        foreach (KeyValuePair<StageScene, List<InteractionObject>> scenes in stageSceneList)
        {
            GUILayout.BeginHorizontal(GUIStyle.none);
            string sceneFocusKey = "stageScene " + tempKey.ToString();
            tempKey++;
            GUI.SetNextControlName(sceneFocusKey);
            stageSceneFold = EditorGUILayout.Foldout(stageSceneFold, scenes.Key.name);
            GUILayout.EndHorizontal();

            if (GUI.GetNameOfFocusedControl() == sceneFocusKey)
            {
                currentStageScene = scenes.Key;
            }

            if (currentStageScene == scenes.Key && stageSceneFold)
            {
                for (int i = 0; i < scenes.Value.Count; ++i)
                {
                    GUILayout.BeginHorizontal(GUIStyle.none);
                    GUILayout.Space(30);
                    string listFocusKey = "Infomation " + i.ToString();
                    GUI.SetNextControlName(listFocusKey);
                    EditorGUILayout.Foldout(false, scenes.Value[i].name);

                    if (GUI.GetNameOfFocusedControl() == listFocusKey)
                    {
                        currentObject = scenes.Value[i];
                        Selection.activeGameObject = currentObject.gameObject;
                        currentReaction = null;
                        reactionFocus = false;
                        LocalDataReset();
                    }
                    GUILayout.EndHorizontal();
                }
            }
        }

        GUILayout.EndScrollView();
    }

    void DrawInteraction()
    {
        GUILayout.BeginVertical(GUILayout.Width(200));
        GUILayout.BeginHorizontal();
        GUI.color = Color.yellow;
        GUILayout.Label("            [ Sprite Image ]", GUILayout.Width(200));
        GUI.color = Color.white;
        Rect rt = GUILayoutUtility.GetLastRect();
        rt.x = 237;
        rt.y = 40;
        rt.width = 200;
        rt.height = 200;
        GUILayout.BeginArea(rt, GUI.skin.box);
        GUILayout.EndArea();
        rt.x = 240;
        rt.y = 43;
        rt.width = 194;
        rt.height = 194;
        GUILayout.BeginArea(rt, GUI.skin.box);

        bool IsSpriteObject = false;
        if (currentObject != null)
        {
            if (currentObject.GetComponent<SpriteRenderer>() != null)
                IsSpriteObject = true;
        }

        if(!IsSpriteObject)
        {
            GUILayout.Label("[Non Sprite Object]");
        }

        GUILayout.EndArea();

        GUILayout.EndHorizontal();
        GUILayout.Space(205);

        if (currentObject != null)
        {
            if (IsSpriteObject)
            {
                rt.x = 242;
                rt.y = 45;
                rt.width = 190;
                rt.height = 190;
                GUI.DrawTexture(rt, currentObject.GetComponent<SpriteRenderer>().sprite.texture, ScaleMode.ScaleToFit);
            }
            GUILayout.BeginHorizontal();

            interactionlistAreaScroll = GUILayout.BeginScrollView(interactionlistAreaScroll, "box", GUILayout.Width(listAreaWidth));

            for (int i = 0; i < currentObject.InteractionList.Count; ++i)
            {
                DrawReactionList(currentObject.InteractionList[i], i);
            }

            GUILayout.EndScrollView();
            GUILayout.EndHorizontal();
        }
        else
        {
            GUILayout.BeginHorizontal();

            interactionlistAreaScroll = GUILayout.BeginScrollView(interactionlistAreaScroll, "box", GUILayout.Width(listAreaWidth));

            GUILayout.EndScrollView();
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
    }

    public void DrawReactionList(Interaction interaction, int controlKey)
    {
        for (int i = 0; i < interaction.ReactionList.Count; ++i)
        {
            if (interaction.ReactionList[i].reactionType == EReactionType.Information)
            {
                GUILayout.BeginHorizontal(GUIStyle.none);

                string listFocusKey = "Reaction " + i.ToString() + controlKey.ToString();

                GUI.SetNextControlName(listFocusKey);
                EditorGUILayout.Foldout(false, interaction.ReactionList[i].key);

                if (GUI.GetNameOfFocusedControl() == listFocusKey)
                {
                    currentReaction = interaction.ReactionList[i];
                    SetLocalDataReset(currentReaction.key);
                    reactionFocus = true;
                }

                if(!reactionFocus)
                {
                    GUI.FocusControl(listFocusKey);
                    currentReaction = interaction.ReactionList[i];
                    SetLocalDataReset(currentReaction.key);
                    reactionFocus = true;
                }

                GUILayout.EndHorizontal();
            }
        }
    }

    public void DrawReaction()
    {
        GUILayout.BeginVertical();
        GUILayout.Space(30);
        if (currentReaction != null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.Label("[Reaction Key]", GUILayout.Width(100));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            currentKey = EditorGUILayout.TextField(currentKey, GUILayout.Width(200));
            if (currentKey != currentReaction.key)
            {
                if (GUILayout.Button("Change Key", GUILayout.Width(100)))
                {
                    currentReaction.key = currentKey;
                    SetLocalDataReset(currentKey);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(30);

            for (int i = 0; i < currentValue.Length; ++i)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(10);
                GUILayout.Label("[" + ((EGameLanuage)i).ToString() + "]", GUILayout.Width(100));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(10);

                currentValue[i] = EditorGUILayout.TextArea(currentValue[i], GUILayout.Width(800), GUILayout.Height(50));
                if (GUI.changed)
                {
                    infoData.AddData(currentReaction.key, ((EGameLanuage)i).ToString(), currentValue[i]);
                    currentValue[i] = infoData.GetData(currentReaction.key, ((EGameLanuage)i).ToString());
                }
                GUILayout.EndHorizontal();
            }

           
        }
        GUILayout.EndVertical();
    }

    public void LocalDataReset()
    {
        currentKey = "";
        for(int i = 0; i < currentValue.Length; ++i)
        {
            currentValue[i] = "";
        }
    }

    public void SetLocalDataReset(string key)
    {
        currentKey = key;

        for (int i = 0; i < currentValue.Length; ++i)
        {
            currentValue[i] = infoData.GetData(currentKey, ((EGameLanuage)i).ToString());
        }
    }
}