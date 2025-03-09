using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Script.Door
{
    using UnityEngine;

    public class Key : MonoBehaviour
    {
        public GameObject door;  // Reference to the door
        private bool isPlayerNearby = false; // Track if player is near the key

        private void Update()
        {
            // Check if the player is nearby and presses "E"
            if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Key picked up!");
                door.GetComponent<Door>().OpenDoor(); // Open the door
                Destroy(gameObject); // Remove the key
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerNearby = true;
                Debug.Log("Press 'E' to pick up the key.");
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerNearby = false;
            }
        }
    }

}
