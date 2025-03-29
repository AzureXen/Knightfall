using System;
using System.Collections;
using UnityEngine;

namespace Assets.Script.Enemy.Slime
{
    public class SlimeManager : EntityManager
    {
        [SerializeField] private Boolean touchDamageEnabled = true;
        public int touchDamage = 5;
        public int touchKnockbackForce = 5;
        public float touchKnockbackDuration = 1.5f;
        public bool hasKey = false;
        private bool isBuffed = false;
        private bool isInvincible = false;
        private Coroutine flashCoroutine;

        private SpriteRenderer sr;
        private SlimeMovement entityMovement;
        public GameObject keyPrefab;

        public void Awake()
        {
            entityHealth = GetComponent<Health>();
            sr = GetComponent<SpriteRenderer>();
            rb = GetComponent<Rigidbody2D>();
            entityMovement = GetComponent<SlimeMovement>();
            isKnockingBack = false;
            takeDamageCooldown = 0.2f;
            flashDuration = 2.5f;
            EntityName = "Slime";
            defaultColor = new Color(1, 1, 1, 1);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            GameObject target = collision.gameObject;
            if (target.CompareTag("Player") && touchDamageEnabled)
            {
                EntityManager targetManager = target.GetComponent<EntityManager>();
                targetManager.TakeMeleeHit(touchDamage, transform.position, touchKnockbackForce, touchKnockbackDuration, this);
            }
        }

        protected override void TakeDamage(int damage)
        {
            if (isInvincible) return; 

            if (!isBuffed && hasKey)
            {
                StartCoroutine(EnterSuperBuffState());
                return;
            }
            if (entityHealth.health <= 10)
            {
                if (hasKey && keyPrefab != null)
                {
                    Instantiate(keyPrefab, transform.position, Quaternion.identity);
                    Debug.Log("Key dropped at: " + transform.position);
                }
            }
            entityHealth.TakeDamage(damage);
            if (damage > 0)
            {
                if (flashCoroutine != null)
                {
                    StopCoroutine(flashCoroutine);
                }
                flashCoroutine = StartCoroutine(FlashRed());
                TakeIFrame(takeDamageCooldown);
            }
            
        }

        private IEnumerator EnterSuperBuffState()
        {
            isBuffed = true;
            entityMovement.DisableMovement();

            hasKey = true;
            SuperBuff();

            SetInvincible(true); // Become invincible

            float buffDuration = 2f;
            float elapsedTime = 0f;
            float maxSize = 2f;
            Vector3 originalScale = transform.localScale;

            while (elapsedTime < buffDuration)
            {
                transform.localScale = Vector3.Lerp(originalScale, originalScale * maxSize, elapsedTime / buffDuration);
                sr.color = Color.Lerp(Color.white, Color.red, elapsedTime / buffDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
                
            }

            transform.localScale = originalScale * maxSize;
            sr.color = Color.red;

            
            SetInvincible(false);
            entityMovement.EnableMovement();
        }

        public void SuperBuff()
        {
            if (hasKey)
            {
                Health health = GetComponent<Health>();
                touchDamage *= 3;
                health.maxHealth *= 3;
                health.health = health.maxHealth;
                SizeInflate(1);
                SetSlimeColor(Color.red);
            }
        }

        public void SetInvincible(bool state)
        {
            isInvincible = state;

            if (state)
            {
                if (flashCoroutine == null)
                {
                    flashCoroutine = StartCoroutine(FlashWhileInvincible());
                }
            }
            else
            {
                if (flashCoroutine != null)
                {
                    StopCoroutine(flashCoroutine);
                    flashCoroutine = null;
                }
                sr.color = defaultColor;
            }
        }

        private IEnumerator FlashWhileInvincible()
        {
            while (isInvincible)
            {
                sr.color = Color.white;
                yield return new WaitForSeconds(0.1f);
                sr.color = new Color(1f, 1f, 1f, 0.5f);
                yield return new WaitForSeconds(0.1f);
            }
        }

        public void ResetStatus()
        {
            entityHealth.maxHealth = entityHealth.defaultHealth;
            touchDamage = 5;
            transform.localScale = Vector3.one;
            ResetColor();
            SetInvincible(false);
        }

        public void SetSlimeColor(Color newColor)
        {
            sr.color = newColor;
            defaultColor = newColor;
        }

        public void ResetColor()
        {
            defaultColor = new Color(1, 1, 1, 1);
            sr.color = defaultColor;
        }

        public void SizeInflate(float multi)
        {
            transform.localScale *= multi;
        }
    }
}
