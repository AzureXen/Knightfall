using Assets.Script.SonScene.Enemies.Assassin;
using System;
using System.Collections;
using UnityEngine;

public class AssassinManager : EntityManager
{
    [SerializeField] private Boolean touchDamageEnabled = true;
    public int attackDamage = 5;
    public float attackCooldown = 1.5f; // Cooldown for attacking
    private Animator animator;
    private AssassinMovement movement;

    private bool isAttacking = false;
    private float nextAttackTime = 0;

    [SerializeField] private float minAttackRange = 1.5f; // Attack when within this range
    [SerializeField] private float maxChaseRange = 5f;

    private bool isEnraged = false;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        entityHealth = GetComponent<EnemyHealth>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<AssassinMovement>();
        animator = GetComponent<Animator>();
        isKnockingBack = false;
        takeDamageCooldown = 0.2f;
        flashDuration = 2.5f;
        EntityName = "Assassin";
        defaultColor = new Color(1, 1, 1, 1);
    }

    private void Update()
    {
        if (movement.player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, movement.player.transform.position);

        // Check if health is below 50% and activate rage mode
        if (!isEnraged && entityHealth.health <= entityHealth.maxHealth * 0.5f)
        {
            EnterRageMode();
        }

        if (isEnraged)
        {
            movement.canMove = true;
            movement.isPatrolling = false;
            movement.isEnraged = true;

            if (Time.time >= nextAttackTime) // Add cooldown
            {
                StartCoroutine(ShootAtPlayer());
                nextAttackTime = Time.time + attackCooldown;
            }
            return;
        }

        if (Time.time >= nextAttackTime && distanceToPlayer < minAttackRange) // Attack only if within range
        {
            StartCoroutine(AttackPlayer());
            nextAttackTime = Time.time + attackCooldown;
        }

        // Enable movement when player is within chase range
        if (distanceToPlayer < maxChaseRange)
        {
            movement.canMove = true; // Chase or attack
        }
        else if (!movement.isPatrolling)
        {
            movement.canMove = true; // Continue patrolling when no player is detected
        }


        //if (Time.time >= nextAttackTime && movement.player != null)
        //{
        //    float distanceToPlayer = Vector2.Distance(transform.position, movement.player.transform.position);

        //    if (distanceToPlayer < 1.5f) // If player is close enough, attack
        //    {
        //        StartCoroutine(AttackPlayer());
        //        nextAttackTime = Time.time + attackCooldown;
        //    }
        //}
    }

    private IEnumerator AttackPlayer()
    {
        isAttacking = true;
        animator.SetBool("IsAttacking", true);

        yield return new WaitForSeconds(0.5f); // Delay for attack animation

        // Deal damage
        if (movement.player != null)
        {
            EntityManager playerManager = movement.player.GetComponent<EntityManager>();
            if (playerManager != null)
            {
                playerManager.TakeMeleeHit(attackDamage, transform.position, 0.5f, 0.5f, this);
            }
        }

        yield return new WaitForSeconds(0.5f); // Ensure full attack animation plays

        isAttacking = false;
        animator.SetBool("IsAttacking", false);
    }

    private void EnterRageMode()
    {
        isEnraged = true;
        movement.isEnraged = true;
        movement.moveSpeed *= 1.5f; // Increase speed by 50%
    }

    private IEnumerator ShootAtPlayer()
    {
        //isAttacking = true;
        animator.SetTrigger("Shoot");

        //yield return new WaitForSeconds(0.5f); // Animation delay

        animator.SetTrigger("Shoot"); // Play shooting animation

        // Wait for the animation to finish
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        //if (movement.player != null)
        //{
        //    GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        //    bullet.transform.localScale = new Vector3(8f, 8f, 1f);
        //    bullet.GetComponent<Bullet>().Initialize(movement.player.transform.position);
        //}

        //yield return new WaitForSeconds(0.5f);

        //isAttacking = false;
    }

    public void FireBullet()
    {
        if (movement.player != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            bullet.transform.localScale = new Vector3(8f, 8f, 1f);
            bullet.GetComponent<Bullet>().Initialize(movement.player.transform.position);
        }
    }
}
