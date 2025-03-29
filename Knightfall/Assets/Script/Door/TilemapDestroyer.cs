using System;
using Assets.Script.Enemy.Slime;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets
{
    public class TilemapDestroyer : MonoBehaviour
    {
        public Tilemap tilemap;
        public GameObject[] lootPrefabs;  // Regular loot items
        public GameObject keyPrefab;      // The key object
        public GameObject slimePrefab;    // The Slime enemy to spawn

        public Vector3Int keyTilePosition;         // Tile where the key is located
        public Vector3Int indestructibleTilePosition; // Tile where the slime spawns

        private bool keyDestroyed = false;  // Track if the key tile has been destroyed

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("PlayerSword"))
            {
                Bounds swordBounds = collision.bounds;
                DestroyTilesInBounds(swordBounds);
            }
        }

        private void DestroyTilesInBounds(Bounds bounds)
        {
            Vector3Int minTile = tilemap.WorldToCell(bounds.min);
            Vector3Int maxTile = tilemap.WorldToCell(bounds.max);

            for (int x = minTile.x; x <= maxTile.x; x++)
            {
                for (int y = minTile.y; y <= maxTile.y; y++)
                {
                    Vector3Int tilePosition = new Vector3Int(x, y, 0);

                    if (tilemap.HasTile(tilePosition))
                    {
                        if (tilePosition == keyTilePosition)
                        {
                            DropKey(tilePosition);
                            keyDestroyed = true; // Mark key as destroyed
                            Debug.Log("Key tile destroyed at " + tilePosition);

                            // Spawn slime immediately after key is dropped
                            SpawnSlime(indestructibleTilePosition);
                        }
                        else
                        {
                            Debug.Log("Box destroyed at " + tilePosition);
                            tilemap.SetTile(tilePosition, null);
                            DropRandomLoot(tilePosition);
                        }
                    }
                }
            }
        }

        private void DropRandomLoot(Vector3Int tilePosition)
        {
            if (lootPrefabs.Length > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, lootPrefabs.Length);
                GameObject lootToSpawn = lootPrefabs[randomIndex];

                if (lootToSpawn != null)
                {
                    Vector3 spawnPosition = tilemap.GetCellCenterWorld(tilePosition);
                    Instantiate(lootToSpawn, spawnPosition, Quaternion.identity);
                }
            }
        }

        private void DropKey(Vector3Int tilePosition)
        {
            if (keyPrefab != null)
            {
                Vector3 spawnPosition = tilemap.GetCellCenterWorld(tilePosition);
                Instantiate(keyPrefab, spawnPosition, Quaternion.identity);
                tilemap.SetTile(tilePosition, null); // Remove key tile
                Debug.Log("Key dropped at " + tilePosition);
            }
        }

        private void SpawnSlime(Vector3Int tilePosition)
        {
            if (slimePrefab != null)
            {
                GameObject spawnedSlime = Instantiate(slimePrefab, tilePosition, Quaternion.identity);
                tilemap.SetTile(tilePosition, null);
                SlimeMovement slimeMovement = spawnedSlime.GetComponent<SlimeMovement>();
                if (slimeMovement != null)
                {
                    slimeMovement.isAbnormal = true;
                }

                Debug.Log("Abnormal Slime spawned and moving toward the key.");
            }
        }
    }
}
