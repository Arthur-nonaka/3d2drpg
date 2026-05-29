using UnityEngine;

public static class CameraUtils
{
    public static Vector3 updateDirections(Vector3 inputDirection, Camera camera)
    {
        Vector3 cameraForward = camera.transform.forward;
        Vector3 cameraRight = camera.transform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDirection = cameraForward * inputDirection.z + cameraRight * inputDirection.x;

        return moveDirection;
    }
}
