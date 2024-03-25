using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        FieldOfView fov = (FieldOfView)target;
        Handles.color = Color.yellow;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward,360, fov.Radious);
        Vector3 leftLine = DirectionFromAngle(fov.transform.eulerAngles.y, -fov.Angle / 2);
        Vector3 rightLine = DirectionFromAngle(fov.transform.eulerAngles.y, fov.Angle / 2);
        Handles.DrawLine(fov.transform.position, fov.transform.position + leftLine * fov.Radious);
        Handles.DrawLine(fov.transform.position, fov.transform.position + rightLine * fov.Radious);


        if (fov.IsTargetInRange())
        {
            Handles.color = Color.green;
            Handles.DrawLine(fov.transform.position, fov.Target.transform.position);
        }
    }
    public Vector3 DirectionFromAngle(float eulerY, float angleDegree)
    {
        float angle = angleDegree + eulerY;
        //angleDegree += eulerY;
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }
}
