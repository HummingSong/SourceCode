using Assets.Scripts.Cores;
using UnityEngine;


public class GameInputMobile : GameInput
{
	private TouchInput touch;

	private bool bCanChaneWeapon = true;
	private float fChangeWeaponTimeInterval = 1.0f;
	private float fChangeWeaponTimer = 0.0f;

    private bool bFirstAimTouch = false;

	void Awake()
	{
		touch = GetComponent<TouchInput>();
	}

	// Update is called once per frame
	protected override void UpdateInput( float timeDelta )
	{
		for( int i = 0; i < Inputs.Length; i++ )
		{
			Inputs[i].Clear();
		}

        if (!Assets.Scripts.Cores.Core.Process.PlayRunning)
        {
            base.UpdateInput(timeDelta);
            return;
        }

        //Debug.Log(touch.bTouchCurrent);
        if (touch.bTouchCurrent)
        {
            if(!bFirstAimTouch)
            {
                bFirstAimTouch = true;
                GameUI.instance.SetAimGuide(touch.touchPosition, true);
            }
        }
        else
        {
            bFirstAimTouch = false;
            GameUI.instance.SetAimGuide(touch.touchPosition, false);
        }

        // update axis input
        float mousex = touch.vPosition.x;
		float mousey = -1.0f * touch.vPosition.y;

		Vector2 axis = new Vector2( mousex, mousey );

		if( Inputs[(int)EInputCommand.AxisRotation].vAxis != axis )
		{
			Inputs[(int)EInputCommand.AxisRotation].bOn = true;
			Inputs[(int)EInputCommand.AxisRotation].vAxis = axis;
		}
		else
		{
			Inputs[(int)EInputCommand.AxisRotation].bOn = false;
		}

        if (CheckButton(GameUI.instance.uiCoverBtn.state))
        {
            Inputs[(int)EInputCommand.CoverToggle].bOn = true;
        }

		// Fire1
		Inputs[(int)EInputCommand.Fire1].bOn = CheckButton( GameUI.instance.useType.btnFire.state );

		if( bCanChaneWeapon )
		{
			if( CheckButton( GameUI.instance.uiChangeWeapon.state ) )
			{
				Inputs[(int)EInputCommand.ChangeWeapon].bOn = true;
				bCanChaneWeapon = false;
			}
		}
		else
		{
			fChangeWeaponTimer += Time.deltaTime;
			if( fChangeWeaponTimer > fChangeWeaponTimeInterval )
			{
				bCanChaneWeapon = true;
				fChangeWeaponTimer = 0.0f;
			}
		}

		for( int i = 0; i < GameUI.instance.uiSkillSlot.Length; ++i )
		{
			if( CheckButton( GameUI.instance.uiSkillSlot[i].uiSkillBtn.state ) )
			{
				Inputs[(int)EInputCommand.SkillUse].bOn = true;
				Inputs[(int)EInputCommand.SkillUse].iValue = i;
			}
		}

		// [ypqp35 2016/08/09] GunSkill
		try
		{
			if( CheckButton( GameUI.instance.uiGunSkillSlot.btn_Normal.state ) )
			{
				CharacterHuman myCharacter = Core.Singleton.Get<GameRegistry>().Mycharacter as CharacterHuman;
				if( myCharacter.gunSkillManager.IsEnableGunSkill() )
				{
					Inputs[(int)EInputCommand.SkillUse].bOn = true;
					Inputs[(int)EInputCommand.SkillUse].iValue = GLOBAL.GUNSKILL_INDEX;
				}
			}
		} catch {}

		if( CheckButton( GameUI.instance.uiReloadBtn.state ) )
		{
			Inputs[(int)EInputCommand.Reload].bOn = true;
		}

		if( CheckButton( GameUI.instance.useType.btnMove[(int)EWayPointLinkDir.Forward].state ) )
		{
			Inputs[(int)EInputCommand.Move].bOn = true;
			Inputs[(int)EInputCommand.Move].iValue = (int)EWayPointLinkDir.Forward;
		}
		else if( CheckButton( GameUI.instance.useType.btnMove[(int)EWayPointLinkDir.Left].state ) )
		{
			Inputs[(int)EInputCommand.Move].bOn = true;
			Inputs[(int)EInputCommand.Move].iValue = (int)EWayPointLinkDir.Left;
		}
		else if( CheckButton( GameUI.instance.useType.btnMove[(int)EWayPointLinkDir.Right].state ) )
		{
			Inputs[(int)EInputCommand.Move].bOn = true;
			Inputs[(int)EInputCommand.Move].iValue = (int)EWayPointLinkDir.Right;
		}
		else if( CheckButton( GameUI.instance.useType.btnMove[(int)EWayPointLinkDir.BackWard].state ) )
		{
			Inputs[(int)EInputCommand.Move].bOn = true;
			Inputs[(int)EInputCommand.Move].iValue = (int)EWayPointLinkDir.BackWard;
		}

		base.UpdateInput( timeDelta );
	}

	bool CheckButton( UIButtonColor.State state )
	{
		switch( state )
		{
		case UIButtonColor.State.Pressed:
			return true;
		default:
			break;
		}

		return false;
	}

	public override void SetInputSensitivity( float value )
	{
		base.SetInputSensitivity( value );
		touch.fMouseSpeed = value;
	}

	public override void SetInputAddSensitivity( float value )
	{
		base.SetInputAddSensitivity( value );
		touch.fMouseAddSpeed = value;
	}
}
