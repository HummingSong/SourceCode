using UnityEngine;
using System.Collections;

public class SetResolutionUI : MonoBehaviour {

	public UISprite sprite;

	public float fConstWidth;

	void Awake ()
	{
		int height = (Screen.height * (int)fConstWidth) / Screen.width;
		sprite.height = height;
	}

}
