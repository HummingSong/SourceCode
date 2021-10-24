using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(FOV2D))]
public class FOV2DEditor : Editor
{
    void OnSceneGUI()
    {
        FOV2D fow = (FOV2D)target;
        Handles.color = fow.circleColor;
        Handles.DrawWireArc(fow.transform.position, Vector3.forward, fow.transform.up, 360, fow.viewRadius);
        Vector3 viewAngleA = fow.DirFromAngle(-fow.viewAngle / 2, false);
        Vector3 viewAngleB = fow.DirFromAngle(fow.viewAngle / 2, false);

        Vector3 temp = viewAngleA;
        viewAngleA.y = temp.z;
        viewAngleA.z = temp.y;
        temp = viewAngleB;
        viewAngleB.y = temp.z;
        viewAngleB.z = temp.y;

        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.viewRadius);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.viewRadius);

        //Handles.DrawLine(fow.transform.position, fow.transform.forward * fow.viewRadius);

        Handles.color = Color.red;
        foreach (Transform visibleTarget in fow.visibleTargets)
        {
            Handles.DrawLine(fow.transform.position, visibleTarget.position);
        }

        //Handles.DrawLine(fow.transform.position, fow.transform.up * 2.0f);
    }

}