using System.Collections;
using Assets.Script.Enemy.Slime;
using UnityEngine;

namespace Assets.Script.Potion
{
    public class DamageBooster : MonoBehaviour
    {
        public float damageMultiplier = 2f;  // Multiplier for increased damage
        public float effectDuration = 5f;    // Duration of the effect in seconds

        public GameObject player;
        private PlayerSword playerSword;
        private int originalDamage;
        public bool isBoostActive = false;

        public GameObject bulletPrefab;
        private PlayerBullet bulletComponent;
        private int originalBulletDamage;

        private SlimeManager slimeManager;
        private int enemyOriginDmg;

        void Start()
        {
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
                playerSword = player.GetComponent<PlayerSword>();

                if (playerSword != null)
                {
                    originalDamage = playerSword.meleeDamage;
                }
                else
                {
                    Debug.LogError("PlayerSword not found in Player's children!");
                }
            }


            // Store bullet original damage (only if prefab is assigned)
            if (bulletPrefab != null)
            {
                bulletComponent = bulletPrefab.GetComponent<PlayerBullet>();
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
            slimeManager.SetSlimeColor(new Color(255f / 255f, 100f / 255f, 50f / 255f));
            yield return new WaitForSeconds(effectDuration);
            slimeManager.touchDamage = enemyOriginDmg;
            slimeManager.ResetColor();
            isBoostActive = false;
        }

        private IEnumerator BoostDamage()
        {
            isBoostActive = true;
            playerSword.meleeDamage = Mathf.RoundToInt(originalDamage * damageMultiplier);
            bulletComponent.damage = Mathf.RoundToInt(originalBulletDamage * damageMultiplier);

            Debug.Log("Damage Boosted: Sword = " + playerSword.meleeDamage + ", Bullet = " + bulletPrefab.GetComponent<PlayerBullet>().damage);

            yield return new WaitForSeconds(effectDuration);

            // Revert back to normal damage
            playerSword.meleeDamage = originalDamage;
            bulletComponent.damage = originalBulletDamage;
            

            isBoostActive = false;
            Debug.Log("Damage boost expired. Back to normal.");
        }
    }
}
