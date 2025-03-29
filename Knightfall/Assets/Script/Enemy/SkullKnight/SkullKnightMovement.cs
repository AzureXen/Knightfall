using System;
using System.Collections;
using UnityEngine;

public class SkullKnightMovement : EntityMovement
{
    private Transform player;
    private Rigidbody2D rb;
    private bool canMove = true;
    private float moveSpeed = 8f;

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
        rb.linearVelocity = direction * moveSpeed;
        FlipSprite(direction);
    }

    public void StopMoving()
    {
        rb.linearVelocity = Vector2.zero;
    }

    private void FlipSprite(Vector2 direction)
    {
        if (direction.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (direction.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
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