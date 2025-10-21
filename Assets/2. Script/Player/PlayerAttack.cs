using UnityEngine;

[RequireComponent(typeof(Transform))]
public class PlayerAttack : MonoBehaviour
{
    [Header("Auto Attack")]
    public string bulletPoolTag = "Bullet";
    public float bulletSpeed = 12f;
    public float attackRate = 0.5f;
    public float range = 10f;
    public float bulletDamage = 1f;
    public Transform fireOrigin; // si null usa transform

    float nextAttackTime = 0f;

    void Start()
    {
        if (fireOrigin == null) fireOrigin = transform;
    }

    void Update()
    {
        if (Time.time < nextAttackTime) return;
        var target = EnemyManager.Instance?.GetClosestEnemy(transform.position, range);
        if (target == null) return;

        ShootAt(target);
        nextAttackTime = Time.time + attackRate;
    }

    void ShootAt(Enemy target)
    {
        Vector2 dir = ((Vector2)target.transform.position - (Vector2)fireOrigin.position).normalized;
        var bullet = ObjectPooler.Instance.SpawnFromPool(bulletPoolTag, fireOrigin.position, Quaternion.identity);
        if (bullet == null) return;

        var pooled = bullet.GetComponent<PooledBullet>();
        if (pooled != null)
        {
            pooled.SetMovement(dir * bulletSpeed);
            pooled.damage = bulletDamage;
            pooled.ownerTag = gameObject.tag; // asigna tag del jugador
        }
        else
        {
            var rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null) rb.velocity = dir * bulletSpeed;
        }
    }
}
