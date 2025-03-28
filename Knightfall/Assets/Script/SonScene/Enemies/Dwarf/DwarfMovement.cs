using System.Collections;
using UnityEngine;

public class DwarfMovement : EntityMovement
{
    public GameObject player;
    [SerializeField] private float moveSpeed = 2f;
    public bool canMove = true;
    private SpriteRenderer sr;

    [SerializeField] private float detectionRange = 6f; // Chase range
    [SerializeField] private float patrolSpeed = 1.5f;
    [SerializeField] private float patrolDistance = 3f; // Patrol range

    private Vector3 patrolLeftPoint;
    private Vector3 patrolRightPoint;
    private Vector3 currentPatrolTarget;
    private bool isChasing = false;

    void Start()
    {
        StartCoroutine(FindPlayer());
        sr = GetComponent<SpriteRenderer>();

        // Set patrol points
        patrolLeftPoint = new Vector3(transform.position.x - patrolDistance, transform.position.y, transform.position.z);
        patrolRightPoint = new Vector3(transform.position.x + patrolDistance, transform.position.y, transform.position.z);
        currentPatrolTarget = patrolRightPoint;
    }

    private void FixedUpdate()
    {
        if (player != null && canMove)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

            if (!isChasing && distanceToPlayer < detectionRange)
            {
                isChasing = true; // Start chasing
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
        Vector2 moveDir = (player.transform.position - transform.position).normalized;
        transform.position += (Vector3)(moveDir * moveSpeed * Time.deltaTime);
        FlipSprite(moveDir.x);
    }

    private void PatrolBetweenPoints()
    {
        Vector3 directionToTarget = (currentPatrolTarget - transform.position).normalized;
        transform.position += (Vector3)(directionToTarget * patrolSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, currentPatrolTarget) < 0.5f)
        {
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

    private void FlipSprite(float direction)
    {
        if ((direction > 0.1f && sr.flipX) || (direction < -0.1f && !sr.flipX))
        {
            sr.flipX = !sr.flipX;
        }
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
