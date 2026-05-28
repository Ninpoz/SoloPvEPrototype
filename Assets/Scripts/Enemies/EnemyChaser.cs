using UnityEngine;

public class EnemyChaser : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Movement")]
    [SerializeField] private float chaseRange = 6f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float turnSpeed = 10f;
    [SerializeField] private float maxChaseDistance = 10f;
    [SerializeField] private float attackRange = 0.4f;
    [SerializeField] private float stoppingDistance = 0.15f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Attack")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackCooldown = 1.5f;

    private Vector3 homePosition;
    private Vector3 verticalVelocity;

    private PlayerHealth playerHealth;
    private EnemyHealth enemyHealth;

    private CharacterController enemyController;
    private CharacterController playerController;
    private Animator animator;

    private float lastAttackTime;
    private bool deathAnimationPlayed;

    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int DieHash = Animator.StringToHash("Die");

    void Start()
    {
        homePosition = transform.position;

        enemyHealth = GetComponent<EnemyHealth>();
        enemyController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
            playerController = player.GetComponent<CharacterController>();
        }

        SetMoving(false);
    }

    void Update()
    {
        if (enemyHealth != null && enemyHealth.IsDead)
        {
            HandleDeadState();
            ApplyGravity();
            return;
        }

        if (player == null || playerHealth == null || playerHealth.IsDead)
        {
            SetMoving(false);
            ApplyGravity();
            return;
        }

        float distanceFromHomeToEnemy = GetFlatDistance(homePosition, transform.position);
        bool enemyTooFarFromHome = distanceFromHomeToEnemy > maxChaseDistance;

        float distanceToPlayer = GetDistanceToPlayer();

        bool playerInAttackRange = distanceToPlayer <= attackRange;
        bool playerInChaseRange = distanceToPlayer <= chaseRange;

        Vector3 targetPosition = GetTargetPosition();

        if (enemyTooFarFromHome)
        {
            HandleReturnHomeState();
        }
        else if (playerInAttackRange)
        {
            HandleAttackState(targetPosition);
        }
        else if (playerInChaseRange)
        {
            HandleChaseState(targetPosition);
        }
        else
        {
            SetMoving(false);
        }

        ApplyGravity();
    }

    private float GetFlatDistance(Vector3 firstPosition, Vector3 secondPosition)
    {
        firstPosition.y = 0f;
        secondPosition.y = 0f;

        return Vector3.Distance(firstPosition, secondPosition);
    }

    private float GetDistanceToPlayer()
    {
        if (enemyController != null && playerController != null)
        {
            Vector3 enemyClosestPoint = enemyController.ClosestPoint(player.position);
            Vector3 playerClosestPoint = playerController.ClosestPoint(transform.position);

            enemyClosestPoint.y = 0f;
            playerClosestPoint.y = 0f;

            return Vector3.Distance(enemyClosestPoint, playerClosestPoint);
        }

        return GetFlatDistance(transform.position, player.position);
    }

    private Vector3 GetTargetPosition()
    {
        return new Vector3(
            player.position.x,
            transform.position.y,
            player.position.z
        );
    }

    private void HandleAttackState(Vector3 targetPosition)
    {
        SetMoving(false);
        RotateTowards(targetPosition);

        if (Time.time - lastAttackTime < attackCooldown)
        {
            return;
        }

        animator?.SetTrigger(AttackHash);

        playerHealth.TakeDamage(damage);
        lastAttackTime = Time.time;
    }

    private void HandleChaseState(Vector3 targetPosition)
    {
        RotateTowards(targetPosition);

        bool moved = MoveTowards(targetPosition);
        SetMoving(moved);
    }

    private void HandleReturnHomeState()
    {
        float distanceToHome = GetFlatDistance(transform.position, homePosition);

        if (distanceToHome <= stoppingDistance)
        {
            SetMoving(false);
            return;
        }

        RotateTowards(homePosition);

        bool moved = MoveTowards(homePosition);
        SetMoving(moved);
    }

    private void HandleDeadState()
    {
        SetMoving(false);

        if (deathAnimationPlayed)
        {
            return;
        }

        animator?.SetTrigger(DieHash);
        deathAnimationPlayed = true;
    }

    private void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.001f)
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

    private bool MoveTowards(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;

        if (direction.magnitude <= stoppingDistance)
        {
            return false;
        }

        Vector3 moveDirection = direction.normalized;

        if (enemyController != null)
        {
            enemyController.Move(moveDirection * moveSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );
        }

        return true;
    }

    private void ApplyGravity()
    {
        if (enemyController == null)
        {
            return;
        }

        if (enemyController.isGrounded && verticalVelocity.y < 0f)
        {
            verticalVelocity.y = -2f;
        }

        verticalVelocity.y += gravity * Time.deltaTime;

        enemyController.Move(verticalVelocity * Time.deltaTime);
    }

    private void SetMoving(bool isMoving)
    {
        animator?.SetBool(IsMovingHash, isMoving);
    }
}