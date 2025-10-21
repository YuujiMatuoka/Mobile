using UnityEngine;

public class PooledBullet : MonoBehaviour, IPooledObject
{
    [Header("Configuración Bala")]
    public float lifeTime = 5f;
    public string poolTag = "Bullet";
    public float damage = 1f;
    public string ownerTag = "Player"; // tag del que disparó (para evitar friendly-fire)

    Rigidbody2D rb;
    Vector2 velocity;
    float spawnTime;
    bool moving = false;

    void Awake() { rb = GetComponent<Rigidbody2D>(); }

    public void OnObjectSpawn()
    {
        spawnTime = Time.time;
        moving = false;
        if (rb != null) rb.velocity = Vector2.zero;
        gameObject.SetActive(true);
    }

    public void OnObjectReturn()
    {
        moving = false;
        if (rb != null) rb.velocity = Vector2.zero;
    }

    public void SetMovement(Vector2 vel)
    {
        velocity = vel;
        moving = true;
        if (rb != null) rb.velocity = velocity;
    }

    void Update()
    {
        if (!moving) return;

        if (rb == null)
            transform.Translate(velocity * Time.deltaTime);

        if (Time.time - spawnTime >= lifeTime)
            ReturnToPool();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // Ignorar colisión con quien disparó (si ownerTag está seteado)
        if (!string.IsNullOrEmpty(ownerTag) && col.CompareTag(ownerTag)) return;

        var enemy = col.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        ReturnToPool();
    }

    void ReturnToPool()
    {
        if (ObjectPooler.Instance != null)
            ObjectPooler.Instance.ReturnToPool(poolTag, gameObject);
        else
            gameObject.SetActive(false);
    }
}