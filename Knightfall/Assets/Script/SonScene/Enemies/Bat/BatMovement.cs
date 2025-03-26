using System;
using System.Collections;
using UnityEngine;

public class BatMovement : EntityMovement
{
    public GameObject player;
    [SerializeField] private float moveSpeed = 2;
    public Boolean canMove = true;
    public bool isChasing = false;
    [HideInInspector] public Vector2 moveDir;


    [SerializeField] private float detectionRange = 5f; // When to start chasing
    [SerializeField] private float patrolSpeed = 1.5f;
    [SerializeField] private float patrolDistance = 3f; // Distance to patrol left and right

    private Vector3 patrolLeftPoint;
    private Vector3 patrolRightPoint;
    private Vector3 currentPatrolTarget;

    private SpriteRenderer sr;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(FindPlayer());

        sr = GetComponent<SpriteRenderer>();

        // Set patrol starting points
        patrolLeftPoint = new Vector3(transform.position.x - patrolDistance, transform.position.y, transform.position.z);
        patrolRightPoint = new Vector3(transform.position.x + patrolDistance, transform.position.y, transform.position.z);
        currentPatrolTarget = patrolRightPoint; // Start patrolling towards right
    }

    private void FixedUpdate()
    {
        if (player != null && canMove)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

            if (!isChasing && distanceToPlayer < detectionRange)
            {
                isChasing = true; // Switch permanently to chase mode
            }

            if (isChasing)
            {
                ChasePlayer();
            }
            else
            {
                PatrolBetweenPoints();
            }
        }
        else if (canMove)
        {
            PatrolBetweenPoints();
        }
    }

    private void ChasePlayer()
    {
        moveDir = (player.transform.position - transform.position).normalized;
        transform.position += (Vector3)(moveDir * moveSpeed * Time.deltaTime);

        FlipSprite(moveDir.x);
    }

    private void PatrolBetweenPoints()
    {
        Vector3 directionToTarget = (currentPatrolTarget - transform.position).normalized;
        moveDir = new Vector2(directionToTarget.x, directionToTarget.y);

        transform.position += (Vector3)(moveDir * patrolSpeed * Time.deltaTime);

        float distanceToTarget = Vector3.Distance(transform.position, currentPatrolTarget);
        if (distanceToTarget < 0.5f)
        {
            // Switch patrol direction
            currentPatrolTarget = (currentPatrolTarget == patrolRightPoint) ? patrolLeftPoint : patrolRightPoint;

            FlipSprite(currentPatrolTarget.x - transform.position.x);
        }
    }

    private IEnumerator FindPlayer()
    {
        while (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            yield return null;
        }
    }

    //private void FlipSprite(float direction)
    //{
    //    if (direction > 0.1f)
    //        sr.flipX = false; // Face right
    //    else if (direction < -0.1f)
    //        sr.flipX = true; // Face left
    //}

    private void FlipSprite(float direction)
    {
        sr.flipX = !sr.flipX;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isChasing && collision.gameObject.CompareTag("Wall"))
        {
            currentPatrolTarget = (currentPatrolTarget == patrolRightPoint) ? patrolLeftPoint : patrolRightPoint;
            FlipSprite(currentPatrolTarget.x - transform.position.x);
        }
    }


    public override void DisableMovement()
    {
        canMove = false;
    }
    public override void EnableMovement()
    {
        canMove = true;
    }
}
