using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 5f;
    float current;

    void Awake() { current = maxHealth; }

    public void TakeDamage(float amount)
    {
        current -= amount;
        Debug.Log($"Player HP: {current}/{maxHealth}");
        if (current <= 0f) Die();
    }

    void Die()
    {
        Debug.Log("PLAYER DEAD");
        gameObject.SetActive(false);
    }
}
