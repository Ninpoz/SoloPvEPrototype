using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 10;

    private int currentHealth;
    private bool isDead;
    public bool IsDead => isDead;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
    }

    // This method is called by other scripts to apply damage to the Enemy.
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"Enemy took {damage} damage, current health: {currentHealth}");
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // This method handles the enemy's death.
    private void Die()
    {
        isDead = true;
        Debug.Log("Enemy has died!");
        gameObject.SetActive(false);

    }
}
