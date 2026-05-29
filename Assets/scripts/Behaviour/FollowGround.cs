using UnityEngine;

public class FollowGround : MonoBehaviour
{
    [Header("Ground Check")]
    [SerializeField]
    private LayerMask groundLayerMask = 1;

    [SerializeField]
    private float groundCheckDistance = 2f;

    [SerializeField]
    private float groundOffset = 0.1f;

    void Update()
    {
        RaycastHit hit;
        Vector3 rayStart = transform.position + Vector3.up * 0.5f;
        float rayDistance = groundCheckDistance + 0.5f;

        if (Physics.Raycast(rayStart, Vector3.down, out hit, rayDistance, groundLayerMask))
        {
            Vector3 targetPosition = transform.position;
            targetPosition.y = hit.point.y + groundOffset;
            transform.position = targetPosition;
        }
    }
}
