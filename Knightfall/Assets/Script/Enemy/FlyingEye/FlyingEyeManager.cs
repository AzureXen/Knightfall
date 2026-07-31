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

        [SerializeField] private Collider2D enemyBodyCollider;
        private Collider2D playerBodyCollider;

        public event Action OnTouchDamage;

        public override void Start()
        {
            entityHealth = GetComponent<Health>();
            sr = GetComponent<SpriteRenderer>();
            rb = GetComponent<Rigidbody2D>();
            entityMovement = GetComponent<FlyingEyeMovement>();
            touchDamageEnabled = false;
            
            EntityName = "FlyingEye";
            defaultColor = new Color(1, 1, 1, 1);
            animator = GetComponent<Animator>();
            Health health = GetComponent<Health>();

            player = GameManagerScript.Instance.player;
            if(player!=null)
            {
                OnPlayerFound?.Invoke(player);
                playerBodyCollider = player.GetComponent<Collider2D>();
            }
            else
            {
                GameManagerScript.OnPlayerReady += RegisterPlayer;
            }
        }

        private void RegisterPlayer(GameObject player)
        {
            this.player = player;
            playerBodyCollider = player.GetComponent<Collider2D>();
            OnPlayerFound?.Invoke(player);
        }

        protected override void OnCollisionStay2D(Collision2D collision)
        {
            GameObject target = collision.gameObject;
            if (touchDamageEnabled && target.CompareTag("Player"))
            {
                EntityManager targetManager = target.GetComponent<EntityManager>();
                if(targetManager.TakeMeleeHit(touchDamage, transform.position, touchKnockbackForce, touchKnockbackDuration, this))
                {
                    OnTouchDamage?.Invoke();
                }
            }
        }

        public void EnablePlayerCollision()
        {
            if (enemyBodyCollider != null && playerBodyCollider != null)
            {
                Physics2D.IgnoreCollision(enemyBodyCollider, playerBodyCollider, false);
            }
            if (enemyBodyCollider == null) Debug.LogWarning("Cannot find enemy collider");
            if (playerBodyCollider == null) Debug.LogWarning("Cannot find player collider");
        }
        public void DisablePlayerCollision()
        {
            if (enemyBodyCollider != null && playerBodyCollider != null)
            {
                Physics2D.IgnoreCollision(enemyBodyCollider, playerBodyCollider, true);
            }
            if (enemyBodyCollider == null) Debug.LogWarning("Cannot find enemy collider");
            if (playerBodyCollider == null) Debug.LogWarning("Cannot find player collider");
        }
    }
}