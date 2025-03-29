using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class SkeletonHeadMovement : EntityMovement
{
    private Transform player;
    private Rigidbody2D rb;

    [SerializeField] private float chaseSpeed = 2f;
    [SerializeField] private float approachSpeed = 1.5f;
    public float backOffSpeed = 0.75f;
    private bool canMove = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FixedUpdate()
    {
        if (!canMove)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
    }

    public void NoticedPlayer()
    {
        if (!canMove) return;
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * 0;
        FlipSprite(direction);
    }

    public void ChasePlayer()
    {
        if (!canMove) return;
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * chaseSpeed;
        FlipSprite(direction);
    }

    public void ApproachPlayer()
    {
        if (!canMove) return;
        Vector2 direction = (player.position - transform.position).normalized; // Move towards player
        rb.linearVelocity = direction * approachSpeed;
        FlipSprite(direction);
    }

    public void BackOffPlayer()
    {
        if (!canMove) return;
        Vector2 direction = (transform.position - player.position).normalized; // Move away from player
        if (player.position.y + 0.5f < transform.position.y && player.position.x < transform.position.x)
        {
            direction.y -= 0.5f;
            direction.x += 1f;

        }
        if (player.position.y + 0.5f < transform.position.y && player.position.x > transform.position.x)
        {
            direction.y -= 0.5f;
            direction.x -= 1f;

        }
        if (player.position.y + 0.5f > transform.position.y && player.position.x < transform.position.x)
        {
            direction.y += 0.5f;
            direction.x += 1f;

        }
        if (player.position.y + 0.5f > transform.position.y && player.position.x > transform.position.x)
        {
            direction.y += 0.5f;
            direction.x -= 1f;

        }
        rb.linearVelocity = direction * (backOffSpeed);
        FlipSprite(-direction);
    }

    public void Detected()
    {
        rb.linearVelocity = Vector2.zero;
        Vector2 direction = (player.position - transform.position).normalized;
        FlipSprite(direction);
    }

    public void StopMoving()
    {
        rb.linearVelocity = Vector2.zero;
    }

    private void FlipSprite(Vector2 direction)
    {
        if (direction.x > 0)
            transform.localScale = new Vector3(1, 1, 1); // Face right
        else if (direction.x < 0)
            transform.localScale = new Vector3(-1, 1, 1); // Face left
    }

    public override void DisableMovement()
    {
        canMove = false;
        rb.linearVelocity = Vector2.zero;
    }

    public override void EnableMovement()
    {
        canMove = true;
    }
}