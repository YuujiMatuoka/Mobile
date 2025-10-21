using UnityEngine;
using System.Collections;

public class BulletSpawner : MonoBehaviour
{
    [Header("Configuración de Nodos")]
    public Transform[] nodes; // Asigna los 4 nodos en orden: esquina superior izquierda, superior derecha, inferior derecha, inferior izquierda

    [Header("Configuración de Balas")]
    public string bulletPoolTag = "Bullet";
    public float initialSpawnRate = 5f;
    public float minSpawnRate = 0.5f;
    public float spawnRateDecrease = 0.5f;
    public float bulletSpeed = 5f;
    public float delayBeforeMove = 1f;

    private float currentSpawnRate;
    private float spawnTimer;
    private float lastSpawnRateDecreaseTime;

    void Start()
    {
        currentSpawnRate = initialSpawnRate;
        spawnTimer = 0f;
        lastSpawnRateDecreaseTime = Time.time;
    }

    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= currentSpawnRate)
        {
            SpawnBullet();
            spawnTimer = 0f;
        }

        // Aumentar tasa de aparición cada 5 segundos
        if (Time.time - lastSpawnRateDecreaseTime >= 5f)
        {
            if (currentSpawnRate > minSpawnRate)
            {
                currentSpawnRate = Mathf.Max(minSpawnRate, currentSpawnRate - spawnRateDecrease);
                Debug.Log("Nueva tasa de aparición: " + currentSpawnRate + " segundos");
            }
            lastSpawnRateDecreaseTime = Time.time;
        }
    }

    void SpawnBullet()
    {
        // Elegir un lado aleatorio (0-3)
        int side = Random.Range(0, 4);
        Vector2 spawnPosition = Vector2.zero;
        Vector2 moveDirection = Vector2.zero;
        bool isVertical = false;

        // Calcular posición y dirección según el lado
        switch (side)
        {
            case 0: // Entre nodo 0 y 1 (arriba) - Movimiento vertical hacia abajo
                spawnPosition = Vector2.Lerp(nodes[0].position, nodes[1].position, Random.value);
                moveDirection = Vector2.down;
                isVertical = true;
                break;
            case 1: // Entre nodo 1 y 2 (derecha) - Movimiento horizontal hacia izquierda
                spawnPosition = Vector2.Lerp(nodes[1].position, nodes[2].position, Random.value);
                moveDirection = Vector2.left;
                isVertical = false;
                break;
            case 2: // Entre nodo 2 y 3 (abajo) - Movimiento vertical hacia arriba
                spawnPosition = Vector2.Lerp(nodes[2].position, nodes[3].position, Random.value);
                moveDirection = Vector2.up;
                isVertical = true;
                break;
            case 3: // Entre nodo 3 y 0 (izquierda) - Movimiento horizontal hacia derecha
                spawnPosition = Vector2.Lerp(nodes[3].position, nodes[0].position, Random.value);
                moveDirection = Vector2.right;
                isVertical = false;
                break;
        }

        // Obtener bala del pool
        GameObject bullet = ObjectPooler.Instance.SpawnFromPool(bulletPoolTag, spawnPosition, Quaternion.identity);

        if (bullet == null) return;

        // Rotar la bala si el movimiento es vertical
        if (isVertical)
        {
            bullet.transform.Rotate(0f, 0f, 90f);
        }

        // Configurar la bala
        StartCoroutine(SetupBulletMovement(bullet, moveDirection, isVertical));
    }

    IEnumerator SetupBulletMovement(GameObject bullet, Vector2 direction, bool isVertical)
    {
        // Esperar 1 segundo antes de moverse
        yield return new WaitForSeconds(delayBeforeMove);

        // Obtener el componente de bala
        PooledBullet pooledBullet = bullet.GetComponent<PooledBullet>();
        if (pooledBullet != null)
        {
            pooledBullet.SetMovement(direction * bulletSpeed);
        }
        else
        {
            // Fallback si no tiene el componente
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = direction * bulletSpeed;
            }
        }
    }
}