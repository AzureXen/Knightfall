using System;
using UnityEngine;

public class RoninMovement : EntityMovement
{
    public Boolean isMoving = false;
    public Boolean isChasing = false;
    public Boolean canMove = true;

    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float chaseSpeed = 7f;
    [SerializeField] private float idealDistance = 5f;
    [SerializeField] private float directionChangeInterval = 2f;
    [SerializeField] private float distanceThreshold = 0.5f; // How close to ideal distance before switching to circular movement

    private Transform player;
    private Rigidbody2D rb;
    private float directionTimer;
    private int currentSideDirection = 1; // 1 for right, -1 for left

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FixedUpdate()
    {
        if (!canMove || player==null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (isMoving)
        {
            if (isChasing)
            {
                ChasePlayer();
            }
            else
            {
                StrategicMovement();
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void ChasePlayer()
    {
        Vector2 directionToPlayer = ((Vector2)player.position - (Vector2)transform.position).normalized;
        rb.linearVelocity = directionToPlayer * chaseSpeed;
    }

    private void StrategicMovement()
    {
        Vector2 directionToPlayer = ((Vector2)player.position - (Vector2)transform.position);
        float currentDistance = directionToPlayer.magnitude;
        directionToPlayer.Normalize();

        // Update side movement direction timer
        directionTimer -= Time.fixedDeltaTime;
        if (directionTimer <= 0)
        {
            currentSideDirection *= -1;
            directionTimer = directionChangeInterval;
        }

        if (currentDistance < idealDistance - distanceThreshold)
        {
            // If too close, boss move away
            // Moving away uses half of moveSpeed only. 
            rb.linearVelocity = -directionToPlayer * moveSpeed * 0.5f;
        }
        else if (currentDistance > idealDistance + distanceThreshold)
        {
            // If too far, boss will move closer
            rb.linearVelocity = directionToPlayer * moveSpeed;
        }
        else
        {
            // At ideal distance - circle around player
            Vector2 circularDirection = new Vector2(-directionToPlayer.y, directionToPlayer.x) * currentSideDirection;
            rb.linearVelocity = circularDirection * moveSpeed;
        }
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