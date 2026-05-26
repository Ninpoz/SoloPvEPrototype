using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Transform enemy;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float turnSpeed = 10f;

    private EnemyHealth enemyHealth;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (enemy != null)
        {
            enemyHealth = enemy.GetComponent<EnemyHealth>();
        }
    }

    // Update is called once per frame
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

        float distanceToEnemy = Vector3.Distance(transform.position, enemy.position);

        if (distanceToEnemy > attackRange)
        {
            Debug.Log("Enemy is out of range");
            return;
        }

        Vector3 targetPosition = GetTargetPosition();
        RotateTowards(targetPosition);
        enemyHealth.TakeDamage(damage);
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
}
