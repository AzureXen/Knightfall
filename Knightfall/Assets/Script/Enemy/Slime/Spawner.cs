using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script.Enemy.Slime
{
    public class Spawner : MonoBehaviour
    {
        public GameObject[] enemyPrefabs; // Enemy types to spawn
        public GameObject bossPrefab; // Boss prefab

        public List<Vector3> enemySpawnPositions; // List of exact spawn positions
        public Vector3 bossSpawnPosition; // Boss spawn position

        public float spawnInterval = 1f; // Time between spawns
        public float bossSpawnDelay = 3f; // Time before boss appears after enemies

        private void Start()
        {
            StartCoroutine(SpawnWave());
        }

        IEnumerator SpawnWave()
        {
            if (enemyPrefabs.Length == 0 || enemySpawnPositions.Count == 0)
            {
                Debug.LogWarning("No enemies or spawn positions assigned!");
                yield break;
            }

            // Spawn exactly one enemy at each designated position
            for (int i = 0; i < enemySpawnPositions.Count; i++)
            {
                GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
                Vector3 spawnPosition = enemySpawnPositions[i];

                Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
                yield return new WaitForSeconds(spawnInterval);
            }

            // Wait before spawning the boss
            yield return new WaitForSeconds(bossSpawnDelay);
            SpawnBoss();
        }

        void SpawnBoss()
        {
            if (bossPrefab == null)
            {
                Debug.LogWarning("Boss prefab is not assigned!");
                return;
            }

            Instantiate(bossPrefab, bossSpawnPosition, Quaternion.identity);
            Debug.Log("Boss has spawned at " + bossSpawnPosition);
        }
    }
}
