using System.Collections;
using Assets.Script.Visual.UI;
using UnityEngine;

namespace Assets.Script.Potion
{
    public class PotionPickup : MonoBehaviour
    {
        private bool isInRange = false;
        private DamageBooster damageBooster;
        private InventorySystem inventory;
        private PlayerManager playerManager;
        public Sprite potionSprite;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                isInRange = true;
                inventory = other.GetComponent<InventorySystem>();
                damageBooster = other.GetComponentInChildren<DamageBooster>();
                playerManager = other.GetComponent<PlayerManager>();
            }
            else if (other.CompareTag("Enemy"))
            {
                DamageBooster damageBooster = other.GetComponent<DamageBooster>();
                if (damageBooster != null)
                {
                    damageBooster.BoostEnemy(); 
                    Destroy(gameObject);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                isInRange = false;
                inventory = null;
            }
        }

        private void Update()
        {
            // Player presses E to pick up
            if (isInRange && Input.GetKeyDown(KeyCode.E))
            {
                PickupPotion();
            }
        }

        private void Boosting()
        {
            if (damageBooster != null)
            {
                damageBooster.ActivateDamageBoost();

                // Get PlayerManager component
                if (playerManager != null)
                {
                    playerManager.StartCoroutine(FlashWhileBuffed(playerManager));
                }
            }
        }

        private IEnumerator FlashWhileBuffed(PlayerManager playerManager)
        {
            while (damageBooster.isBoostActive) // Since it's public now
            {
                playerManager.StartFlashing(); // Calls the public function
                yield return new WaitForSeconds(0.00005f); // Slight delay to avoid overloading
            }
        }


        private void PickupPotion()
        {
            if (inventory != null && potionSprite != null)
            {
                inventory.AddItem("Potion", potionSprite, Boosting ); 
                Destroy(gameObject);
            }
        }
    }
}
