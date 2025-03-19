using System;
using UnityEngine;
using System.Collections;


namespace Assets.Script.Enemy.FlyingEye
{
    public class FlyingEyeMovement : EntityMovement
    {
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private float attackRange = 15f;
        [SerializeField] private float fireRate = 2f;
        public GameObject player;
        [SerializeField] private float moveSpeed = 2;
        public Boolean canMove = true;
        [HideInInspector] public Vector2 moveDir;
        private SpriteRenderer SpriteRenderer;
        private float nextFireTime;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            StartCoroutine(FindPlayer());
            SpriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (player != null)
            {
                if (canMove)
                {
                    moveDir = (player.transform.position - transform.position).normalized;
                    transform.position = Vector2.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);
                    SpriteRenderer.flipX = moveDir.x < 0 ? true : false;
                }
                else moveDir = Vector2.zero;
                if (Vector2.Distance(transform.position, player.transform.position) <= attackRange)
                {
                    if (Time.time >= nextFireTime)
                    {
                        Shoot();
                        nextFireTime = Time.time + fireRate;
                    }
                }
            }
        }

        private IEnumerator FindPlayer()
        {
            while (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player");
                yield return null;
            }
        }

        public override void DisableMovement()
        {
            canMove = false;
        }
        public override void EnableMovement()
        {
            canMove = true;
        }

        private void Shoot()
        {
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            MonsterProjectile projectileScript = projectile.GetComponent<MonsterProjectile>();
            if (projectileScript != null)
            {
                Vector2 direction = (player.transform.position - transform.position).normalized;
                projectileScript.SetDirection(direction);
            }
        }
    }
}