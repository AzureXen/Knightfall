using System.Collections;
using UnityEngine;

namespace Assets.Script.Potion
{
    public class PotionPickup : MonoBehaviour
    {
        private bool isInRange = false;
        private DamageBooster damageBooster;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") || other.CompareTag("Enemy"))
            {
                isInRange = true;
                damageBooster = other.GetComponentInChildren<DamageBooster>();

                if (other.CompareTag("Enemy"))
                {
                    damageBooster = other.GetComponent<DamageBooster>();
                    AutoPickup(); 
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player") || other.CompareTag("Enemy"))
            {
                isInRange = false;
                damageBooster = null;
            }
        }

        private void Update()
        {
            // Player needs to press E to pick up
            if (isInRange && Input.GetKeyDown(KeyCode.E))
            {
                PickupPotion();
            }
        }

        private void AutoPickup()
        {
            if (damageBooster != null)
            {
                damageBooster.BoostEnemy();
            }

            Destroy(gameObject);
        }

        private void PickupPotion()
        {
            if (damageBooster != null)
            {
                damageBooster.ActivateDamageBoost();
            }

            Destroy(gameObject); 
        }
    }
}
