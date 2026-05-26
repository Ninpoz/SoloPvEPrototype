using UnityEngine;

public class EnemyChaser : MonoBehaviour
{
    // The player this enemy should react to.
    // Assign this in the Inspector by dragging the Player object here.
    [SerializeField] private Transform player;

    // If the player is inside this distance, the enemy starts chasing.
    [SerializeField] private float chaseRange = 5f;

    // How fast the enemy moves while chasing.
    [SerializeField] private float moveSpeed = 3f;

    // How quickly the enemy turns to face the player.
    [SerializeField] private float turnSpeed = 10f;

    // If the player is inside this distance, the enemy switches from chasing
    // to attack state.
    [SerializeField] private float attackRange = 1.5f;

    // Remembers whether the enemy was in attack range during the previous frame.
    // We use this to detect the moment the enemy enters attack range.
    private bool wasInAttackRange;

    void Update()
    {
        // Stop early if the enemy has no player assigned.
        if (player == null)
        {
            return;
        }

        // Measure the current distance to the player.
        // Then convert that number into simple gameplay checks.
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool isPlayerInAttackRange = distanceToPlayer <= attackRange;
        bool isPlayerInChaseRange = distanceToPlayer <= chaseRange;

        // Build a flat target position using the player's X and Z,
        // but keep the enemy's current Y height so it stays on the ground plane.
        Vector3 targetPosition = new Vector3(
            player.position.x,
            transform.position.y,
            player.position.z
        );

        // This detects the moment the enemy enters attack range.
        // We only log once when the state changes from "not attacking" to "attacking".
        if (!wasInAttackRange && isPlayerInAttackRange)
        {
            Debug.Log("Enemy is attacking");
        }

        // Attack has priority over chase.
        // In attack range, the enemy should face the player but stop moving.
        if (isPlayerInAttackRange)
        {
            RotateTowards(targetPosition);
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

        // Save the current attack-range result so next frame can detect
        // whether the enemy just entered or left attack range.
        wasInAttackRange = isPlayerInAttackRange;
    }

    private void RotateTowards(Vector3 targetPosition)
    {
        // Find the direction from the enemy to the target.
        Vector3 direction = targetPosition - transform.position;

        // Ignore vertical difference so rotation stays flat on the ground.
        direction.y = 0f;

        // Only rotate if there is a real direction to face.
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Smoothly turn toward the target instead of snapping instantly.
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                turnSpeed * Time.deltaTime
            );
        }
    }
}