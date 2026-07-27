using System;
using UnityEngine;

namespace Assets.Script.Enemy.FlyingEye
{
    public class FlyingEyeManager : EnemyManager
    {
        private Animator animator;
        [SerializeField] private float attackRange = 10f;
        [SerializeField] private float fireRate = 2f;
        private Health health;

        [SerializeField] private float healChance = 0.3f; 
        [SerializeField] private int healAmount = 20;

        public event Action<GameObject> OnPlayerFound;
        protected GameObject player;
        
        public override void Start()
        {
            entityHealth = GetComponent<Health>();
            sr = GetComponent<SpriteRenderer>();
            rb = GetComponent<Rigidbody2D>();
            entityMovement = GetComponent<FlyingEyeMovement>();
            
            EntityName = "FlyingEye";
            defaultColor = new Color(1, 1, 1, 1);
            animator = GetComponent<Animator>();
            Health health = GetComponent<Health>();

            touchDamageEnabled = false;
            touchDamage = 10;
            touchKnockbackForce = 10;
            touchKnockbackDuration = 1.5f;
            isKnockingBack = false;
            takeDamageCooldown = 0.2f;
            flashDuration = 2.5f;

            player = GameManagerScript.Instance.player;
            if(player!=null)
            {
                OnPlayerFound?.Invoke(player);
            }
            else
            {
                GameManagerScript.OnPlayerReady += RegisterPlayer;
            }
        }

        private void RegisterPlayer(GameObject player)
        {
            this.player = player;
            OnPlayerFound?.Invoke(player);
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

    }
}