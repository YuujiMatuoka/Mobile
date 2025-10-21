using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Enemy : MonoBehaviour
{
    public EnemySO config;
    public float currentHealth;
    public string poolTag = "Enemy_Small";

    void OnEnable()
    {
        if (config != null && currentHealth == 0f) currentHealth = config.maxHealth;
        EnemyManager.Instance?.Register(this);
    }

    void OnDisable()
    {
        EnemyManager.Instance?.Unregister(this);
    }

    public void Initialize(EnemySO so, string poolTag = null)
    {
        config = so;
        this.poolTag = poolTag ?? this.poolTag;
        currentHealth = so.maxHealth;
        EnemyManager.Instance?.Register(this);
    }

    public void TakeDamage(float dmg)
    {
        currentHealth -= dmg;
        if (currentHealth <= 0f) Die();
    }

    void Die()
    {
        EnemyManager.Instance?.Unregister(this);
        if (!string.IsNullOrEmpty(poolTag) && ObjectPooler.Instance != null)
            ObjectPooler.Instance.ReturnToPool(poolTag, gameObject);
        else
            gameObject.SetActive(false);
    }
}