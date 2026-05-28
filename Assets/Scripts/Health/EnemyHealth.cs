using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private WorldHealthBar healthBar;

    private int currentHealth;

    public bool IsDead { get; private set; }

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(int damage)
    {
        if (IsDead)
        {
            return;
        }

        currentHealth -= damage;
        UpdateHealthBar();
        Debug.Log($"Enemy took {damage} damage, current health: {currentHealth}");
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            IsDead = true;
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth, maxHealth);
        }
    }
}