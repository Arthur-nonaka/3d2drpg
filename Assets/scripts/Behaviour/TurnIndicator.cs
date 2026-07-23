using UnityEngine;

public class TurnIndicator : MonoBehaviour
{
    private Transform target;
    private Vector3 offset = new Vector3(0, 2f, 0);

    [Header("Animation")]
    [SerializeField]
    private float bounceHeight;

    [SerializeField]
    private float bounceSpeed;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        var scale = transform.localScale;
        scale.x = -scale.x;
        transform.localScale = scale;
    }

    public void RemoveTarget()
    {
        target = null;
        transform.position = new Vector3(0, -1000, 0);
    }

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 basePosition = target.position + offset;

            float bounce = Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;
            transform.position = basePosition + Vector3.up * bounce;
        }
    }
}
