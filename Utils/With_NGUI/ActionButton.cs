/// InGame Button Script
/// 
/// 작성자 : 배정욱
/// 작성일 : 2014.11.19
/// 
/// UI 버튼의 프레스 상태 감지 후 값을 변경함.
/// </summary>

using UnityEngine;
using System.Collections;

public class ActionButton : MonoBehaviour {

	[HideInInspector]
	public bool bPress = false;

	[HideInInspector]
	public int iPressID = -1;

	void OnPress(bool isPressed)
	{
		iPressID = UICamera.currentTouchID;

		if ( isPressed )
		{
			bPress = true;

			if ( iPressID != -1 )
				GameUI.instance.AddFingerID(iPressID);
		}
		else
		{
			bPress = false;

			GameUI.instance.RemoveFingerID(iPressID);
			iPressID = -1;
		}
	}
}

