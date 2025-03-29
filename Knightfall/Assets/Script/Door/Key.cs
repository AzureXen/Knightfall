using System;
using UnityEngine;
using Assets.Script.Visual.UI; // Assuming InventorySystem is here

namespace Assets.Script.Door
{
    public class Key : MonoBehaviour
    {
        public GameObject door;  // Reference to the door
        private bool isPlayerNearby = false; // Track if player is near the key
        private InventorySystem inventory;  // Player's inventory
        public Sprite keySprite; // Key's icon for inventory

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerNearby = true;
                inventory = other.GetComponent<InventorySystem>(); // Get the player's inventory
                Debug.Log("Press 'E' to pick up the key.");
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerNearby = false;
                inventory = null;
            }
        }

        private void Update()
        {
            // Player picks up the key
            if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
            {
                PickupKey();
            }
        }

        private void OpenDoor()
        {
            if (door != null)
            {
                door.GetComponent<Door>().OpenDoor();
                Debug.Log("Door opened using key!");
            }
        }

        private void PickupKey()
        {
            if (inventory != null && keySprite != null)
            {
                Debug.Log("Key picked up!");

                // Add key to inventory with a use function to open the door
                inventory.AddItem("Key", keySprite, OpenDoor);
                Destroy(gameObject); // Remove the key from the scene
            }
        }
    }
}
