using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using DG.Tweening;

public class LoadingUI : MonoBehaviour
{
    public static LoadingUI instance = null;

    public GameObject objLoading;
    public Image uiLoadingGage;

    public GameObject objTipText;
    public Text tipText;

    public float timer = 0f;

    public void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        Init();
    }

    public void Init()
    {
        SetLoadingGage(0);
    }

    public void SetLoadingGage(float gage)
    {
        uiLoadingGage.fillAmount = gage;
    }
}
