using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Health), typeof(SpriteRenderer))]
public class MushroomAnimatorController : MonoBehaviour
{
    [Header("Combat Settings")]
    public float attackRange = 1.2f;
    public float attackCooldown = 2f;
    public int attackDamage = 10;
    public float damageDelay = 0.4f;

    [Header("Movement Control")]
    public float stopDistanceFactor = 0.8f;

    [Header("Angry Mode Settings")]
    public Color angryColor = new Color(0.8f, 0.2f, 0.2f, 1f); // đỏ đậm
    public float speedMultiplier = 1.5f;

    private Animator animator;
    private Health health;
    private Transform player;
    private Health playerHealth;
    private SpriteRenderer sr;
    private NhatMovement movement;

    private bool isAttacking = false;
    private bool isDead = false;
    private bool hasEnraged = false;
    private float lastAttackTime = -999f;
    private int lastHealth = -1;

    public bool IsAttacking => isAttacking;

    void Start()
    {
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
        sr = GetComponent<SpriteRenderer>();
        movement = GetComponent<NhatMovement>();
        lastHealth = health.health;

        StartCoroutine(FindPlayer());
    }

    void Update()
    {
        if (isDead || player == null) return;

        // === Check death ===
        if (health.health <= 0)
        {
            if (!isDead)
            {
                isDead = true;
                animator.SetBool("isDead", true);
                StartCoroutine(DestroyAfterDeath());
            }
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        // === Flip ===
        Vector3 scale = transform.localScale;
        scale.x = (player.position.x > transform.position.x) ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;

        // === Trigger hit animation ===
        if (health.health < lastHealth)
        {
            animator.SetTrigger("hit");
            lastHealth = health.health;
        }

        // === Enrage when under 50% HP ===
        if (!hasEnraged && health.health <= health.maxHealth * 0.5f)
        {
            hasEnraged = true;
            sr.color = angryColor;
            if (movement != null)
            {
                movement.moveSpeed *= speedMultiplier;
            }
        }

        // === Stop moving when close ===
        float stopDistance = attackRange * stopDistanceFactor;
        bool shouldStop = distance <= stopDistance;

        if (movement != null)
            movement.canMove = !shouldStop && !isAttacking;

        animator.SetBool("isWalking", !shouldStop && !isAttacking);

        // === Attack ===
        if (!isAttacking && distance <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        animator.SetTrigger("attack");
        lastAttackTime = Time.time;

        yield return new WaitForSeconds(damageDelay);

        if (player != null && Vector2.Distance(transform.position, player.position) <= attackRange + 0.3f)
        {
            playerHealth?.TakeDamage(attackDamage);
        }

        isAttacking = false;
    }

    IEnumerator DestroyAfterDeath()
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }

    IEnumerator FindPlayer()
    {
        while (player == null)
        {
            GameObject found = GameObject.FindGameObjectWithTag("Player");
            if (found != null)
            {
                player = found.transform;
                playerHealth = found.GetComponent<Health>();
            }
            yield return null;
        }
    }
}
