using UnityEngine;

public class CameraDeadzone
{
    private Vector2 size;

    public CameraDeadzone(Vector2 deadzoneSize)
    {
        size = deadzoneSize;
    }

    public Vector3 Apply(Vector3 targetPos, Vector3 cameraPos, Transform cameraTransform)
    {
        Vector3 localOffset = cameraTransform.InverseTransformPoint(targetPos) - cameraTransform.InverseTransformPoint(cameraPos);

        if (Mathf.Abs(localOffset.x) > size.x)
            cameraPos += cameraTransform.right * (localOffset.x - Mathf.Sign(localOffset.x) * size.x);

        if (Mathf.Abs(localOffset.z) > size.y)
            cameraPos += cameraTransform.forward * (localOffset.z - Mathf.Sign(localOffset.z) * size.y);

        cameraPos.y = targetPos.y;

        return cameraPos;
    }
}