using System.Collections;
using UnityEngine;

namespace Assets.Script.Potion
{
    public class PotionPickup : MonoBehaviour
    {
        private bool isPlayerInRange = false;
        private DamageBooster playerBooster;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerInRange = true;
                playerBooster = other.GetComponent<DamageBooster>();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerInRange = false;
                playerBooster = null;
            }
        }

        private void Update()
        {
            if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
            {
                PickupPotion();
            }
        }

        private void PickupPotion()
        {
            if (playerBooster != null)
            {
                playerBooster.ActivateDamageBoost();
            }

            Destroy(gameObject); // Remove potion after pickup
        }
    }
}
