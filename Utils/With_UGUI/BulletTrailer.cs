using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTrailer : PSObject
{
    public Vector3 startPos;
    public Vector3 endPos;

    public float trailspeed;

    public TrailRenderer tr;

    private Vector3 dir = Vector3.zero;

    private float originDistance = 5.0f;

    private float second;

    public void Awake()
    {
       
    }

    public void SetTrailer(Vector3 start, Vector3 end, float speed, float sec)
    {
        tr.Clear();
        tr.emitting = true;

        startPos = start;
        endPos = end;
        second = sec;

        trailspeed = speed;
        dir = end - start;
        dir = dir.normalized;

        originDistance = Vector3.Distance(start, end);

        StartCoroutine(TrailRendering());
    }

    public IEnumerator TrailRendering()
    {
        float elaps = 0.0f;

        while (true)
        {
            elaps += Time.deltaTime;
            Vector3 newPos = transform.position;

            newPos += trailspeed * dir * Time.deltaTime;

            float dist = Vector3.Distance(startPos, newPos);

           
            if (elaps >= second)
            {
                tr.emitting = false;
                tr.Clear();
              
                transform.position = endPos;
                PSObjectPoolManager.instance.SaveObject(key, gameObject);
                yield break;
            }

            if (dist >= originDistance)
            {
                tr.emitting = false;
                tr.Clear();
                transform.position = endPos;
                PSObjectPoolManager.instance.SaveObject(key, gameObject);
                yield break;
            }


            transform.position = newPos;

            yield return new WaitForEndOfFrame();
        }
    }
}
