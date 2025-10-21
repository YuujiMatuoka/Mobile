using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }
    List<Enemy> enemies = new List<Enemy>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Register(Enemy e)
    {
        if (e == null) return;
        if (!enemies.Contains(e)) enemies.Add(e);
    }

    public void Unregister(Enemy e)
    {
        if (e == null) return;
        enemies.Remove(e);
    }

    public Enemy GetClosestEnemy(Vector2 pos, float range = Mathf.Infinity)
    {
        Enemy best = null;
        float bestDistSq = range * range;
        for (int i = 0; i < enemies.Count; i++)
        {
            var en = enemies[i];
            if (en == null || !en.gameObject.activeInHierarchy) continue;
            float d = (en.transform.position - (Vector3)pos).sqrMagnitude;
            if (d <= bestDistSq)
            {
                bestDistSq = d;
                best = en;
            }
        }
        return best;
    }
}