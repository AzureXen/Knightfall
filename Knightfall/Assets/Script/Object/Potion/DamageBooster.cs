using System.Collections;
using Assets.Script.Enemy.Slime;
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

        public GameObject bulletPrefab; 
        private int originalBulletDamage;

        private SlimeManager slimeManager;
        private int enemyOriginDmg;

        void Start()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
            if (enemy != null)
            {
                slimeManager = enemy.GetComponent<SlimeManager>();
                if (slimeManager != null)
                {
                    enemyOriginDmg = slimeManager.touchDamage;
                }
                else
                {
                    Debug.LogError("Slime not found in Player's children!");
                }
            }
            if (player != null)
            {
                playerSword = player.GetComponentInChildren<PlayerSword>();

                if (playerSword != null)
                {
                    originalDamage = playerSword.meleeDamage;
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

            // Store bullet original damage (only if prefab is assigned)
            if (bulletPrefab != null)
            {
                PlayerBullet bulletComponent = bulletPrefab.GetComponent<PlayerBullet>();
                if (bulletComponent != null)
                {
                    originalBulletDamage = bulletComponent.damage;
                }
                else
                {
                    Debug.LogError("PlayerBullet script not found on bullet prefab!");
                }
            }
            else
            {
            }
        }

        public void ActivateDamageBoost()
        {
            if (!isBoostActive && playerSword != null)
            {
                StartCoroutine(BoostDamage());
            }
        }

        public void BoostEnemy()
        {
            StartCoroutine(BoosterDanger());
        }

        private IEnumerator BoosterDanger()
        {
            isBoostActive = true;
            slimeManager.touchDamage = Mathf.RoundToInt(enemyOriginDmg * damageMultiplier);
            slimeManager.SetSlimeColor(Color.red);
            yield return new WaitForSeconds(effectDuration);
            slimeManager.touchDamage = enemyOriginDmg;
            slimeManager.ResetColor();
            isBoostActive = false;
        }

        private IEnumerator BoostDamage()
        {
            isBoostActive = true;
            playerSword.meleeDamage = Mathf.RoundToInt(originalDamage * damageMultiplier);

            // Boost bullet damage for future spawned bullets
            if (bulletPrefab != null)
            {
                PlayerBullet bulletComponent = bulletPrefab.GetComponent<PlayerBullet>();
                if (bulletComponent != null)
                {
                    bulletComponent.damage = Mathf.RoundToInt(originalBulletDamage * damageMultiplier);
                }
            }

            Debug.Log("Damage Boosted: Sword = " + playerSword.meleeDamage + ", Bullet = " + bulletPrefab.GetComponent<PlayerBullet>().damage);

            yield return new WaitForSeconds(effectDuration);

            // Revert back to normal damage
            playerSword.meleeDamage = originalDamage;

            if (bulletPrefab != null)
            {
                PlayerBullet bulletComponent = bulletPrefab.GetComponent<PlayerBullet>();
                if (bulletComponent != null)
                {
                    bulletComponent.damage = originalBulletDamage;
                }
            }

            isBoostActive = false;
            Debug.Log("Damage boost expired. Back to normal.");
        }
    }
}
