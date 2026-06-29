using Unity.AI.Assistant;
using UnityEngine;

public class EnemySpawnScript : MonoBehaviour
{
    // This is a generic enemy spawn script, to use, you must extend this class and implement your own additional methods if
    // necessary
    // Why this class exist? eh, i just thought different enemies might have different spawn effects or whatever event they
    // have, perhaps even a cutscene after spawn, i dunno

    [SerializeField] protected GameObject enemyPrefab;
    public float spawnDelayTime = 2f;

    protected virtual void Start()
    {
        Debug.Log("invoking method to spawn enemy.");
        Invoke(nameof(SpawnEnemy), spawnDelayTime);
    }
    protected virtual void SpawnEnemy()
    {
        Instantiate(enemyPrefab, transform.position, Quaternion.identity);
    }
}
