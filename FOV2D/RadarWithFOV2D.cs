using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarWithFOV2D : MonoBehaviour
{
    private FOV2D fov2d;

    public bool isNearEnemy = true;

    [HideInInspector]
    public GameObject objTarget;

    public void Awake()
    {
        fov2d = GetComponent<FOV2D>();
    }

    public void FixedUpdate()
    {
        if(fov2d.isTargetInside)
        {
            if(isNearEnemy)
            {
                objTarget = GetNearestTarget();
            }
            else
            {
                objTarget = GetFurthermostTarget();
            }
        }
        else
        {
            objTarget = null;
        }
    }

    public GameObject GetNearestTarget()
    {
        if (fov2d.visibleTargets[0] == null)
            return null;

        float dist = Vector2.Distance(transform.position, fov2d.visibleTargets[0].position);
        int cnt = 0;
        for(int i = 0; i < fov2d.visibleTargets.Count; ++i)
        {
            if (fov2d.visibleTargets[i] == null)
                continue;

            if (Vector2.Distance(transform.position, fov2d.visibleTargets[i].position) < dist)
            {
                dist = Vector2.Distance(transform.position, fov2d.visibleTargets[i].position);
                cnt = i;
            }
        }

        if (fov2d.visibleTargets.Count == 0)
            return null;

        return fov2d.visibleTargets[cnt].gameObject;
    }

    public GameObject GetFurthermostTarget()
    {
        if (fov2d.visibleTargets[0] == null)
            return null;

        float dist = Vector2.Distance(transform.position, fov2d.visibleTargets[0].position);
        int cnt = 0;
        for (int i = 0; i < fov2d.visibleTargets.Count; ++i)
        {
            if (fov2d.visibleTargets[i] == null)
                continue;

            if (Vector2.Distance(transform.position, fov2d.visibleTargets[i].position) > dist)
            {
                dist = Vector2.Distance(transform.position, fov2d.visibleTargets[i].position);
                cnt = i;
            }
        }

        if (fov2d.visibleTargets.Count == 0)
            return null;

        return fov2d.visibleTargets[cnt].gameObject;
    }
}
