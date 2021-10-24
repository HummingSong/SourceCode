using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIPressManual : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool press = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        press = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        press = false;
    }
}