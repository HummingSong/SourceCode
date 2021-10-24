using UnityEngine;
using System.Collections;

public class OnOffButton : MonoBehaviour {

	public UILabel btnLabel;

	private bool bButtonPush = true;

	public string strOnText = "ON";
	public string strOffText = "OFF";

	void Start ()
	{
		UpdateButtonState ();
	}

	public void UpdateButtonState ()
	{
		if ( bButtonPush )
		{
			gameObject.GetComponent<UIButton>().state = UIButtonColor.State.Normal;

			if ( btnLabel != null )
				btnLabel.text = strOnText;
		}
		else
		{
			gameObject.GetComponent<UIButton>().state = UIButtonColor.State.Disabled;

			if ( btnLabel != null )
				btnLabel.text = strOffText;
		}
	}

	void Update ()
	{
		UpdateButtonState ();
	}

	public void OnStateChange ()
	{
		bButtonPush = !bButtonPush;

		UpdateButtonState ();
	}

	public void OnHover()
	{
		UpdateButtonState ();
	}
}
