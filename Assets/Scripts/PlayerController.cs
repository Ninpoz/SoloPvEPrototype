using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float turnSpeed = 10f;
    [SerializeField] private float stoppingDistance = 0.1f;
    [SerializeField] private float gravity = -9.81f;

    private Vector3 targetPosition;
    private Vector3 verticalVelocity;

    private CharacterController characterController;
    private Animator animator;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        targetPosition = transform.position;
    }

    void Update()
    {
        HandleMouseInput();
        MoveToTarget();
    }

    private void HandleMouseInput()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                targetPosition = new Vector3(
                    hit.point.x,
                    transform.position.y,
                    hit.point.z
                );
            }
        }
    }

    private void MoveToTarget()
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;

        bool isMoving = direction.magnitude > stoppingDistance;

        if (isMoving)
        {
            Vector3 moveDirection = direction.normalized;

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                turnSpeed * Time.deltaTime
            );

            characterController.Move(
                moveDirection * moveSpeed * Time.deltaTime
            );
        }

        ApplyGravity();

        if (animator != null)
        {
            animator.SetBool("IsRunning", isMoving);
        }
    }

    private void ApplyGravity()
    {
        if (characterController.isGrounded && verticalVelocity.y < 0f)
        {
            verticalVelocity.y = -2f;
        }

        verticalVelocity.y += gravity * Time.deltaTime;

        characterController.Move(verticalVelocity * Time.deltaTime);
    }

    public void StopMovement()
    {
        targetPosition = transform.position;

        if (animator != null)
        {
            animator.SetBool("IsRunning", false);
        }
    }
}