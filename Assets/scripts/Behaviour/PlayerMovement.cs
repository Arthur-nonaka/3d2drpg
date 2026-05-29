using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerInput input;
    private Animator animator;
    public PlayerStats playerStats;
    private CharacterController controller;

    [Header("Gravity")]
    [SerializeField]
    private float gravity = -9.81f;
    private Vector3 velocity;

    void Awake()
    {
        input = GetComponent<PlayerInput>();
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        Vector3 moveDirection = CameraUtils.updateDirections(
            new Vector3(input.direction.x, 0, input.direction.y),
            Camera.main
        );
        Vector3 movementDelta = moveDirection * playerStats.Velocity;

        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        movementDelta.y = velocity.y;

        controller.Move(movementDelta * Time.deltaTime);

        animator.SetFloat("MoveX", input.direction.x);
        animator.SetFloat("MoveY", input.direction.y);
    }
}
