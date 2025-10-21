using UnityEngine;
using System.Collections;

public class PooledLaser : MonoBehaviour, IPooledObject
{
    [Header("Laser")]
    public float warningDelay = 0.12f; // telegraph antes de activar collider
    public float activeDuration = 1f;
    public string poolTag = "Laser";
    public float damage = 1f;

    Collider2D col;
    SpriteRenderer sr;
    Coroutine lifeRoutine;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        if (col != null) col.enabled = false;
        if (sr != null) sr.enabled = false;
    }

    public void OnObjectSpawn()
    {
        if (sr != null) sr.enabled = true;
        lifeRoutine = StartCoroutine(LifeSequence());
    }

    IEnumerator LifeSequence()
    {
        // aviso corto (spriter visible pero sin collider)
        if (warningDelay > 0f) yield return new WaitForSeconds(warningDelay);

        if (col != null) col.enabled = true;
        yield return new WaitForSeconds(activeDuration);

        ReturnToPool();
    }

    public void OnObjectReturn()
    {
        if (lifeRoutine != null) { StopCoroutine(lifeRoutine); lifeRoutine = null; }
        if (col != null) col.enabled = false;
        if (sr != null) sr.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var ph = other.GetComponent<PlayerHealth>();
            if (ph != null) ph.TakeDamage(damage);
        }
    }

    void ReturnToPool()
    {
        if (ObjectPooler.Instance != null)
            ObjectPooler.Instance.ReturnToPool(poolTag, gameObject);
        else
            gameObject.SetActive(false);
    }
}
