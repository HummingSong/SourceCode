using UnityEngine;
using System.Collections;
using Assets.Scripts.Cores;
using System;

public class LobbyPage : MonoBehaviour 
{
    // 페이지에 못들어오게 막는 변수
    public bool pageLock = false;

    // 페이지에 생성 확인 변수
    [HideInInspector]
    public bool pageOpen = false;
    
    public ELobbyPage pageName;

    public ELobbyPage eBackStepPage = ELobbyPage.None;
    public ELobbyPage eNextStepPage = ELobbyPage.None;

    // 서브 백 스텝 페이지 개념을 없애자!!
    // 큐를 이용해 언도 스텝으로 들어온 페이지를 LobbyUI 큐에 저장하고 정상 스텝으로 들어온 페이지는 패스.

    public virtual bool CanChangePage()
    {
        return true;
    }

    public virtual void EnterPage()
    {
        Core.Presenter.Get</******/>().SetCurrentPage(pageName);
    }

    public virtual void EnterPage(int gateWay)
    {
        Core.Presenter.Get</******/>().SetCurrentPage(pageName);
    }

    public virtual void ClosePage()
    {
        if (GetComponent<TweenHelper>() != null)
            GetComponent<TweenHelper>().Reset();
    }

    public virtual void UpdatePage()
    {
    }

    public virtual void ChangeTargetPage()
    {
        LobbyUI.instance.ChangeTargetPage(eNextStepPage);
    }

    public virtual void ChangeNextStepPage()
    {
        LobbyUI.instance.ChangeTargetPage(eNextStepPage);
    }
}
