using System;
using System.Collections;
using UnityEngine;

public class DwarfManager : EntityManager
{
    public int attackDamage = 8;
    public float attackCooldown = 1.5f; // Cooldown for attacking
    private Animator animator;
    private DwarfMovement movement;
    private float nextAttackTime = 0;

    [SerializeField] private float minAttackRange = 1.5f; // Attack 

    public override void Start()
    {
        entityHealth = GetComponent<EnemyHealth>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<DwarfMovement>();
        animator = GetComponent<Animator>();

        isKnockingBack = false;
        takeDamageCooldown = 0.2f;
        flashDuration = 2.5f;
        EntityName = "Dwarf";
        defaultColor = new Color(1, 1, 1, 1);
    }

    private void Update()
    {
        if (movement.player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, movement.player.transform.position);

        if (Time.time >= nextAttackTime && distanceToPlayer < minAttackRange) // Attack when in range
        {
            StartCoroutine(AttackPlayerForDwarf());
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    private IEnumerator AttackPlayerForDwarf()
    {
        animator.SetBool("IsAttacking", true);

        yield return new WaitForSeconds(0.5f); // Delay for attack animation

        if (movement.player != null)
        {
            EntityManager playerManager = movement.player.GetComponent<EntityManager>();
            if (playerManager != null)
            {
                playerManager.TakeMeleeHit(attackDamage, transform.position, 0.5f, 0.5f, this);
            }
        }

        yield return new WaitForSeconds(0.5f);

        animator.SetBool("IsAttacking", false);
    }
}
