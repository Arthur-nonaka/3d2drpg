using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public Vector2 direction { get; private set; }
    public int cameraDirection { get; private set; }

    void Update()
    {
        direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        cameraDirection =
            Input.GetKey(KeyCode.J) ? 1
            : Input.GetKey(KeyCode.K) ? 2
            : 0;
    }
}
