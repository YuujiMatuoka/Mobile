using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/EnemyWave")]
public class EnemyWaveSO : ScriptableObject
{
    public List<WaveEntry> enemies;

    [System.Serializable]
    public class WaveEntry
    {
        public EnemySO enemyType;
        public int count;
    }
}
