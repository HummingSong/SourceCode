/// <summary>
/// DynamicHelper
/// 
/// 작성자 : 배정욱
/// 작성일 : 2015.1.
/// 
/// 화면안의 고정된 UI가 아닌 액티브한 UI를 컨트롤 하기 위해 만든 스크립트
/// 
/// </summary>

using UnityEngine;
using System.Collections;
using Assets.Scripts.Utility;


public class DynamicHelper : MonoBehaviour 
{
	private Transform targetTr;

	[HideInInspector]
    public Vector3 vOffset;

	private bool bReady = false; // 초기 정보 기반으로 셋팅 완료 여부

	public UISprite	uiGage;

	private UIWidget[] uiComp = null;

    // TartgetInfo
	public GameObject uiBg1;		// 아이콘(계급) 제외한 배경
	public GameObject uiBg2;        // 아이콘(계급) 포함한 배경
	public UILabel uiName;
    public UILabel uiLevel;
	public UISprite uiRank;
	public UISprite uiIcon;
    public UISprite uiIconOnly;    // 아이콘 정보만 있는 루트

    private bool bIsInsight = true;

    [HideInInspector]
    public bool bActive = true;

    public GameObject[] objTargetInfo;

    public enum ETargetInfoType
    {
        Aimed,
        Normal,
        None
    }

    // 아이콘이 없는 경우 텍스트의 위치를 옮겨달라 함. 기존의 좌표를 저장하고 0으로 바꾼다.
    private Vector3 vOriginPos;
	private Camera currentCamera;


	void Awake ()
	{
		if ( !bReady )
		{
            uiComp = gameObject.GetComponentsInChildren<UIWidget>();

            if (uiComp.Length > 0)
			{
                for (int i = 0; i < uiComp.Length; ++i)
				{
                    uiComp[i].enabled = false;
				}
			}
		}

	}

	// Update is called once per frame
	void Update () 
	{
		if ( !bReady )
			return;

		CoordinateUpdate ();
	}

	// NGUI의 좌표 변환을 위해 처음 생성 후 이 함수를 호출해 포지션을 먼저 잡고 활성화 시킨다.
	public void SetInfoForReady ( Transform tr, Vector3 offset )
	{
        // 따라다닐 타겟 지정
		targetTr = tr;

        // 위치 보정
        vOffset = offset;

        // 처음 위치 업데이트
		CoordinateUpdate ();

        // 준비 플래그를 On ( off이면 CoordinateUpdate() 를 안돔 )
		bReady = true;

        // UI 콤포넌트 전부 활성화
        if (uiComp != null)
		{
            for (int i = 0; i < uiComp.Length; ++i)
			{
                uiComp[i].enabled = true;
			}
		}

        // 새 애.
		// 죽었을 때도 없애야 하는데
        // 죽었때 플래그를 가져와야 한다. 현재 Tr로는 알 수가 없고 PvP에서 계속 Active 상태이니
        // 후에 값을 받던가 해야겄다.

        currentCamera = DynamicUI.instance.GetBaseCamera();
	}


	public void SetCurrentCamera(Camera cam)
	{
		currentCamera = cam;
	}


	// WORLD -> SCREEN -> NGUI 좌표로 변환
	public void CoordinateUpdate ()
	{
        //if (Camera.main == null)
		if(currentCamera == null)
        {
            //[iceme] - 멀티플레이시 currentCamera가 셋팅되지 못하여 적 Player의 ID나 HP의 좌표를 Update하지 못하므로 여기서 카메라 셋팅을 해준다.
            if (GameEngine.instance.currentGameinfo.currentGameCamera)
                SetCurrentCamera(GameEngine.instance.currentGameinfo.currentGameCamera.GetComponent<Camera>());

            return;
        }

        if ( targetTr == null )
            return;

		//Vector3 screenPos = Camera.main.WorldToScreenPoint (targetTr.position + vOffset);
		Vector3 screenPos = currentCamera.WorldToScreenPoint (targetTr.position + vOffset);

       
        if (bIsInsight != GetTargetInView(screenPos))
        {
            bIsInsight = GetTargetInView(screenPos);

            if (uiComp != null)
            {
                for (int i = 0; i < uiComp.Length; ++i)
                {
                    uiComp[i].enabled = bIsInsight;
                }
            }
        }

		screenPos = DynamicUI.instance.ConvertPosToUI (screenPos);

		transform.localPosition = screenPos;
  
	}

	// 게이지 형태의 UI 를 다루는 함수 ( HP 형의 UI )
	public void SetGageInfo ( float current, float max )
	{
		if ( uiGage == null )
			return;

		float percent = current / max;
		
        if ( percent < 0.0f )
			percent = 0.0f;

		uiGage.fillAmount = percent;

        if ( current <= 0 )
        {
            if (!Util.IsPVPModePlay())
            {
                bActive = false;
                GetComponent<TweenHelper>().Reset();

                Invoke("DeActivate", 1.0f);
            }
        }
	}

    // 인보크용 함수
    public void DeActivate()
    {
        gameObject.SetActive(false);
    }

	// 타겟 인포 전용
	public void SetTargetInfo( int level, int rank, string nickName, string iconName )
	{
		try
		{
			uiRank.spriteName = string.Empty;
			uiIcon.spriteName = string.Empty;

			// PvP 모드에서는 계급 표시
			if( Util.IsPVPModePlay() )
			{
				uiRank.spriteName = UIUtil.GetCurrRankIcon( rank );

				uiLevel.gameObject.SetActive( false );
				uiRank.gameObject.SetActive( true );

				// 계급과 스킬아이콘은 같은 자리에 나오기 때문에 하나만 출력
				iconName = string.Empty;
			}
			// 그 외 모드에서는 기존 그대로 레벨 표시
			else
			{
				uiLevel.text = "LV." + level.ToString();

				uiLevel.gameObject.SetActive( true );
				uiRank.gameObject.SetActive( false );
			}

			uiName.text = nickName;
			uiIcon.spriteName = iconName;
			uiIconOnly.spriteName = iconName;

			for( int i = 0; i < objTargetInfo.Length; ++i )
				objTargetInfo[i].SetActive( false );

			// 계급 또는 스킬아이콘 모두 비었다면 단순한 배경 사용
			bool bEmpty = string.IsNullOrEmpty( uiRank.spriteName ) && string.IsNullOrEmpty( uiIcon.spriteName );

			uiBg1.SetActive( bEmpty );
			uiBg2.SetActive( !bEmpty );
		}
		catch {}
	}

	// Aim이 되어있을 때와 아닐 때를 구분하는 함수.
	public void SetViewInfo(bool bAimed)
    {
        for (int i = 0; i < objTargetInfo.Length; ++i)
            objTargetInfo[i].SetActive(false);

		if( bAimed )
		{
			objTargetInfo[(int)ETargetInfoType.Aimed].SetActive( true );
		}
		else
		{
			objTargetInfo[(int)ETargetInfoType.Normal].SetActive( true );
		}
    }

    // 멀티 전용
    public void SetTargetName(string name)
    {
		if( uiName != null )
			uiName.text = name;
	}

	bool GetTargetInView(Vector3 screenPos)
    {
        int width = GameUI.instance.GetComponent<UIRoot>().manualWidth;
        int height = GameUI.instance.GetComponent<UIRoot>().manualHeight;

        // 스크린 포즈의 값 중 하나라도 - 값이 되면 스크린 밖에 있다는 말이다.
        if (screenPos.x <= 0 || screenPos.x >= (float)width || screenPos.z <= 0)
            return false;

        // 스크린 포즈의 값 중 하나라도 - 값이 되면 스크린 밖에 있다는 말이다.
        if (screenPos.y <= 0 || screenPos.y >= (float)height || screenPos.z <= 0)
            return false;

        return true;
    }
}
