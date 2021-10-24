using UnityEngine;


[System.Serializable]
public enum EInputCommand
{
	Move,
	Fire1,
	Fire2,
	Reload,
	ChangeWeapon,
	UseItem,
	AxisRotation,
	LockOnSwipe,
	LockOnTab,
	AxisMove,
	CoverToggle,
	SkillUse,
	Count,
	None
};

[System.Serializable]
public enum EButtonState
{
	Off,
	Down,
	Press,
	Release,
};

[System.Serializable]
public enum EInputType
{
	Button,
	Axis,
};

[System.Serializable]
public class InputValue
{
	public EInputCommand eCommand;

	public EInputType eInputType = EInputType.Button;

	[HideInInspector]
	public bool bOn = false;

	public string strFsmCommand;

	public EButtonState eButtonConditionOn = EButtonState.Press;

	[HideInInspector]
	public EButtonState eButtonState = EButtonState.Off;

	[HideInInspector]
	public bool bButton;

	[HideInInspector]
	public bool bButtonPrev;

	public float fValue;

	public int iValue;

	public Vector2 vAxis;

	public GameObject objTarget;

	public void Clear()
	{
		bButtonPrev = bButton;
		bOn = false;
		bButton = false;
		fValue = 0.0f;
		iValue = 0;
		vAxis = Vector2.zero;
		objTarget = null;
	}
};


public class GameInput : MonoBehaviour
{
	public InputValue[] Inputs = new InputValue[(int)EInputCommand.Count];

	[HideInInspector]
	public bool bClearActionButton = false;

	[HideInInspector]
	public bool bTutorial = false;

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		UpdateInput( Time.deltaTime );

		if( bClearActionButton )
			ClearActionButton();
	}

	protected virtual void UpdateInput( float timeDelta )
	{
		if( bTutorial )
		{
			ClearActionButton();
		}
	}

	public bool GetInputBool( EInputCommand eCmd )
	{
		if( eCmd == EInputCommand.Count )
			return false;

		return Inputs[(int)eCmd].bButton;
	}

	public bool GetButton( EInputCommand eCmd )
	{
		if( eCmd == EInputCommand.Count )
			return false;

		return Inputs[(int)eCmd].bOn;
	}

	public float GetInputFloat( EInputCommand eCmd )
	{
		if( eCmd == EInputCommand.Count )
			return 0.0f;

		return Inputs[(int)eCmd].fValue;
	}

	public Vector2 GetInputAxis( EInputCommand eCmd )
	{
		if( eCmd == EInputCommand.Count )
			return Vector2.zero;

		return Inputs[(int)eCmd].vAxis;
	}

	public GameObject GetInputObject( EInputCommand eCmd )
	{
		if( eCmd == EInputCommand.Count )
			return null;

		return Inputs[(int)eCmd].objTarget;
	}

	public virtual void SetInputSensitivity( float value )
	{
	}

	public virtual void SetInputAddSensitivity( float value )
	{
	}

	public void ClearActionButton()
	{
		Inputs[(int)EInputCommand.Fire1].Clear();
		Inputs[(int)EInputCommand.ChangeWeapon].Clear();
		Inputs[(int)EInputCommand.SkillUse].Clear();
		Inputs[(int)EInputCommand.Move].Clear();
	}
}
