using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class InformationAsset : ScriptableObject
{
    [HideInInspector]
    public List<DoubleKeyData> dataList = new List<DoubleKeyData>();

    #region [InGame]
    #endregion

    public void Save()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }

    public bool ContainsKey(string key)
    {
        for(int i = 0; i < dataList.Count; ++i)
        {
            if (dataList[i].mainKey == key)
                return true;
        }

        return false;
    }

    public void AddData(string key, string lan, string text)
    {
        bool hasKey = false;
        for (int i = 0; i < dataList.Count; ++i)
        {
            if (dataList[i].mainKey == key)
            {
                dataList[i].Add(key, lan, text);
                hasKey = true;
                break;
            }
        }

        if (!hasKey)
        {
            DoubleKeyData data = new DoubleKeyData();
            data.mainKey = key;
            data.Add(key, lan, text);
            dataList.Add(data);
        }
    }

    public string GetData(string key, string lan)
    {
        if (!ContainsKey(key))
            return "";

        for (int i = 0; i < dataList.Count; ++i)
        {
            if (dataList[i].mainKey == key)
            {
                if (!dataList[i].ContainsKey(lan))
                    return "";

                return dataList[i].GetValue(lan);
            }
        }

        return "";
    }

    public void RemoveData(string key)
    {
        for (int i = 0; i < dataList.Count; ++i)
        {
            if (dataList[i].mainKey == key)
            {
                dataList[i].RemoveData();
                dataList.RemoveAt(i);
                break;
            }
        }
    }

    public void OnGUI()
    {
        
    }
}

[System.Serializable]
public class KeyData
{
    public string key;
    public string value;
}

[System.Serializable]
public class DoubleKeyData
{
    public string mainKey;
    public List<KeyData> valueList = new List<KeyData>();

    public void Add(string key, string lan, string text)
    {
        mainKey = key;

        bool hasKey = false;
        for(int i = 0; i < valueList.Count; ++i)
        {
            if(valueList[i].key == lan)
            {
                valueList[i].value = text;
                hasKey = true;
                break;
            }
        }

        if (!hasKey)
        {
            KeyData data = new KeyData();
            data.key = lan;
            data.value = text;
            valueList.Add(data);
        }
    }

    public bool ContainsKey(string key)
    {
        for (int i = 0; i < valueList.Count; ++i)
        {
            if (valueList[i].key == key)
            {
                return true;
            }
        }

        return false;
    }

    public string GetValue(string key)
    {
        for (int i = 0; i < valueList.Count; ++i)
        {
            if (valueList[i].key == key)
            {
                return valueList[i].value;
            }
        }

        return "";
    }

    public void RemoveData()
    {
        valueList.Clear();
    }
}
