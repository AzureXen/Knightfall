using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets
{
    public class TilemapDestroyer : MonoBehaviour
    {
        public Tilemap tilemap;
        public GameObject[] lootPrefabs; // Array of possible loot (items or monsters)

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Check if hit by player's sword
            if (collision.CompareTag("PlayerSword"))
            {
                // Get bounds of the sword collider
                Bounds swordBounds = collision.bounds;
                DestroyTilesInBounds(swordBounds);
            }
        }

        private void DestroyTilesInBounds(Bounds bounds)
        {
            // Convert bounds to tilemap grid positions
            Vector3Int minTile = tilemap.WorldToCell(bounds.min);
            Vector3Int maxTile = tilemap.WorldToCell(bounds.max);

            for (int x = minTile.x; x <= maxTile.x; x++)
            {
                for (int y = minTile.y; y <= maxTile.y; y++)
                {
                    Vector3Int tilePosition = new Vector3Int(x, y, 0);

                    if (tilemap.HasTile(tilePosition))
                    {
                        Debug.Log("Box destroyed at " + tilePosition);
                        tilemap.SetTile(tilePosition, null);
                        DropRandomLoot(tilePosition);
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
    }
}
