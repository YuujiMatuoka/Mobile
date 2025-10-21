using UnityEngine;

public class EnemyBuilder
{
    public GameObject Build(EnemySO enemySO, Vector3 position)
    {
        GameObject enemy = ObjectPooler.Instance.SpawnFromPool(enemySO.enemyID, position, Quaternion.identity);
        var enemyComponent = enemy.GetComponent<Enemy>();
        enemyComponent.Initialize(enemySO);
        return enemy;
    }
}