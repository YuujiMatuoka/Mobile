using UnityEngine;

[CreateAssetMenu(menuName = "Game/EnemySO")]
public class EnemySO : ScriptableObject
{
    public string enemyID;       // "Zombie"
    public GameObject prefab;
    public float maxHealth = 10f;
    public float moveSpeed = 2f;
    public float damage = 1f;
    public bool isBoss = false;
}
