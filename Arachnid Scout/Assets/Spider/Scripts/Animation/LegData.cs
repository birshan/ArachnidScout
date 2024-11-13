using UnityEngine;

public class LegData
{
    public Transform IKTarget;
    public Transform RaycastOrigin;
    public bool IsLegMoving;

    public LegData(Transform IKTargetValue, Transform RayCastOriginValue, bool flag)
    {
        IKTarget = IKTargetValue;
        RaycastOrigin = RayCastOriginValue;
        IsLegMoving = flag;

    }
}