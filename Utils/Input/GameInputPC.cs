using Assets.Scripts.Cores;
using UnityEngine;


public class GameInputPC : GameInput
{
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

        // update axis input
        float mousex = Input.GetAxis( "Mouse X" );
		float mousey = -1.0f * Input.GetAxis( "Mouse Y" );

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

		// move axis
		float movefb = (Input.GetKey( KeyCode.W ) ? 1.0f : 0.0f) + (Input.GetKey( KeyCode.S ) ? -1.0f : 0.0f);
		float movelr = (Input.GetKey( KeyCode.A ) ? -1.0f : 0.0f) + (Input.GetKey( KeyCode.D ) ? 1.0f : 0.0f);

		Inputs[(int)EInputCommand.AxisMove].bOn = true;
		Inputs[(int)EInputCommand.AxisMove].vAxis = new Vector2( movelr, movefb );

		if( Input.GetKeyDown( KeyCode.Space ) == true )
		{
			Inputs[(int)EInputCommand.CoverToggle].bOn = true;
		}
		else
		{
			Inputs[(int)EInputCommand.CoverToggle].bOn = false;
		}

		CheckButton( ref Inputs[(int)EInputCommand.Fire1], "Fire1" );
		CheckButton( ref Inputs[(int)EInputCommand.Reload], "Reload" );
		CheckButton( ref Inputs[(int)EInputCommand.ChangeWeapon], "ChangeWeapon" );
		CheckButton( ref Inputs[(int)EInputCommand.LockOnTab], "LockOnTab" );

		if( Input.GetKeyDown( KeyCode.C ) == true )
		{
			Inputs[(int)EInputCommand.ChangeWeapon].bOn = true;
		}

		if( Input.GetKeyUp( KeyCode.V ) == true )
		{
			Inputs[(int)EInputCommand.LockOnSwipe].bOn = true;
			Inputs[(int)EInputCommand.LockOnSwipe].vAxis.x = -1.0f;
			Inputs[(int)EInputCommand.LockOnTab].bOn = false;
		}
		else if( Input.GetKeyUp( KeyCode.B ) == true )
		{
			Inputs[(int)EInputCommand.LockOnSwipe].bOn = true;
			Inputs[(int)EInputCommand.LockOnSwipe].vAxis.x = 1.0f;
			Inputs[(int)EInputCommand.LockOnTab].bOn = false;
		}

		if( Input.GetKeyDown( KeyCode.W ) == true )
		{
			Inputs[(int)EInputCommand.Move].bOn = true;
			Inputs[(int)EInputCommand.Move].iValue = (int)EWayPointLinkDir.Forward;
		}
		else if( Input.GetKeyDown( KeyCode.A ) == true )
		{
			Inputs[(int)EInputCommand.Move].bOn = true;
			Inputs[(int)EInputCommand.Move].iValue = (int)EWayPointLinkDir.Left;
		}
		else if( Input.GetKeyDown( KeyCode.D ) == true )
		{
			Inputs[(int)EInputCommand.Move].bOn = true;
			Inputs[(int)EInputCommand.Move].iValue = (int)EWayPointLinkDir.Right;
		}
		else if( Input.GetKeyDown( KeyCode.S ) == true )
		{
			Inputs[(int)EInputCommand.Move].bOn = true;
			Inputs[(int)EInputCommand.Move].iValue = (int)EWayPointLinkDir.BackWard;
		}

		if( Input.GetKeyDown( KeyCode.Alpha1 ) == true )
		{
			Inputs[(int)EInputCommand.SkillUse].bOn = true;
			Inputs[(int)EInputCommand.SkillUse].iValue = 0;
		}
		else if( Input.GetKeyDown( KeyCode.Alpha2 ) == true )
		{
			Inputs[(int)EInputCommand.SkillUse].bOn = true;
			Inputs[(int)EInputCommand.SkillUse].iValue = 1;
		}
		// [ypqp35 2016/08/09] GunSkill
		else if( Input.GetKeyDown( KeyCode.Alpha3 ) == true )
		{
			try
			{
				CharacterHuman myCharacter = Core.Singleton.Get<GameRegistry>().Mycharacter as CharacterHuman;
				if( myCharacter.gunSkillManager.IsEnableGunSkill() )
				{
					Inputs[(int)EInputCommand.SkillUse].bOn = true;
					Inputs[(int)EInputCommand.SkillUse].iValue = GLOBAL.GUNSKILL_INDEX;
				}
			}
			catch {}
		}

		if( Input.GetKeyDown( KeyCode.Tab ) )
		{
			GameUI.instance.ShowGameUI_ScoreBoard();
		}
		else if( Input.GetKeyUp( KeyCode.Tab ) )
		{
			GameUI.instance.HideGameUI_ScoreBoard();
		}

		base.UpdateInput( timeDelta );
	}

	bool CheckButton( ref InputValue value, string strInputName )
	{
		switch( value.eButtonConditionOn )
		{
		case EButtonState.Down:
			value.bOn = Input.GetButtonDown( strInputName );
			break;

		case EButtonState.Press:
			value.bOn = Input.GetButton( strInputName );
			break;

		case EButtonState.Release:
			value.bOn = Input.GetButtonUp( strInputName );
			break;
		}

		return value.bOn;
	}
}
