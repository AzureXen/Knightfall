using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerScript : MonoBehaviour
{
    private List<Transform> spawnPoints = new List<Transform>();
    [SerializeField] private GameObject enemyPrefab;

    void Start()
    {
        InitializeSpawnPoints();
        SpawnEnemyWave(6);
    }

    public void SpawnEnemyWave(int enemiesToSpawn)
    {
        if (spawnPoints.Count == 0)
        {
            Debug.LogError("No spawn points available!");
            return;
        }

        if (enemiesToSpawn > spawnPoints.Count)
        {
            Debug.LogWarning($"Requested {enemiesToSpawn} enemies, but only have {spawnPoints.Count} points. Capping spawn count to max available points.");
            enemiesToSpawn = spawnPoints.Count;
        }

        List<Transform> availablePoints = new List<Transform>(spawnPoints);

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            int randomIndex = Random.Range(0, availablePoints.Count);
            Transform chosenPoint = availablePoints[randomIndex];

            Instantiate(enemyPrefab, chosenPoint.position, Quaternion.identity);

            availablePoints.RemoveAt(randomIndex);
        }
    }

    private void InitializeSpawnPoints()
    {
        GameObject parentObject = GameObject.FindWithTag("EnemySpawnPoints");

        if (parentObject != null)
        {
            spawnPoints.Clear();

            foreach (Transform child in parentObject.transform)
            {
                spawnPoints.Add(child);
            }

            Debug.Log($"Successfully fetched {spawnPoints.Count} spawn points from tagged parent!");
        }
        else
        {
            Debug.LogError("EnemySpawner: Could not find any GameObject with the tag 'EnemySpawnPoints'!");
        }
    }

}
