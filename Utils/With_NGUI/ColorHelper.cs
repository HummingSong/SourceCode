/// <summary>
/// ColorHelper
/// 
/// 작성자 : 배정욱
/// 작성일 : 2015.2.
/// 
/// 가지고 있는 UI 색을 바꿀 떄 쓰는 스크립트. 일단은 Sprite 류만.. 
/// 후에 추가하거나 범용으로 수정하도록 한다. ( 단, 기획이 바뀐다는 전제하에 .. )
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class ColorHelper : MonoBehaviour {

	public UISprite[]	uiSprite;

	private Color 		myColor;

	public Color 		targetColor;

	void Awake () {
		if ( uiSprite.Length > 0 )
		{
			myColor = uiSprite[0].color;
		}
	}
	
	public void SetSpriteColor ( bool set )
	{
		for ( int i = 0 ; i < uiSprite.Length ; ++i )
		{
			if ( set )
			{
				uiSprite[i].color = targetColor;
			}
			else
			{
				uiSprite[i].color = myColor;
			}
		}
	}
}
