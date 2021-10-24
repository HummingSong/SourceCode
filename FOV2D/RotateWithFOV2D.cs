using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 레이더가 필수
public class RotateWithFOV2D : MonoBehaviour
{
    public GameObject objForwardBase;

    public GameObject objRotateTarget = null;

    private RadarWithFOV2D fovRadar;

    public float angleSpeed = 90.0f;
    public float targetAngle = 5.0f;

    public bool lookTarget = false;

    public void Awake()
    {
        fovRadar = GetComponent<RadarWithFOV2D>();

        lookTarget = false;
    }

    public void FixedUpdate()
    {
        if(objRotateTarget == null)
        {
            lookTarget = false;
            return;
        }

        if(fovRadar.objTarget)
        {
            RotateToTarget();
        }
        else
        {
            RotateToOrigin();
        }
    }

    public void RotateToOrigin()
    {
        Quaternion originQt = Quaternion.FromToRotation(Vector3.up, objForwardBase.transform.up);
        originQt.x = 0;
        originQt.y = 0;
        objRotateTarget.transform.rotation = Quaternion.RotateTowards(objRotateTarget.transform.rotation, originQt, angleSpeed * Time.deltaTime);

        lookTarget = false;
    }

    public void RotateToTarget()
    {
        Vector3 dir = fovRadar.objTarget.transform.position - transform.position;
        dir.z = 0;

        Quaternion newQt = Quaternion.FromToRotation(Vector3.up, dir.normalized);
        newQt.x = 0;
        newQt.y = 0;
        objRotateTarget.transform.rotation = Quaternion.RotateTowards(objRotateTarget.transform.rotation, newQt, angleSpeed * Time.deltaTime);

        if(Quaternion.Angle(newQt, objRotateTarget.transform.rotation) <= targetAngle)
        {
            lookTarget = true;
        }
        else
        {
            lookTarget = false;
        }
    }

    public GameObject GetTarget()
    {
        return fovRadar.objTarget;
    }

    public void SetNearEnemy(bool set)
    {
        fovRadar.isNearEnemy = set;
    }
}
