using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BanditAnimatorController : MonoBehaviour
{
    private Animator animator;
    private Transform player;
    private NhatMovement movement;
    private Health health;

    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 2f;
    private float nextAttackTime;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        movement = GetComponent<NhatMovement>();
        health = GetComponent<Health>();
    }

    void Update()
    {
        if (health == null || health.health <= 0) return;
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Đổi hướng theo vị trí player
        Vector3 scale = transform.localScale;
        scale.x = (player.position.x < transform.position.x) ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;

        if (distance <= attackRange)
        {
            // Attack nếu cooldown xong
            if (Time.time >= nextAttackTime)
            {
                animator.SetInteger("AnimState", 2); // Attack
                animator.SetTrigger("Attack");

                nextAttackTime = Time.time + attackCooldown;
            }

            // Ngừng di chuyển
            movement.DisableMovement();
        }
        else
        {
            animator.SetInteger("AnimState", 1); // Run
            movement.EnableMovement();
        }
    }

    // Gọi hàm này từ Animation Event Frame nếu muốn gây damage
    public void DealDamage()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, attackRange, LayerMask.GetMask("Player"));
        if (hit != null && hit.CompareTag("Player"))
        {
            EntityManager playerEntity = hit.GetComponent<EntityManager>();
            if (playerEntity != null)
            {
                playerEntity.TakeMeleeHit(10, transform.position, 10, 1f, GetComponent<EntityManager>());
            }
        }
    }
}
