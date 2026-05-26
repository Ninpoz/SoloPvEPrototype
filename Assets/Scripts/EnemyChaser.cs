using UnityEngine;

public class EnemyChaser : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Movement")]
    [SerializeField] private float chaseRange = 5f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float turnSpeed = 10f;
    [SerializeField] private float maxChaseDistance = 8f;

    [Header("Attack")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackCooldown = 1f;

    private Vector3 homePosition;
    private bool wasInAttackRange;
    private PlayerHealth playerHealth;
    private float lastAttackTime;

    void Start()
    {
        homePosition = transform.position;

        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }
    }

    void Update()
    {
        if (player == null)
        {
            return;
        }

        float distanceFromHomeToPlayer = Vector3.Distance(homePosition, player.position);
        bool isPlayerTooFarFromHome = distanceFromHomeToPlayer > maxChaseDistance;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool isPlayerInAttackRange = distanceToPlayer <= attackRange;
        bool isPlayerInChaseRange = distanceToPlayer <= chaseRange;

        Vector3 targetPosition = GetTargetPosition();

        HandleAttackRangeEnter(isPlayerInAttackRange);

        if ((playerHealth != null && playerHealth.IsDead) || isPlayerTooFarFromHome)
        {
            HandleReturnHomeState();
        }

        else if (isPlayerInAttackRange)
        {
            HandleAttackState(targetPosition);
        }
        else if (isPlayerInChaseRange)
        {
            HandleChaseState(targetPosition);
        }

        wasInAttackRange = isPlayerInAttackRange;
    }

    private Vector3 GetTargetPosition()
    {
        return new Vector3(
            player.position.x,
            transform.position.y,
            player.position.z
        );
    }

    private void HandleAttackRangeEnter(bool isPlayerInAttackRange)
    {
        if (!wasInAttackRange && isPlayerInAttackRange)
        {
            Debug.Log("Enemy is attacking");
        }
    }

    private void HandleAttackState(Vector3 targetPosition)
    {
        RotateTowards(targetPosition);

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            if (playerHealth != null && !playerHealth.IsDead)
            {
                playerHealth.TakeDamage(damage);
                lastAttackTime = Time.time;
            }
        }
    }

    private void HandleChaseState(Vector3 targetPosition)
    {
        RotateTowards(targetPosition);
        MoveTowards(targetPosition);
    }

    private void HandleReturnHomeState()
    {
        RotateTowards(homePosition);
        MoveTowards(homePosition);
    }

    private void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;

        if (direction == Vector3.zero)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            turnSpeed * Time.deltaTime
        );
    }

    private void MoveTowards(Vector3 targetPosition)
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );
    }
}