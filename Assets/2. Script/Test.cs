using UnityEngine;

public class Test : MonoBehaviour
{
    public string enemyPoolTag = "Enemy_Small";
    public int count = 3;
    public float radius = 3f;
    void Start()
    {
        for (int i = 0; i < count; i++)
        {
            Vector2 pos = (Vector2)transform.position + Random.insideUnitCircle * radius;
            ObjectPooler.Instance.SpawnFromPool(enemyPoolTag, pos, Quaternion.identity);
        }
    }
}