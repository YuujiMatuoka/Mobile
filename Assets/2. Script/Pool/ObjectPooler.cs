using UnityEngine;
using System.Collections.Generic;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
        public bool expandable = true;
    }

    public static ObjectPooler Instance;

    [Header("Configuración del Pool")]
    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;
    public Dictionary<string, Pool> poolInfo;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // Descomenta si quieres que persista entre escenas
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializePools();
    }

    private void InitializePools()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        poolInfo = new Dictionary<string, Pool>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
            poolInfo.Add(pool.tag, pool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool con tag {tag} no existe");
            return null;
        }

        // Buscar objeto inactivo en el pool
        GameObject objectToSpawn = null;
        Queue<GameObject> poolQueue = poolDictionary[tag];

        // Verificar si hay objetos disponibles en el pool
        if (poolQueue.Count > 0)
        {
            objectToSpawn = poolQueue.Dequeue();
        }
        else if (poolInfo[tag].expandable)
        {
            // Crear nuevo objeto si el pool es expandible
            objectToSpawn = Instantiate(poolInfo[tag].prefab);
        }
        else
        {
            Debug.LogWarning($"Pool {tag} está vacío y no es expandible");
            return null;
        }

        // Configurar el objeto
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        // Llamar a OnObjectSpawn si existe
        IPooledObject pooledObject = objectToSpawn.GetComponent<IPooledObject>();
        pooledObject?.OnObjectSpawn();

        return objectToSpawn;
    }

    public void ReturnToPool(string tag, GameObject objectToReturn)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool con tag {tag} no existe");
            return;
        }

        objectToReturn.SetActive(false);
        poolDictionary[tag].Enqueue(objectToReturn);
    }

    // Método para expandir un pool específico
    public void ExpandPool(string tag, int additionalSize)
    {
        if (!poolInfo.ContainsKey(tag) || !poolInfo[tag].expandable)
        {
            Debug.LogWarning($"No se puede expandir el pool {tag}");
            return;
        }

        Queue<GameObject> poolQueue = poolDictionary[tag];
        Pool pool = poolInfo[tag];

        for (int i = 0; i < additionalSize; i++)
        {
            GameObject obj = Instantiate(pool.prefab);
            obj.SetActive(false);
            poolQueue.Enqueue(obj);
        }

        pool.size += additionalSize;
        Debug.Log($"Pool {tag} expandido a {pool.size} objetos");
    }
}

// Interface para objetos del pool
public interface IPooledObject
{
    void OnObjectSpawn();
    void OnObjectReturn();
}