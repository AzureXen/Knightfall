using UnityEngine;
using System.Collections;


public class MonsterSpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public Transform[] spawnPoints;
    public Transform bossSpawnPoint;
    public GameObject bossPrefab;
    public int enemiesPerWave = 10;
    public float timeBetweenWaves = 15f;
    public float spawnInterval = 1f;
    private int waveNumber = 1;
    public int maxWave = 1;

    private void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        while (waveNumber <= maxWave - 1)
        {

            for (int i = 0; i < enemiesPerWave; i++)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(spawnInterval);
            }

            yield return new WaitForSeconds(timeBetweenWaves);

            waveNumber++;

            if (waveNumber == maxWave)
            {
                yield return new WaitForSeconds(5f);
                SpawnBoss();
            }
        }
        
    }

    void SpawnEnemy()
    {
        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }

    void SpawnBoss()
    {
        if (bossPrefab != null)
        {
            Instantiate(bossPrefab, bossSpawnPoint.position, bossSpawnPoint.rotation);
            Debug.Log("Boss has spawned!");
        }
        else
        {
            Debug.LogWarning("Boss prefab is not assigned!");
        }
    }
}
