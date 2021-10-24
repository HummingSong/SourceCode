using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Audio;

[CustomEditor(typeof(InteractionObject))]
public class InteractionEditor : Editor
{
    InteractionObject interactionObject = null;

    Reaction tempReaction = null;
    Interaction tempInteraction = null;

    public void OnEnable()
    {
        interactionObject = (InteractionObject)target;
    }

    public void DrawInteraction()
    {
        interactionObject.key = interactionObject.name;

        tempInteraction = null;
        if (GUILayout.Button("+ Interaction \n Add First", GUILayout.Width(200)))
        {
            Interaction inter = new Interaction();
            interactionObject.InteractionList.Insert(0,inter);
        }

        for (int i = 0; i < interactionObject.InteractionList.Count; ++i)
        {
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.BeginVertical(GUI.skin.box);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Up", GUILayout.Width(35)))
            {
                MoveUpInteraction(i);
            }
            if (GUILayout.Button("Down", GUILayout.Width(45)))
            {
                MoveDownInteraction(i);
            }

            GUILayout.Label((i + 1).ToString() + " : Interaction Key", GUILayout.Width(130));
            /*
             */

            if (GUILayout.Button("-", GUILayout.Width(30)))
            {
                tempInteraction = interactionObject.InteractionList[i];
                break;
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Disposable", GUILayout.Width(80));
            interactionObject.InteractionList[i].IsDisposable = EditorGUILayout.Toggle(interactionObject.InteractionList[i].IsDisposable);
            GUILayout.EndHorizontal();

            DrawEachInteraction(interactionObject.InteractionList[i]);
            GUILayout.EndVertical();
            GUILayout.EndVertical();
            GUILayout.Space(2);
        }

        if (tempInteraction != null)
        {
            interactionObject.InteractionList.Remove(tempInteraction);
        }

        if (GUILayout.Button("+ Interaction \n Add Last", GUILayout.Width(200)))
        {
            Interaction inter = new Interaction();
            interactionObject.InteractionList.Add(inter);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(interactionObject.gameObject.scene);
        }
    }


    void DrawEachInteraction(Interaction interaction)
    {
        GUILayout.BeginVertical();

        for (int i = interaction.ConditionList.Count - 1; i >= 0; --i)
        {
            DrawCondition(interaction, interaction.ConditionList[i]);
        }

        GUILayout.BeginHorizontal();
        GUILayout.Space(50);

        if (GUILayout.Button("+Condition", GUILayout.Width(150)))
        {
            interaction.ConditionList.Add(new Condition());
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        // 인스펙터창에 그려지는 순서를 위해 삭제를 후처리로 뺀다.
        tempReaction = null;
        for (int i = 0; i < interaction.ReactionList.Count; ++i)
        {
            DrawReaction(interaction, interaction.ReactionList[i], i);
        }

        if (tempReaction != null)
        {
            interaction.ReactionList.Remove(tempReaction);
        }

        GUILayout.BeginHorizontal();
        GUILayout.Space(50);
        if (GUILayout.Button("+Reaction", GUILayout.Width(150)))
        {
            interaction.ReactionList.Add(new Reaction());
        }
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    void DrawCondition(Interaction interaction, Condition con)
    {
        GUILayout.BeginVertical(GUI.skin.box);
        GUILayout.BeginHorizontal();
        GUILayout.Space(50);

        GUILayout.Label("Condition", GUILayout.Width(100));
        con.conditionType = (EConditionType)EditorGUILayout.EnumPopup(con.conditionType);
        con.key = EditorGUILayout.TextField(con.key, GUILayout.MinWidth(100));

        if (GUILayout.Button("-", GUILayout.Width(30)))
        {
            interaction.ConditionList.Remove(con);
        }
        GUILayout.EndHorizontal();

        if (con.conditionType == EConditionType.Active
            || con.conditionType == EConditionType.NotActive
            || con.conditionType == EConditionType.StateCheck)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(100);

            if (GUILayout.Button("AutoLink", GUILayout.Width(100)))
            {
                if (con.key == "")
                {
                    con.key = interactionObject.key;
                    con.targetObject = (TriggerObject)EditorGUILayout.ObjectField(interactionObject, typeof(TriggerObject), true);
                }
                else
                    con.targetObject = LinkTriggerObject(con.key);
            }

            con.targetObject = (TriggerObject)EditorGUILayout.ObjectField(con.targetObject, typeof(TriggerObject), true);
            GUILayout.EndHorizontal();
        }

        if (con.conditionType == EConditionType.StateCheck)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(200);
            GUILayout.Label("CheckState");
            con.targetState = (EObjectState)EditorGUILayout.EnumPopup(con.targetState);
            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();
    }

    void DrawReaction(Interaction interaction, Reaction react, int index)
    {
        GUILayout.BeginVertical(GUI.skin.box);
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Up", GUILayout.Width(35)))
        {
            MoveUpReaction(interaction, index);
        }
        if (GUILayout.Button("Down", GUILayout.Width(45)))
        {
            MoveDownReaction(interaction, index);
        }

        GUILayout.Space(10);

        GUILayout.Label("Reaction " + (index + 1).ToString(), GUILayout.Width(80));
        react.reactionType = (EReactionType)EditorGUILayout.EnumPopup(react.reactionType);
        react.key = EditorGUILayout.TextField(react.key, GUILayout.MinWidth(100));

        if (react.reactionType == EReactionType.Dialogue
            || react.reactionType == EReactionType.Information)
        {
            react.delayType = EDelayType.NoDelay;
        }

        react.delayType = (EDelayType)EditorGUILayout.EnumPopup(react.delayType);
        react.delayValue = EditorGUILayout.FloatField(react.delayValue);

        if (GUILayout.Button("-", GUILayout.Width(30)))
        {
            tempReaction = react;
            return;
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(20);
        GUILayout.Label("Sound On :", GUILayout.Width(70));
        react.soundEnable = EditorGUILayout.Toggle(react.soundEnable, GUILayout.Width(30));

        if (react.reactionType == EReactionType.ActiveObject
           || react.reactionType == EReactionType.InActiveObject
           || react.reactionType == EReactionType.ChangeSprite
           || react.reactionType == EReactionType.MoveObject
           || react.reactionType == EReactionType.ChangeState
           || react.reactionType == EReactionType.ZoomIn
           || react.reactionType == EReactionType.SoundOffObject
           || react.reactionType == EReactionType.RotateObject)
        {
            /*
             */
        }
        else if (react.reactionType == EReactionType.LinkInteraction)
        {
            /*
             */
        }
        else if (react.reactionType == EReactionType.MiniGame)
        {
            /*
             */
        }
        else if (react.reactionType == EReactionType.Dialogue)
        {
            /*
             */
        }
        GUILayout.EndHorizontal();

        if (react.reactionType == EReactionType.MoveObject)
        {
            /*
              */
        }
        else if(react.reactionType == EReactionType.RotateObject)
        {
            /*
             */
        }
        else if (react.reactionType == EReactionType.ChangeSprite)
        {
            /*
             */
        }
        else if (react.reactionType == EReactionType.ChangeState)
        {
            /*
             */
        }

        if (react.soundEnable)
        {
            /*
             */
        }

        GUILayout.EndVertical();
    }

    public TriggerObject LinkTriggerObject(string key)
    {
        if (key == "")
            return null;

        TriggerObject[] obj = PSHelper.FindObjectInScene<TriggerObject>();

        for (int i = 0; i < obj.Length; ++i)
        {
            if (obj[i].key == key)
            {
                return obj[i];
            }
        }

        return null;
    }

    public MiniGame LinkMiniGame(string key)
    {
        if (key == "")
            return null;

        MiniGame[] obj = PSHelper.FindObjectInScene<MiniGame>();
 
        for (int i = 0; i < obj.Length; ++i)
        {
            if (obj[i].key == key)
            {
                return obj[i];
            }
        }

        return null;
    }

    public void MoveUpReaction(Interaction interaction, int index)
    {
        if (index == 0)
            return;

        Reaction prevReact = interaction.ReactionList[index - 1];
        interaction.ReactionList[index - 1] = interaction.ReactionList[index];
        interaction.ReactionList[index] = prevReact;
    }

    public void MoveDownReaction(Interaction interaction, int index)
    {
        if (index == interaction.ReactionList.Count - 1)
            return;

        Reaction nextReact = interaction.ReactionList[index + 1];
        interaction.ReactionList[index + 1] = interaction.ReactionList[index];
        interaction.ReactionList[index] = nextReact;
    }

    public void MoveUpInteraction(int index)
    {
        if (index == 0)
            return;

        Interaction prevInter = interactionObject.InteractionList[index - 1];
        interactionObject.InteractionList[index - 1] = interactionObject.InteractionList[index];
        interactionObject.InteractionList[index] = prevInter;
    }

    public void MoveDownInteraction(int index)
    {
        if (index == interactionObject.InteractionList.Count - 1)
            return;

        Interaction nextInter = interactionObject.InteractionList[index + 1];
        interactionObject.InteractionList[index + 1] = interactionObject.InteractionList[index];
        interactionObject.InteractionList[index] = nextInter;
    }
}
