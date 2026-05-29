using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    public Transform player;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Vector2 deadzone;
    [SerializeField] private float smoothSpeed;

    private CameraDeadzone cameraDeadzone;

    void Awake()
    {
        cameraDeadzone = new CameraDeadzone(deadzone);
    }

    void LateUpdate()
    {
        if (player == null)
            return;

        var playerForward = player.forward;
        var frontPosition = player.position + (playerForward * offset.z);

        Vector3 targetPosition = frontPosition + offset.y * Vector3.up;

        Vector3 finalPosition = cameraDeadzone.Apply(targetPosition, transform.position, transform);

        transform.position = Vector3.Lerp(transform.position, finalPosition, Time.deltaTime * smoothSpeed);
    }
}
