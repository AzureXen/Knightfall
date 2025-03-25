using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BanditAnimatorController : MonoBehaviour
{
    private Animator animator;
    private NhatMovement movement;
    private NhatManager manager;
    private Health health;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Transform player;

    private bool isDead = false;

    private enum AnimStates
    {
        Idle = 0,
        Run = 1,
        CombatIdle = 2
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        movement = GetComponent<NhatMovement>();
        manager = GetComponent<NhatManager>();
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (isDead) return;

        // === Flip sprite hướng về phía Player ===
        if (player != null)
        {
            if (player.position.x > transform.position.x)
                sr.flipX = false;
            else
                sr.flipX = true;
        }

        // === Set Grounded và AirSpeed cho jump/fall ===
        animator.SetBool("Grounded", true); // Nếu có hệ thống nhảy, bạn cần thay đổi giá trị này
        animator.SetFloat("AirSpeed", rb.linearVelocity.y);

        // === Set trạng thái chạy hoặc idle ===
        if (movement != null && movement.enabled)
        {
            float dist = Vector2.Distance(transform.position, player.position);
            if (dist > 1.5f)
            {
                animator.SetInteger("AnimState", (int)AnimStates.Run);
            }
            else
            {
                animator.SetInteger("AnimState", (int)AnimStates.CombatIdle);
            }
        }

        // === Kiểm tra bị chết ===
        if (health.health <= 0 && !isDead)
        {
            isDead = true;
            animator.SetTrigger("Death");
        }
    }

    // Gọi khi tấn công (ví dụ từ script NhatAttack hoặc timer)
    public void PlayAttack()
    {
        if (!isDead)
            animator.SetTrigger("Attack");
    }

    // Gọi khi bị thương
    public void PlayHit()
    {
        if (!isDead)
            animator.SetTrigger("Hurt");
    }

    // Gọi khi hồi phục
    public void PlayRecover()
    {
        if (!isDead)
            animator.SetTrigger("Recover");
    }

    // Gọi khi nhảy (nếu có hệ thống nhảy)
    public void PlayJump()
    {
        if (!isDead)
            animator.SetTrigger("Jump");
    }
}
