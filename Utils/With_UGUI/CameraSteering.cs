using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSteering : MonoBehaviour
{
    public GameObject objTarget;

    public float trackingSpeed = 0.125f;
    private float prevTrackingSpeed = 0.0f;

    private bool isReady = false;

    public void Awake()
    {
        Init();
    }

    public void Init()
    {
        prevTrackingSpeed = trackingSpeed;
        isReady = false;
    }

    public void LateUpdate()
    {
        if (!isReady)
            return;

        Vector3 targetPos = objTarget.transform.position;
        targetPos.z = transform.position.z;
        Vector3 newPos = Vector3.Lerp(transform.position, targetPos, trackingSpeed/* * Time.deltaTime*/);

        transform.position = newPos;
    }

    public void SetTrackingTarget(GameObject target)
    {
        objTarget = target;
        Vector3 targetPos = objTarget.transform.position;
        targetPos.z = transform.position.z;
        transform.position = targetPos;
    }

    public void SetReady(bool set)
    {
        isReady = set;
    }

    public void SetTrackingSpeed(float speed)
    {
        prevTrackingSpeed = trackingSpeed;
        trackingSpeed = speed;
    }

    public void UndoSpeed()
    {
        trackingSpeed = prevTrackingSpeed;
    }
}
