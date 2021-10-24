/// <summary>
/// Control Type 에 따른 스크립트
/// ActionButton을 관리하며
/// 이 스크립트는 GameUI가 관리한다.
/// GameUI > ControlType > ActionButton
/// 
/// 작성자 : 배정욱
/// 작성일 : 2014.11.19
/// 
/// </summary>


using UnityEngine;
using System.Collections;
using Assets.Scripts.Cores;

public class ControlType : MonoBehaviour {

	// 버튼 등록 ( 이동, 슛 등등 )

	// ActionButton을 적용할 경우 press상태를 쉽게 가져올 수 있지만 다른 상태를 가져올 수 없다.
	// 공용으로 UIButton 자체를 다 가져와야 하나...
	// 일단 사용범위가 정해지지 않았으니 UIButton으로 선언

	public UIButton[]		btnMove;
    public GameObject[]     objMoveEff;

	public UIButton			btnFire;

	private bool			bMovePress = false;
	private bool			bFireEffectOn = false;
	public GameObject[] 	objFireEffect;

    public GameObject       objReload;

	void Awake ()
	{
		bFireEffectOn = false;

		for ( int i = 0 ; i < objFireEffect.Length ; ++i )
		{
			objFireEffect[i].SetActive ( false );
		}
	}

	void Update ()
	{
		FireBtnDownEffect ();
		MoveBtnDownEffect ();
	}

	void FireBtnDownEffect ()
	{
		if ( objFireEffect[GameUI.instance.iSelectWeaponIndex] == null )
			return;

		if ( bFireEffectOn != CheckButton ( btnFire.state ) )
		{
			bFireEffectOn = CheckButton ( btnFire.state );
		
			objFireEffect[GameUI.instance.iSelectWeaponIndex].SetActive ( bFireEffectOn );
		}
	}

	void MoveBtnDownEffect ()
	{	
		bool press = false;

		for ( int i = 0 ; i < btnMove.Length ; ++i )
		{
			press = CheckButton ( btnMove[i].state );
			if ( press )
				break;
		}

		if ( press != bMovePress )
		{
			bMovePress = press;

            for (int i = 0; i < btnMove.Length; ++i)
            {
                btnMove[i].gameObject.GetComponentInChildren<TweenAlpha>().PlayForward = !bMovePress;
            }
		}

	}

	public void MoveBtnOnOff ( int index, bool set )
	{
        if (!btnMove[index].gameObject.transform.parent.gameObject.activeSelf)
            return;

		if ( set )
		{
			btnMove [index].state = UIButton.State.Normal;
			btnMove [index].enabled = true;

            btnMove[index].gameObject.GetComponent<TweenHelper>().Reset();
            btnMove [index].gameObject.GetComponentInChildren<TweenScale>().PlayForward(); 

            SyncTween(index);
		}
		else
		{
			btnMove [index].state = UIButton.State.Disabled;
			btnMove [index].enabled = false;
            btnMove [index].gameObject.GetComponent<TweenHelper>().StopAndInit();
		}

        objMoveEff[index].SetActive(set);

        if (btnMove[index].gameObject.GetComponentInChildren<TweenAlpha>() == null) return;
        btnMove[index].gameObject.GetComponentInChildren<TweenAlpha>().enabled = set;

        if (btnMove[index].gameObject.GetComponentInChildren<TweenScale>() == null) return;
        btnMove[index].gameObject.GetComponentInChildren<TweenScale>().enabled = set;
    }

    void SyncTween(int index)
    {
        float factor = btnMove[index].gameObject.GetComponentInChildren<TweenAlpha>().tweenFactor;

        for ( int i = 0 ;i < btnMove.Length; ++ i)
        {
            if ( btnMove[i].enabled)
            {
                btnMove[index].gameObject.GetComponent<TweenHelper>().PlayForward();
            }
        }
        
    }

	bool CheckButton(UIButtonColor.State state)
	{	
		switch (state) 
		{
		case UIButtonColor.State.Pressed :
			return true;
		default:
			break;
		}
		
		return false;
	}

}
