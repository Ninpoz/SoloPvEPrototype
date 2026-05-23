using UnityEngine;

public class EnemyChaser : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float chaseRange = 5f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float stopDistance = 1.5f;
    [SerializeField] private float turnSpeed = 10f;

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= chaseRange && distanceToPlayer > stopDistance)
        {
            Vector3 targetPosition = new Vector3(
                player.position.x,
                transform.position.y,
                player.position.z
            );

            Vector3 direction = targetPosition - transform.position;
            direction.y = 0;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    turnSpeed * Time.deltaTime
                );
            }

            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );
        }
    }
}
