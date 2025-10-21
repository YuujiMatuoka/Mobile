using UnityEngine;
using System.Collections;

public class LaserSpawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    public string laserPoolTag = "Laser";
    public float spawnInterval = 2.5f;
    public float spawnIntervalRandomOffset = 0.6f;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            float wait = spawnInterval + Random.Range(-spawnIntervalRandomOffset, spawnIntervalRandomOffset);
            yield return new WaitForSeconds(Mathf.Max(0.1f, wait));

            if (spawnPoints == null || spawnPoints.Length == 0) continue;
            var sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
            ObjectPooler.Instance?.SpawnFromPool(laserPoolTag, sp.position, sp.rotation);
        }
    }
}
