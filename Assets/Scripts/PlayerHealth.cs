using UnityEngine;

public class PlayerHealth : MonoBehaviour
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

    // This method is called by other scripts to apply damage to the player.
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"Player took {damage} damage, current health: {currentHealth}");
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // This method handles the player's death.
    private void Die()
    {
        isDead = true;
        Debug.Log("Player has died!");

    }
}
