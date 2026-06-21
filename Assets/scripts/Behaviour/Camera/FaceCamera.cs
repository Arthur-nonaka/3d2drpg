using UnityEngine;

public class Billboard : MonoBehaviour
{
    void LateUpdate()
    {
        var cameraRotation = Camera.main.transform.rotation;

        transform.rotation = new Quaternion()
        {
            x = 0,
            y = cameraRotation.y,
            z = 0,
            w = transform.rotation.w,
        };
    }
}
