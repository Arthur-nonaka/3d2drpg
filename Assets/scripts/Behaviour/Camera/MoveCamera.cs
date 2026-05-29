using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] private GameObject playerGameObject;
    [SerializeField] private float rotationSpeed = 90f;
    private PlayerInput input;

    void Awake()
    {
        input = playerGameObject.GetComponent<PlayerInput>();
    }

    void LateUpdate()
    {
        float rotationThisFrame = 0f;

        if (input.cameraDirection == 2)
        {
            rotationThisFrame = -rotationSpeed * Time.deltaTime;
        }
        else if (input.cameraDirection == 1)
        {
            rotationThisFrame = rotationSpeed * Time.deltaTime;
        }

        if (rotationThisFrame != 0f)
        {
            transform.Rotate(0, rotationThisFrame, 0, Space.World);
        }
    }
}