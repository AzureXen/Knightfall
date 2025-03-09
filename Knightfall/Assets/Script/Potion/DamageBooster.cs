using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Script.Potion
{
    public class DamageBooster : MonoBehaviour
    {
        public float damageMultiplier = 2f;  // Multiplier for increased damage
        public float effectDuration = 5f;    // Duration of the effect in seconds

        private PlayerSword playerSword;
        private int originalDamage;
        private bool isBoostActive = false;

        void Start()
        {
            // Get Player GameObject and find PlayerSword in its children
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                playerSword = player.GetComponentInChildren<PlayerSword>();

                if (playerSword != null)
                {
                    originalDamage = playerSword.meleeDamage; // Store original damage
                }
                else
                {
                    Debug.LogError("PlayerSword not found in Player's children!");
                }
            }
            else
            {
                Debug.LogError("Player GameObject not found! Make sure it has the 'Player' tag.");
            }
        }

        public void ActivateDamageBoost()
        {
            if (!isBoostActive && playerSword != null)
            {
                StartCoroutine(BoostDamage());
            }
        }

        private IEnumerator BoostDamage()
        {
            isBoostActive = true;
            playerSword.meleeDamage = Mathf.RoundToInt(originalDamage * damageMultiplier);
            Debug.Log("Damage Boosted to: " + playerSword.meleeDamage);

            yield return new WaitForSeconds(effectDuration);

            playerSword.meleeDamage = originalDamage; // Revert to normal damage
            isBoostActive = false;
            Debug.Log("Damage boost expired. Back to: " + playerSword.meleeDamage);
        }
    }
}
