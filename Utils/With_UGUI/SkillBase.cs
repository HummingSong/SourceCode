using UnityEngine;
using System;
using Assets.Scripts.User;
using Assets.Scripts.Utility;
using Assets.Scripts.Cores;
using System.Collections;
using System.Collections.Generic;


public abstract class SkillBase : LobbyPage
{
	// 장착 스킬 UI
	[Serializable]
	public struct UI_SkillEquipData
	{
		public UISprite uiImage;
		public UILabel uiCount;
		public GameObject objBack;
		public GameObject objOpen;
        public UISprite uiMoneyIcon;
		public UILabel uiMoney;
	
		public void Init()
		{
			uiImage.spriteName = string.Empty;
			uiCount.text = string.Empty;
			objBack.SetActive( true );
			objOpen.SetActive( false );
			uiMoney.text = "0";
		}
	}
	public UI_SkillEquipData[] uiSkillEquipSlot;

	// 스킬 리스트 스크롤
	public GameObject objSkillGrid;

	// 스킬 저장용 데이터
	// 기존과 달리 페이지 별로 가지고 있는것이 아니라 EBattleMode 별로 static으로 가지고 있게 하였습니다.
	// 그 이유는, 서버 저장은 모드별로 되고, 2개 이상의 페이지가 하나의 데이터를 공유할 수 있기 때문입니다.
	public struct UseSkillData
	{
		public string itemCode;
		public int skillCount;

		public void Init() { SetData( string.Empty, 0 ); }

		public void SetData( string code, int count )
		{
			itemCode = code;
			skillCount = count;
		}
	}
	protected static List<UseSkillData[]> useSkillDataList = new List<UseSkillData[]>();
	public bool CheckEnablePlay_and_RefreshUI { get; set; }


	protected abstract int GetModeTypeIndex();

	// IsValidSkillPage()가 false가 나올 경우 동작 하지 않음
	// SkillBase를 상속 받더라도 툴에서 연결을 안할 경우 LobbyPage를 상속 받은것과 같이 적용됩니다.
	protected bool IsValidSkillPage()
	{
		if( uiSkillEquipSlot == null || uiSkillEquipSlot.Length <= 0 )
			return false;

		if( objSkillGrid == null )
			return false;

		return true;
	}

	protected UseSkillData[] GetSkillData( int n )
	{
		try { return useSkillDataList[n]; }
		catch { return null; }
	}

	protected UseSkillData[] GetSkillData()
	{
		return GetSkillData( GetModeTypeIndex() );
	}

	protected virtual void Awake()
	{
		if( useSkillDataList.Count <= 0 )
		{
			for( int i = 0; i < (int)EBattleMode.Count; i++ )
			{
				var data = new UseSkillData[GLOBAL.MAX_SKILL_COUNT];

				useSkillDataList.Add( data );
			}
		}
	}

	// ClosePage
	public override void ClosePage()
	{
		GlobalUI.instance.PopUpClick( EPopUpMenu.PopAlphaStop );
        base.ClosePage();
        StartCoroutine( OnSend_EquipSkill( ( bool bSuccess ) =>
		{
			GlobalUI.instance.PopUpClose();
		} ) );
	}

	// ChangeTargetPage
	public override void ChangeTargetPage()
	{
		GlobalUI.instance.PopUpClick( EPopUpMenu.PopAlphaStop );

		StartCoroutine( OnSend_EquipSkill( ( bool bSuccess ) =>
		{
			GlobalUI.instance.PopUpClose();
			LobbyUI.instance.ChangeTargetPage( eNextStepPage );
		} ) );
	}

	// ChangeQueuePage
	public void ChangeQueuePage( ELobbyPage pageEnum )
	{
		GlobalUI.instance.PopUpClick( EPopUpMenu.PopAlphaStop );

		StartCoroutine( OnSend_EquipSkill( ( bool bSuccess ) =>
		{
			GlobalUI.instance.PopUpClose();
			LobbyUI.instance.ChangeQueuePage( pageEnum );
		} ) );
	}

	public override void UpdatePage()
	{
		if( !IsValidSkillPage() )
			return;

		UpdateLocalSkillData();
		RefreshUI_SkillInvenList();
		RefreshUI_SkillSlot();
	}

	protected virtual void RefreshUI() {}

	public virtual IEnumerator OnSend_EquipSkill( Action<bool> onComplete )
	{
		var skillData = GetSkillData();
		int modeType = GetModeTypeIndex();

		if( !IsValidSkillPage() || skillData == null || modeType == 0 )
		{
			onComplete( true );
			yield break;
		}

		bool isSuccess = false;

		yield return StartCoroutine( LobbyUI.instance.EquipSkill( modeType, skillData[0].itemCode, skillData[1].itemCode, string.Empty, ( bool bSuccess ) =>
		{
			isSuccess = bSuccess;
			if( !bSuccess )
			{
				#region 에러 로그
#if UNITY_EDITOR

				Console.LogWarning( "ERROR : OnSend_EquipSkill - EquipSkill Failed : " + ((EBattleMode)(modeType - 1)).ToString() );
#endif
				#endregion
			}

			onComplete( isSuccess );
		} ) );
	}

	// 로컬데이터를 UI로 적용
	protected void RefreshUI_SkillSlot()
	{
		var skillData = GetSkillData();
		if( !IsValidSkillPage() || skillData == null )
			return;

		for( int i = 0; i < GLOBAL.MAX_SKILL_COUNT; i++ )
		{
			if( i >= uiSkillEquipSlot.Length )
				break;

			uiSkillEquipSlot[i].Init();

			dfItemInfo itemInfo = Core.Presenter.Get<GameFactory>().GetItemInfo( skillData[i].itemCode );
          
			if( itemInfo != null )
			{
                dfShopInfo shopinfo = Core.Presenter.Get<SFManager>().GetShopInfo(itemInfo.iFactoryIndex);

				uiSkillEquipSlot[i].uiImage.spriteName = itemInfo.itemimage;
				uiSkillEquipSlot[i].uiCount.text = skillData[i].skillCount.ToString();
				uiSkillEquipSlot[i].objBack.SetActive( false );
				uiSkillEquipSlot[i].objOpen.SetActive( true );
                uiSkillEquipSlot[i].uiMoneyIcon.spriteName = SelectMoneyIcon(shopinfo.iSellType);
                if (shopinfo.iSellType == 1)
				    uiSkillEquipSlot[i].uiMoney.text = itemInfo.gold_price_s.ToString();
                else if(shopinfo.iSellType == 2)
				    uiSkillEquipSlot[i].uiMoney.text = itemInfo.jewel_price_s.ToString();
			}
		}
	}

	// SetSkillInventory
	protected void RefreshUI_SkillInvenList()
	{
		int modeType = GetModeTypeIndex();

		if( !IsValidSkillPage() || modeType == 0 )
			return;

		var lobbyGrid = objSkillGrid.GetComponent<LobbyGrid>();
		lobbyGrid.DeleteAllChild();

		var uiGrid = objSkillGrid.GetComponent<UIGrid>();
		uiGrid.transform.DetachChildren();

		dfModeInfo mode = Core.Presenter.Get<GameFactory>().GetFactoryItem<dfModeInfo>( modeType );
		if( mode != null )
		{
			for( int i = 0; i < mode.limitItemList.Length; i++ )
			{
				dfItemInfo itemInfo = Core.Presenter.Get<GameFactory>().GetItemInfo( mode.limitItemList[i].ItemCode );
				lobbyGrid.AddSkillItem( itemInfo );
			}
		}

		uiGrid.Reposition();
		uiGrid.transform.parent.gameObject.GetComponent<UIScrollView>().ResetPosition();
	}

	// 실 데이터를 로컬 데이타로 옮긴다.
	protected void InitSkillData()
	{
		var skillData = GetSkillData();
		int modeType = GetModeTypeIndex();

		if( !IsValidSkillPage() || skillData == null || modeType == 0 )
			return;

		for( int i = 0; i < skillData.Length; i++ )
			skillData[i].Init();

		var user = Core.Singleton.Get<GameRegistry>().GetLobbyPlayer( UserSlotType.Master );
		EquipItemSkill data = user.EquipData.GetItemSkillData( GetModeTypeIndex() );

		if( data == null )
		{
			Console.LogWarning( "SkillBase : InitSkillData() : Skill Data is Invalid : " + ((EBattleMode)(modeType - 1)).ToString() );
			return;
		}

		user.InitUserItemSkill( Core.Singleton.Get<GameRegistry>().User, modeType );

		for( int i = 0; i < skillData.Length; i++ )
		{
			skillData[i].skillCount = data.GetSkillCount(i);
			skillData[i].itemCode = data.GetSkillCode(i);
		}
	}

	public bool IsInEquipSlot( string itemcode )
	{
		var skillData = GetSkillData();
		if( !IsValidSkillPage() || skillData == null )
			return false;

		for( int i = 0; i < skillData.Length; i++ )
		{
			if( skillData[i].itemCode == itemcode )
				return true;
		}

		return false;
	}

    public bool IsEquipSkillCanUse()
    {
        var skillData = GetSkillData();
		if( !IsValidSkillPage() || skillData == null )
			return true;

        for (int i = 0; i < skillData.Length; i++)
        {
            if ( !string.IsNullOrEmpty( skillData[i].itemCode ) && skillData[i].skillCount == 0)
                return false;
        }

        return true;
    }

	// 슬롯 2개 모두 찼는지 체크
	public bool IsEquipSlotFull()
	{
		var skillData = GetSkillData();
		if( !IsValidSkillPage() || skillData == null )
			return true;

		for( int i = 0; i < skillData.Length; i++ )
		{
			if( string.IsNullOrEmpty( skillData[i].itemCode ) )
				return false;
		}

		return true;
	}

	// 슬롯 모두 비었는지 체크
	public bool IsEquipSlotEmpty()
	{
		var skillData = GetSkillData();
		if( !IsValidSkillPage() || skillData == null )
			return true;

		for( int i = 0; i < skillData.Length; i++ )
		{
			if( !string.IsNullOrEmpty( skillData[i].itemCode ) && skillData[i].skillCount > 0 )
				return false;
		}

		return true;
	}

	protected void UpdateLocalSkillData()
	{
		var skillData = GetSkillData();
		int modeType = GetModeTypeIndex();

		if( !IsValidSkillPage() || skillData == null || modeType == 0 )
			return;

		dfModeInfo modeInfo = Core.Presenter.Get<GameFactory>().GetFactoryItem<dfModeInfo>( modeType );
		if( modeInfo != null )
		{
			for( int i = 0; i < skillData.Length; i++ )
			{
				if( string.IsNullOrEmpty( skillData[i].itemCode ) )
					continue;

				ItemInven itemInven = Core.Singleton.Get<GameRegistry>().User.GetItemFromItemcode( skillData[i].itemCode );
				if( itemInven == null )
					continue;

				int limitCnt = modeInfo.GetLimitItemCount( skillData[i].itemCode );

				if( itemInven.use_cnt > limitCnt )
					skillData[i].skillCount = limitCnt;
				else
					skillData[i].skillCount = itemInven.use_cnt;
			}
		}
	}

	public void InitSkillSlot( int n )
	{
		var skillData = GetSkillData();

		try { skillData[n].Init(); }
		catch {}
	}

	public void ClickSkillBtn( string itemCode )
	{
		var skillData = GetSkillData();
		int modeType = GetModeTypeIndex();

		if( !IsValidSkillPage() || skillData == null || modeType == 0 )
			return;

		// 튜토리얼 일때만 쓰는 부분
		if( Core.Presenter.Get<SFManager>().bPlayTutorial && PopTutorial.instance != null )
		{
			PopTutorial.instance.CheckBtnEventProcess();
		}

		int newIndex = 0;

		// 스킬 중복 검사 : 중복된 코드가 있으면 해제해야한다.
		for( int i = 0; i < skillData.Length; i++ )
		{
			if( itemCode == skillData[i].itemCode )
			{
				newIndex = i;
				InitSkillSlot(i);
				RefreshUI_SkillSlot();
				return;
			}
		}

		bool emptySlot = false;

		// 장착이 가능한 자리가 있는지 한번 더 검사해야 한다.
		for( int i = 0; i < skillData.Length; i++ )
		{
			if( string.IsNullOrEmpty( skillData[i].itemCode ) )
			{
				newIndex = i;
				emptySlot = true;
				break;
			}
		}

		if( emptySlot )
		{
			dfModeInfo modeInfo = Core.Presenter.Get<GameFactory>().GetFactoryItem<dfModeInfo>( modeType );
			if( modeInfo != null )
			{
				int limitCount = modeInfo.GetLimitItemCount( itemCode );
				ItemInven itemInven = Core.Singleton.Get<GameRegistry>().User.GetItemFromItemcode( itemCode );

				skillData[newIndex].itemCode = itemCode;

				if( itemInven == null )
				{
					skillData[newIndex].skillCount = 0;
				}
				else
				{
					if( itemInven.use_cnt > limitCount )
						skillData[newIndex].skillCount = limitCount;
					else
						skillData[newIndex].skillCount = itemInven.use_cnt;
				}
			}
		}

		RefreshUI_SkillSlot();
	}

	public void OnClick_EquipSlot( int n )
	{
		if( n >= GLOBAL.MAX_SKILL_COUNT )
			return;

		InitSkillSlot(n);

		RefreshUI_SkillSlot();
		RefreshUI_SkillInvenList();

		CheckEnablePlay_and_RefreshUI = true;
	}

	// 구매 버튼 누름
	public void OnClick_BuySkillItem( int n )
	{
		if( n >= GLOBAL.MAX_SKILL_COUNT )
			return;

		var skillData = GetSkillData();
		if( !IsValidSkillPage() || skillData == null )
			return;

		try
		{
			if( string.IsNullOrEmpty( skillData[n].itemCode ) )
				return;

			bool result = Util.IsCanBuyItem( skillData[n].itemCode, 1 );
			if( !result )
				return;

			dfItemInfo itemInfo = Core.Presenter.Get<GameFactory>().GetItemInfo( skillData[n].itemCode );

			dfShopInfo data = Core.Presenter.Get<SFManager>().GetShopInfo( itemInfo.iFactoryIndex );

			Core.Presenter.Get<SFManager>().SetBuyItemId( data, itemInfo.iFactoryIndex, data.iItemCount );

			GlobalUI.instance.PopUpClick( EPopUpMenu.PopBuy );
		}
		catch {}
	}

    // 다른 상점 아이콘들은 사전 오브젝트로 미리 제작되었지만, 이 부분은 후반부 요청에 따라 이부분은 리소스 이름으로 직접 변경한다.
    string SelectMoneyIcon(int selltype)
    {
        switch (selltype)
        {
            case 1:
                return "*****";
            case 2:
                return "*****";
            case 3:
                return "*****";
            case 4:
                return "*****";
            default:
                return "*****";
        }
    }
}
