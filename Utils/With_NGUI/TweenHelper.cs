/// <summary>
/// TweenHelper
/// 
/// 작성자 : 배정욱
/// 작성일 : 2015.1.
/// 
/// + 2014.2.17
///  Panel 연출 리셋 스크립트와 통합
/// 
/// 가지고 있는 UI 연출을 관리하는 스크립트 ( 팝업류가 아닐 때 쓰기 위해 제작 )
/// 모두 리셋할 필요난 없으니 사전에 등록해둔다.
/// 
/// </summary>


using UnityEngine;
using System.Collections;

public class TweenHelper : MonoBehaviour {
	
	public GameObject[] objTweenForReset;

	void Awake () 
	{
		// 팝업 외의 클릭, 터치가 안되게 컬리더 사이즈를 스크린만큼 늘려준다.
		if ( gameObject.GetComponent<UIPanel> () != null )
		{
            if (GetComponent<BoxCollider>() != null)
            {
                BoxCollider col = GetComponent<BoxCollider>();
                Vector3 size = Vector3.zero;

                size.x = (float)Screen.width;
                size.y = (float)Screen.height;
                col.size = size;
            }
		}
	}

	public void Reset ()
	{
		if( objTweenForReset == null )
			return;
		
		for (int i = 0; i < objTweenForReset.Length; ++i) 
		{
			if( objTweenForReset[i] == null )
				continue;

			UITweener[] tween = objTweenForReset[i].GetComponents < UITweener > ();
			if( tween == null )
				continue;

			for( int j = 0; j < tween.Length; ++j) 
			{
				if( tween[j] == null )
					continue;

				tween[j].ResetToBeginning();
				tween[j].enabled = true;
			}
		}
	}

    public void PlayForward()
    {
		if( objTweenForReset == null )
			return;

        for (int i = 0; i < objTweenForReset.Length; ++i)
        {
			if( objTweenForReset[i] == null )
				continue;

			UITweener[] tween = objTweenForReset[i].GetComponents<UITweener>();
			if( tween == null )
				continue;

			for( int j = 0; j < tween.Length; ++j)
            {
				if( tween[j] == null )
					continue;

				tween[j].ResetToBeginning();
                tween[j].PlayForward();
            }
        }
    }

    public void StopAndInit()
    {
		if( objTweenForReset == null )
			return;

        for (int i = 0; i < objTweenForReset.Length; ++i)
        {
			if( objTweenForReset[i] == null )
				continue;

			UITweener[] tween = objTweenForReset[i].GetComponents<UITweener>();
			if( tween == null )
				continue;

			for( int j = 0; j < tween.Length; ++j)
            {
				if( tween[j] == null )
					continue;

				tween[j].ResetToBeginning();
                tween[j].enabled = false;
            }
        }
    }
}
