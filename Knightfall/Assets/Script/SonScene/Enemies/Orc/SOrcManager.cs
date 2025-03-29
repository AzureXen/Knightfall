using UnityEngine;
using System.Collections;

public class SOrcManager : EntityManager
{
    [SerializeField] private int lightAttackDamage = 5;
    [SerializeField] private int heavyAttackDamage = 10;
    public float attackCooldown = 1.5f;

    private Animator animator;
    private SOrcMovement movement;
    private bool isAttacking = false;
    private float nextAttackTime = 0;

    [SerializeField] private float attackRange = 3.5f;
    [SerializeField] private float chaseRange = 5f; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        entityHealth = GetComponent<EnemyHealth>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<SOrcMovement>();
        animator = GetComponent<Animator>();
        isKnockingBack = false;
        takeDamageCooldown = 0.2f;
        flashDuration = 2.5f;
        EntityName = "Orc";
        defaultColor = new Color(1, 1, 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (movement.player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, movement.player.transform.position);
        Debug.Log("distance" + distanceToPlayer);

        if (Time.time >= nextAttackTime && distanceToPlayer < attackRange)
        {
            //animator.SetBool("IsRunning", false);
            StartCoroutine(AttackPlayer());
            nextAttackTime = Time.time + attackCooldown;
            //animator.SetBool("IsRunning", true);
        }
    }

    private IEnumerator AttackPlayer()
    {
        Debug.Log("called");
        isAttacking = true;
        //movement.canMove = false;

        // Randomly choose an attack animation
        int attackType = Random.Range(0, 2); // 0 = light attack, 1 = heavy attack
        animator.SetBool("IsRunning", false);
        if (attackType == 0)
        {
            animator.SetTrigger("Attack1");
            yield return new WaitForSeconds(0.5f); // Wait for animation
            DealDamage(lightAttackDamage);
        }
        else
        {
            animator.SetTrigger("Attack2");
            yield return new WaitForSeconds(0.7f); // Wait for animation
            DealDamage(heavyAttackDamage);
        }

        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
        animator.SetBool("IsRunning", true);
        //movement.canMove = true;
    }

    private void DealDamage(int damage)
    {
        if (movement.player != null)
        {
            EntityManager playerManager = movement.player.GetComponent<EntityManager>();
            if (playerManager != null)
            {
                playerManager.TakeMeleeHit(damage, transform.position, 0.5f, 0.5f, this);
            }
        }
    }
}
