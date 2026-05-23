using UnityEngine;

public class EnemyChaser : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float chaseRange = 5f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float turnSpeed = 10f;
    [SerializeField] private float attackRange = 1.5f;

  

    void Update()
    {
        // Stop early if the enemy has no player assigned.
        if (player == null)
        {
            return;
        }

        // Measure distance to the player and turn it into simple behavior checks.
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool isPlayerInAttackRange = distanceToPlayer <= attackRange;
        bool isPlayerInChaseRange = distanceToPlayer <= chaseRange;

        Vector3 targetPosition = new Vector3(
              player.position.x,
              transform.position.y,
              player.position.z
          );

        // Attack has priority over chase.
        if (isPlayerInAttackRange)
        {
            RotateTowards(targetPosition);
            Debug.Log("Enemy is attacking");
        }
        // If the player is not close enough to attack but is close enough to chase,
        // rotate toward the player and move closer.
        else if (isPlayerInChaseRange)
        {
          

            RotateTowards(targetPosition);

            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );
        }

        // If the player is outside chase range, the enemy stays idle.
    }

    private void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                turnSpeed * Time.deltaTime
            );
        }
    }
}