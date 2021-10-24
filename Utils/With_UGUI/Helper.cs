using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public static class Helper
{
    public static T[] FindObjectInScene<T>()
    {
        List<T> objList = new List<T>();
        Scene activeScene = SceneManager.GetActiveScene();
        GameObject[] objs = activeScene.GetRootGameObjects();
        for (int i = 0; i < objs.Length; ++i)
        {
            T[] obj = objs[i].GetComponentsInChildren<T>(true);
            for (int j = 0; j < obj.Length; ++j)
            {
                objList.Add(obj[j]);
            }
        }

        return objList.ToArray();
    }

    public static T[] FindObjectInParent<T>(GameObject parent)
    {
        List<T> objList = new List<T>();
        Scene activeScene = SceneManager.GetActiveScene();

        T[] obj = parent.GetComponentsInChildren<T>(true);
        for (int j = 0; j < obj.Length; ++j)
        {
            objList.Add(obj[j]);
        }

        return objList.ToArray();
    }

    public static T[] FindObjectInProject<T>() where T : Object
    {
        T[] objs = Resources.FindObjectsOfTypeAll<T>();

        return objs;
    }

    public static T FindObjectByName<T>(string name) where T : Object
    {
        T[] objs = Resources.FindObjectsOfTypeAll<T>();

        for (int i = 0; i < objs.Length; ++i)
        {
            if (objs[i].name == name)
            {
                return objs[i];
            }
        }

        return null;
    }

    public static void ChangeLanguageSetting(EGameLanuage lan)
    {
        switch (lan)
        {
            case EGameLanuage.Korean:
                Core.RSS.SetCurrentLanguage(EGameLanuage.Korean);
                break;
            case EGameLanuage.English:
                Core.RSS.SetCurrentLanguage(EGameLanuage.Korean);
                break;
            case EGameLanuage.Japanese:
                Core.RSS.SetCurrentLanguage(EGameLanuage.Korean);
                break;
            case EGameLanuage.Russian:
                Core.RSS.SetCurrentLanguage(EGameLanuage.Korean);
                break;
        }
    }

    public static int GetTotalDaysFromYear(System.DateTime time, int from)
    {
        int year = time.Year - from;
        int days = time.DayOfYear;

        return (year * 365) + days;
    }

    public static bool ScrambledEquals<T>(IEnumerable<T> list1, IEnumerable<T> list2)
    {
        var cnt = new Dictionary<T, int>();
        foreach (T s in list1)
        {
            if (cnt.ContainsKey(s))
            {
                cnt[s]++;
            }
            else
            {
                cnt.Add(s, 1);
            }
        }
        foreach (T s in list2)
        {
            if (cnt.ContainsKey(s))
            {
                cnt[s]--;
            }
            else
            {
                return false;
            }
        }
        return cnt.Values.All(c => c == 0);
    }

    public static bool IsInternetAccess()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
            return false;

        else
            return true;
    }

    public static Vector2 ConvertScreenToAnchoredPos(RectTransform parent, Vector3 screen, Camera uiCam)
    {
        Vector2 newInputPos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, screen, uiCam, out newInputPos);

        return newInputPos;
    }
}
