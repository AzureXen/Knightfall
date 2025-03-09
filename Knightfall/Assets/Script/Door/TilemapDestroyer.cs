using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets
{
    public class TilemapDestroyer : MonoBehaviour
    {
        public Tilemap tilemap;
        public GameObject[] lootPrefabs; // Array of possible loot (items or monsters)
        private Transform playerTransform;

        private void Start()
        {
            playerTransform = UnityEngine.Object.FindFirstObjectByType<PlayerManager>().transform;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                TryDestroyTile();
            }
        }

        private void TryDestroyTile()
        {
            Vector3Int tilePosition = GetTileUnderPlayer();
            if (tilemap.HasTile(tilePosition))
            {
                Debug.Log("Box destroyed!");
                tilemap.SetTile(tilePosition, null);
                DropRandomLoot(tilePosition);
            }
        }

        private Vector3Int GetTileUnderPlayer()
        {
            return tilemap.WorldToCell(playerTransform.position);
        }

        private void DropRandomLoot(Vector3Int tilePosition)
        {
            if (lootPrefabs.Length > 0)
            {
                // Choose a random loot from the array
                GameObject lootToSpawn = lootPrefabs[UnityEngine.Random.Range(0, lootPrefabs.Length)];

                if (lootToSpawn != null)
                {
                    Vector3 spawnPosition = tilemap.GetCellCenterWorld(tilePosition);
                    Instantiate(lootToSpawn, spawnPosition, Quaternion.identity);
                }
            }
        }
    }
}
