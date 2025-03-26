using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Health), typeof(SpriteRenderer))]
public class BanditAnimatorController : MonoBehaviour
{
    public float attackRange = 1.2f;
    public float attackCooldown = 2f;
    public int attackDamage = 10;
    public float damageDelay = 0.3f;

    public float stopDistanceFactor = 0.8f;

    private Animator animator;
    private Health health;
    private Transform player;
    private Health playerHealth;
    private SpriteRenderer sr;
    private NhatMovement movement;

    private bool isAttacking = false;
    private bool isDead = false;
    private float lastAttackTime = -999f;
    private int lastHealth = -1;

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

        if (health.health <= 0)
        {
            isDead = true;
            animator.SetTrigger("Death");
            StartCoroutine(DestroyAfterDeath());
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        // === Flip hướng ===
        Vector3 scale = transform.localScale;
        scale.x = (player.position.x > transform.position.x) ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;

        // === Hit animation ===
        if (health.health < lastHealth)
        {
            animator.SetTrigger("Hurt");
            lastHealth = health.health;
        }

        // === Dừng lại nếu gần ===
        float stopDistance = attackRange * stopDistanceFactor;
        bool shouldStop = distance <= stopDistance;
        movement.canMove = !shouldStop && !isAttacking;

        // === Idle / Run ===
        animator.SetInteger("AnimState", shouldStop ? 0 : 2);

        // === Tấn công nếu trong phạm vi ===
        if (!isAttacking && distance <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");
        lastAttackTime = Time.time;

        yield return new WaitForSeconds(damageDelay);

        if (player != null && Vector2.Distance(transform.position, player.position) <= attackRange + 0.2f)
        {
            playerHealth?.TakeDamage(attackDamage);
        }

        isAttacking = false;
    }

    IEnumerator DestroyAfterDeath()
    {
        yield return new WaitForSeconds(2f);
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
