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

    private Animator animator;
    private Health health;
    private Transform player;
    private Health playerHealth;
    private SpriteRenderer sr;

    private bool isAttacking = false;
    private bool isDead = false;
    private float lastAttackTime = -999f;

    private int lastHealth = -1;

    public bool IsAttacking => isAttacking;

    void Start()
    {
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
        sr = GetComponent<SpriteRenderer>();
        lastHealth = health.health;

        StartCoroutine(FindPlayer());
    }

    void Update()
    {
        if (isDead || player == null) return;

        // === Death ===
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

        // === Flip theo hướng player ===
        Vector3 scale = transform.localScale;
        if (player.position.x > transform.position.x)
            scale.x = -Mathf.Abs(scale.x); // ← đổi chỗ
        else
            scale.x = Mathf.Abs(scale.x);

        transform.localScale = scale;

        // === Hit animation khi máu giảm ===
        if (health.health < lastHealth)
        {
            animator.SetTrigger("hit");
            lastHealth = health.health;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        // === Walking condition ===
        animator.SetBool("isWalking", !isAttacking && distance > attackRange);

        // === Attack condition ===
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
