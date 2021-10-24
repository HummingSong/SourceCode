using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public static Joystick instance = null;

    public bool IsJoyStick = false;

    public Vector2 moveDir = Vector2.zero;
    private Vector3 realDir = Vector3.zero;
    public float round = 100.0f;

    private bool IsPressed = false;
    private Vector2 originPos = Vector2.zero;
    private Vector2 useOriginPos = Vector2.zero;
    private Vector2 inputPos = Vector2.zero;
    private RectTransform rt;

    public Camera uiCamera;
    public GameObject uiJoystickOut;
    public GameObject uiJoystickIn;
    public RectTransform outsideRt;

    private bool GamePause = false;

    private Vector2 offset = Vector2.zero;

    private bool isFixedStick = false;
    private bool isReverse = false;
    private int reverseConst = 1;

    public void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        rt = GetComponent<RectTransform>();
        originPos = rt.anchoredPosition;
        useOriginPos = originPos;

        uiJoystickOut.SetActive(false);
        uiJoystickIn.SetActive(false);
    }
    
    public void SetJoystickOption(bool fix, bool reverse)
    {
        isFixedStick = fix;
        isReverse = reverse;

        if (isReverse)
            reverseConst = -1;
        else
            reverseConst = 1;

        useOriginPos.x = originPos.x * reverseConst;

        //if (isFixedStick)
        //{
            rt.anchoredPosition = useOriginPos;
            outsideRt.anchoredPosition = useOriginPos;
        //}

        uiJoystickOut.SetActive(true);
        uiJoystickIn.SetActive(true);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //IsPressed = true;
        //inputPos = eventData.position;
        //offset = eventData.position;
        //inputPos = ConvertScreenToAnchoredPos(parent, inputPos, uiCamera);
        //offset = ConvertScreenToAnchoredPos(parent, offset, uiCamera);
        //Debug.Log(offset);
    }

    public void OnPointerDownManual(Vector2 eventData)
    {
        inputPos = eventData;
        inputPos = ConvertScreenToAnchoredPos(inputPos);

        offset = useOriginPos - inputPos;

        if (!isFixedStick)
        {
            IsPressed = true;
            outsideRt.anchoredPosition = inputPos;
            rt.anchoredPosition = inputPos;
            uiJoystickOut.SetActive(true);
            uiJoystickIn.SetActive(true);
        }
        else
        {
            if (Vector2.Distance(rt.anchoredPosition, inputPos) < 50)
            {
                IsPressed = true;
            }
        }
    }

    public void OnDragManual(Vector2 eventData)
    {
        if (!IsPressed)
            return;

        inputPos = eventData;
        inputPos = ConvertScreenToAnchoredPos(inputPos);

        if (!isFixedStick)
        {
            rt.anchoredPosition = inputPos;

            if (Vector2.Distance(rt.anchoredPosition, outsideRt.anchoredPosition) > round)
            {
                Vector2 dir = rt.anchoredPosition - outsideRt.anchoredPosition;
                dir = dir.normalized;

                outsideRt.anchoredPosition = rt.anchoredPosition - dir * round;
            }
        }
        else
        {
            rt.anchoredPosition = inputPos + offset;

            if (Vector2.Distance(inputPos, useOriginPos) > round)
            {
                Vector2 dir = inputPos - useOriginPos;
                dir = dir.normalized;

                rt.anchoredPosition = useOriginPos + dir * round + offset;
            }
        }
    }

    public void OnPointerUpManual(Vector2 eventData)
    {
        moveDir = Vector2.zero;

        //if (!isFixedStick)
        //{
        //    uiJoystickOut.SetActive(false);
        //    uiJoystickIn.SetActive(false);
        //}

        inputPos = Vector2.zero;
        offset = Vector2.zero;
        IsPressed = false;

        outsideRt.anchoredPosition = useOriginPos;
        rt.anchoredPosition = useOriginPos;

        //if (!isFixedStick)
        //{
        //    outsideRt.anchoredPosition = inputPos;
        //    rt.anchoredPosition = inputPos;
        //}
        //else
        //{
        //    rt.anchoredPosition = useOriginPos;
        //}
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //rt.anchoredPosition = originPos;
        //inputPos = Vector2.zero;
        //offset = Vector2.zero;
        //IsPressed = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //inputPos = eventData.position;
        //inputPos = ConvertScreenToAnchoredPos(parent, inputPos, uiCamera);
    }

    public void FixedUpdate()
    {
        if (IsPressed)
        {
            if (Vector2.Distance(rt.anchoredPosition, outsideRt.anchoredPosition) < 0.1f)
            {
                return;
            }

            moveDir = rt.anchoredPosition - outsideRt.anchoredPosition;// inputPos.normalized;
            moveDir = moveDir.normalized;
        }
    }

    public Vector3 GetDir()
    {
        if (IsPressed)
        {
            realDir = moveDir;

            return realDir;
        }
        else
            return Vector3.zero;
    }

    public float GetMoveForce()
    {
        if (IsPressed)
        {
            if (Vector2.Distance(rt.anchoredPosition, outsideRt.anchoredPosition) < 0.1f)
            {
                return 0;
            }

            float force = Vector2.Distance(rt.anchoredPosition, outsideRt.anchoredPosition) / round;
            if (force > 1.0f)
                force = 1.0f;

            //return force;
            return 1.0f;
        }
        else
            return 0.0f;
    }

    public void SetGamePause(bool set)
    {
        GamePause = set;
    }

    public Vector2 ConvertScreenToAnchoredPos(RectTransform parent, Vector3 screen, Camera uiCam)
    {
        Vector2 newInputPos = Vector2.zero;
       
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, screen, uiCam, out newInputPos);

        return newInputPos;
    }

    public Vector2 ConvertScreenToAnchoredPos(Vector3 screen)
    {
        Vector2 newInputPos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), screen, uiCamera, out newInputPos);

        return newInputPos;
    }

    public void SetUIJoyStick(bool value)
    {
        uiJoystickIn.SetActive(value);
        uiJoystickOut.SetActive(value);
    }
}
