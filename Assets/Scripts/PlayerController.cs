using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float turnSpeed = 10f;
    [SerializeField] private float stoppingDistance = 0.05f;

    private Vector3 targetPosition;
    private Animator animator;

    void Start()
    {
        targetPosition = transform.position;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                targetPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            }
        }

        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;

        bool isMoving = direction.magnitude > stoppingDistance;

        if (isMoving)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                turnSpeed * Time.deltaTime
            );

            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );
        }

        if (animator != null)
        {
            animator.SetBool("IsRunning", isMoving);
        }
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