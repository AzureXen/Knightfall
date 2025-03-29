using System;
using Assets.Script.Enemy.Slime;
using UnityEngine;

namespace Assets.Script.Object.Apple
{
    public class Healing : MonoBehaviour
    {
        public int healAmount = 20; // Heal for player
        public float healthMultiplier = 2f; // Multiplier for enemy health
        public float sizeMultiplier = 1.5f; // How much the enemy scales up when buffed

        private Health playerHealth;
        private Health enemyHealth;
        private SlimeManager slimeManager;

        void Start()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");

            if (player != null)
            {
                playerHealth = player.GetComponent<Health>();
            }

            if (enemy != null)
            {
                enemyHealth = enemy.GetComponent<Health>();
                slimeManager = enemy.GetComponent<SlimeManager>(); 
            }
        }

        public void Heal()
        {
            if (playerHealth != null)
            {
                playerHealth.health = Mathf.Min(playerHealth.health + healAmount, playerHealth.maxHealth);
            }
        }

        public void BuffEnemy()
        {
            if (enemyHealth != null)
            {
                // Increase max health
                enemyHealth.maxHealth = Mathf.RoundToInt(enemyHealth.maxHealth * healthMultiplier);

                // Heal the enemy by half of the new max health
                enemyHealth.health = Mathf.Min(enemyHealth.maxHealth / 2 + enemyHealth.health, enemyHealth.maxHealth);

                // Increase enemy size
                slimeManager.SizeInflate(sizeMultiplier);
            }
        }
    }
}
