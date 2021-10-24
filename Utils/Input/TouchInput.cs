/// <summary>
/// TouchModule
/// 
/// 작성자 : 배정욱
/// 작성일 : 2014.11.20
/// 
/// 현재 단순 터치 입력의 포지션 값만을 넘기는 스크립트
/// 후처리는 기획에 따라 추가하도록 한다. 14.11.20
/// </summary>


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchInput : MonoBehaviour {

	private Rect 			touchZone;
	
	// 실제 마지막 값을 저장하는 포지션 벡터
	[HideInInspector]
	public Vector2			vPosition;

    [HideInInspector]
    public Vector2          touchPosition;

	// 터치되는 손가락의 인덱스와 각 값들
    private Dictionary<int, int> tableLastFinger = new Dictionary<int, int>();
    private Dictionary<int, Vector2> tableDownPos = new Dictionary<int, Vector2>();
	private int[] 			iLastFingerId;	
	private Vector2[] 		vFingerDownPos; 

	// 각 손가락의 터치 포지션을 저장할 리스트
	private List<Vector2> 	idPosition; 

	// 터치 감도
	public float 			fMouseSpeed = 45.0f;

    // 터치 가속도
    public float            fMouseAddSpeed = 0.0f;

	private Touch 			touch;

	private Vector2			moveDt;


	public AnimationCurve	accelCurve;//터치를 빠르게 움직일때의 가속 그래프(0에서 maxAccelAbsoluteValue의 값을 거리 / deltaTime 으로 나누어 값을 얻는다)
	public float			maxAccelAbsoluteValue = 20.0f;//터치를 빠르게 움직일때 움직인 거리(도트)의 최대치

    [HideInInspector]
    public bool bTouchCurrent = false;

	void Awake () 
	{
		touchZone 		= new Rect(0.0f, 0.0f, Screen.width * 1f, Screen.height * 1f);

		idPosition 		= new List<Vector2> ();
		vFingerDownPos 	= new Vector2[5];
		iLastFingerId	= new int[5];
		
		for ( int i = 0 ; i < vFingerDownPos.Length; ++i ) 
		{
			vFingerDownPos[i] = Vector2.zero;	
			iLastFingerId[i] = -1;
		}
	}

	void Update ()
	{	
		int count = Input.touchCount;
        //bTouchCurrent = false;

		if ( count > 0 )
		{
			idPosition.Clear();
            

			for ( int i = 0 ; i < count ; ++i )
			{
				touch = Input.GetTouch(i);

                //if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                //    continue;
                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    tableLastFinger.Remove(touch.fingerId);
                    tableDownPos.Remove(touch.fingerId);
                }
				// 터치 입력을 판별한다. UI 입력을 통한 터치는 제외한다.
				// UI입력을 현재는 코드로 구현. 크리티컬한 위험요소가 없는 한 진행한다.
				// 크리티컬한 위험요소가 있을 시 터치 영역제한으로 바꾸도록 한다.
				if (touchZone.Contains (touch.position) && !GameUI.instance.CheckID(touch.fingerId))
			    {
                    //// 새로운 터치인지 검사 
                    //if (iLastFingerId[i] == -1 || iLastFingerId[i] != touch.fingerId)
                    //{
                    //    iLastFingerId[i] = touch.fingerId;
                    //    vFingerDownPos[i] = touch.position;
                    //}
                    touchPosition = touch.position;
                    // 새로운 터치인지 검사 
                    if (!tableLastFinger.ContainsKey(touch.fingerId) || tableLastFinger[touch.fingerId] != touch.fingerId)
                    {
                        tableLastFinger.Add(touch.fingerId, touch.fingerId);

                        if (tableDownPos.ContainsKey(touch.fingerId))
                            tableDownPos[touch.fingerId] = touch.position;
                        else
                            tableDownPos.Add(touch.fingerId, touch.position);
                    }

					// 기존 터치상태
                    //if (iLastFingerId[i] == touch.fingerId)
                    //{
                    //    Vector2 temp = Vector2.zero;
                    //    if (touch.position.x - vFingerDownPos[i].x == 0)
                    //        temp.x = 0;
                    //    else
                    //        temp.x = (touch.position.x - vFingerDownPos[i].x) / (touchZone.width / 2) * fMouseSpeed;

                    //    if (touch.position.y - vFingerDownPos[i].y == 0)
                    //        temp.y = 0;
                    //    else
                    //        temp.y = (touch.position.y - vFingerDownPos[i].y) / (touchZone.height / 2) * fMouseSpeed;

                    //    idPosition.Add(temp);

                    //    vFingerDownPos[i] = touch.position;

                    //    if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    //        iLastFingerId[i] = -1;

                    //    Debug.Log(temp + " : iLastFingerId[" + i + "] :" + iLastFingerId[i] + " : " + touch.fingerId);
                    //}

                    // 터치 아이디 순서가 보장되지 않으니 터치 아이디 순서 자체를 키값으로 지정하게 변경함.
                    if (tableLastFinger.ContainsKey(touch.fingerId))
                    {
                        Vector2 temp = Vector2.zero;

						float accel = 0.0f;


                        if (touch.position.x - tableDownPos[touch.fingerId].x == 0)
                            temp.x = 0;
                        else
						{
							float dx = (touch.position.x - tableDownPos[touch.fingerId].x) / (touchZone.width / 2);

							if(idPosition.Count == 0)
							{
								moveDt.x = dx / Time.deltaTime;
								accel = GetAccelValue(moveDt.x);

								//Debug.Log(accel);
							}
							else
								accel = 1.0f;

							temp.x = dx * fMouseSpeed * accel;
						}

                        if (touch.position.y - tableDownPos[touch.fingerId].y == 0)
                            temp.y = 0;
                        else
						{
							float dy = (touch.position.y - tableDownPos[touch.fingerId].y) / (touchZone.height / 2);

							/*
							if(idPosition.Count == 0)
							{
								moveDt.y = dy / Time.deltaTime;
								accel = GetAccelValue(moveDt.y);
							}
							else*/
								accel = 1.0f;

							temp.y = dy * fMouseSpeed * accel;
						}

                        idPosition.Add(temp);

                        tableDownPos[touch.fingerId] = touch.position;

                        if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                        {
                            tableLastFinger.Remove(touch.fingerId);
                            tableDownPos.Remove(touch.fingerId);
                        }


                    }
				}
                else
                {

                }
			}
            
            vPosition = Vector2.zero;

			if ( idPosition.Count > 0 ) {
				// 중복 터치를 무시하자. 테스트 필요함.
				vPosition.x = idPosition[0].x;
				vPosition.y = idPosition[0].y;

                if(!bTouchCurrent)
                {
                    bTouchCurrent = true;
                }
			}
            else
            {
                bTouchCurrent = false;
            }

		}
		else
		{
			ResetTouch ();
		}

	}

	float GetAccelValue(float dValue)
	{
		float index = Mathf.Abs (dValue);

		index = Mathf.Clamp (index, 0.0f, maxAccelAbsoluteValue);

		index = index / maxAccelAbsoluteValue;

		return accelCurve.Evaluate (index);
	}

	void ResetTouch () 
	{
		vPosition = Vector3.zero;
		idPosition.Clear ();
        bTouchCurrent = false;

		for ( int i = 0 ; i < vFingerDownPos.Length; ++i ) 
		{
			vFingerDownPos[i] = Vector2.zero;	
			iLastFingerId[i] = -1;
		}
	}

    /*void OnGUI()
    {
        maxAccelAbsoluteValue = GUI.HorizontalSlider(new Rect(5, 125, 200, 30), maxAccelAbsoluteValue, 0.0F, 20.0F);
        GUI.Label(new Rect(220, 125, 100, 30), maxAccelAbsoluteValue.ToString());
    }*/
}
