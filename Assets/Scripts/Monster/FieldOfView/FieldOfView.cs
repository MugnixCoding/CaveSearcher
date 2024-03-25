

using System;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField]
    private float radious;
    [SerializeField]
    [Range(0,360)]
    private float angle;
    [SerializeField]
    private LayerMask targetMask;
    [SerializeField]
    private LayerMask obstructMask;

    private Transform detectTarget;

    public bool IsTargetInRange()
    {
        Collider[] rangeCheck = Physics.OverlapSphere(transform.position, radious, targetMask);
        for (int i = 0;i<rangeCheck.Length;i++)
        {
            Transform target = rangeCheck[i].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructMask))
                {
                    detectTarget = target;
                    return true;
                }
            }
        }
        detectTarget = null;
        return false;
    }
    public float Radious => radious;
    public float Angle => angle;

    public Transform Target => detectTarget;
}
