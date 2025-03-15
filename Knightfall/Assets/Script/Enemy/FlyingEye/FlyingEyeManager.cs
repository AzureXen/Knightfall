using System;
using UnityEngine;

namespace Assets.Script.Enemy.FlyingEye
{
    public class FlyingEyeManager : EntityManager
    {
        [SerializeField] private Boolean touchDamageEnabled = true;
        public int touchDamage = 10;
        public int touchKnockbackForce = 10;
        public float touchKnockbackDuration = 1.5f;
        private Animator animator;
        [SerializeField] private float attackRange = 10f;
        [SerializeField] private float fireRate = 2f;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void Start()
        {
            entityHealth = GetComponent<Health>();
            sr = GetComponent<SpriteRenderer>();
            rb = GetComponent<Rigidbody2D>();
            entityMovement = GetComponent<FlyingEyeMovement>();
            isKnockingBack = false;
            takeDamageCooldown = 0.2f;
            flashDuration = 2.5f;
            EntityName = "FlyingEye";
            defaultColor = new Color(1, 1, 1, 1);
            animator = GetComponent<Animator>();
        }

        // Update is called once per frame
        private void OnCollisionStay2D(Collision2D collision)
        {
            GameObject target = collision.gameObject;
            if (target.CompareTag("Player") && touchDamageEnabled)
            {
                EntityManager targetManager = target.GetComponent<EntityManager>();
                targetManager.TakeMeleeHit(touchDamage, transform.position, touchKnockbackForce, touchKnockbackDuration, this);
            }
        }
    }
}