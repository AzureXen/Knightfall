using UnityEngine;
public class GoblinSpawn : BasicEnemySpawnScript
{
    protected override void SpawnEnemy()
    {
        base.SpawnEnemy();
        Debug.Log("Spawning goblin");
    }
}
