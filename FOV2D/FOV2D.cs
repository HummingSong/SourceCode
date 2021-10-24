using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOV2D : MonoBehaviour
{
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();

    [HideInInspector]
    public bool isTargetInside = false;

    public Color circleColor = Color.white;

    public void OnEnable()
    {
        StartCoroutine("FindTargetsWithDelay", .2f);
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll((Vector2)transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.up, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics2D.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                }
            }
        }

        if (visibleTargets.Count > 0)
        {
            isTargetInside = true;
            //mainEnemyTr = GetCloseEnemy();
        }
        else
        {
            isTargetInside = false;
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees -= transform.eulerAngles.z;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public void SetRadius(float rad)
    {
        viewRadius = rad;
    }

    //public Transform GetCloseEnemy()
    //{
    //    float dist = Vector2.Distance(transform.position, visibleTargets[0].position);
    //    Transform tr = visibleTargets[0];
    //    if (visibleTargets.Count > 1)
    //    {
    //        for (int i = 1; i < visibleTargets.Count; ++i)
    //        {
    //            float tempdist = Vector2.Distance(transform.position, visibleTargets[i].position);

    //            if (tempdist < dist)
    //            {
    //                dist = tempdist;
    //                tr = visibleTargets[i];
    //            }
    //        }
    //    }

    //    return tr;
    //}
}
