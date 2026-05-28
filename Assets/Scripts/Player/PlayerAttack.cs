using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform enemy;

    [Header("Attack")]
    [SerializeField] private float attackRange = 0.6f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackCooldown = 0.75f;

    private float lastAttackTime;

    private EnemyHealth enemyHealth;
    private PlayerController playerController;
    private Animator animator;

    private CharacterController playerControllerCollider;
    private CharacterController enemyControllerCollider;

    private static readonly int AttackHash = Animator.StringToHash("Attack");

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        animator = GetComponentInChildren<Animator>();
        playerControllerCollider = GetComponent<CharacterController>();

        if (enemy != null)
        {
            enemyHealth = enemy.GetComponent<EnemyHealth>();
            enemyControllerCollider = enemy.GetComponent<CharacterController>();
        }
    }

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            TryAttack();
        }
    }

    private void TryAttack()
    {
        if (enemy == null || enemyHealth == null || enemyHealth.IsDead)
        {
            Debug.Log("Enemy dead or no valid enemy to attack");
            return;
        }

        float distanceToEnemy = GetDistanceToEnemy();

        if (distanceToEnemy > attackRange)
        {
            Debug.Log($"Enemy is out of range. Distance: {distanceToEnemy}");
            return;
        }

        if (Time.time - lastAttackTime < attackCooldown)
        {
            return;
        }

        if (playerController != null)
        {
            playerController.StopMovement();
        }

        Vector3 targetPosition = GetTargetPosition();
        RotateTowards(targetPosition);

        if (animator != null)
        {
            animator.SetTrigger(AttackHash);
        }

        enemyHealth.TakeDamage(damage);
        lastAttackTime = Time.time;
    }

    private float GetDistanceToEnemy()
    {
        if (playerControllerCollider != null && enemyControllerCollider != null)
        {
            Vector3 playerClosestPoint = playerControllerCollider.ClosestPoint(enemy.position);
            Vector3 enemyClosestPoint = enemyControllerCollider.ClosestPoint(transform.position);

            playerClosestPoint.y = 0f;
            enemyClosestPoint.y = 0f;

            return Vector3.Distance(playerClosestPoint, enemyClosestPoint);
        }

        Vector3 playerPosition = transform.position;
        Vector3 enemyPosition = enemy.position;

        playerPosition.y = 0f;
        enemyPosition.y = 0f;

        return Vector3.Distance(playerPosition, enemyPosition);
    }

    private Vector3 GetTargetPosition()
    {
        return new Vector3(
            enemy.position.x,
            transform.position.y,
            enemy.position.z
        );
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
        transform.rotation = targetRotation;
    }
}