using System;
using System.Collections;
using UnityEngine;

public class AssassinMovement : EntityMovement
{
    public GameObject player;
    [SerializeField] public float moveSpeed = 2;
    public Boolean canMove = true;
     public Vector2 moveDir;

    private Animator animator;
    private SpriteRenderer sr;

    [SerializeField] private float minAttackRange = 1.5f; // Attack when within this range
    [SerializeField] private float retreatRange = 0.8f;
    [SerializeField] private float maxChaseRange = 5f; // Enemy starts chasing if player is within this range
    [SerializeField] private float patrolDistance = 3f; // Distance to patrol left and right
    [SerializeField] private float patrolSpeed = 1.5f;

    private Vector3 patrolLeftPoint;
    private Vector3 patrolRightPoint;
    private Vector3 currentPatrolTarget;
    public bool isPatrolling = true;
    public bool isEnraged = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(FindPlayer());
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        // Set patrol starting point
        patrolLeftPoint = new Vector3(transform.position.x - patrolDistance, transform.position.y, transform.position.z);
        patrolRightPoint = new Vector3(transform.position.x + patrolDistance, transform.position.y, transform.position.z);


        // Initially patrol toward the right point
        currentPatrolTarget = patrolRightPoint;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (player != null && canMove)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
            if (isEnraged)
            {
                ChasePlayer(distanceToPlayer);
                isPatrolling = false;
                return;
            }

            

            if (distanceToPlayer < maxChaseRange) // Stop patrolling and chase player
            {
                isPatrolling = false;
                ChasePlayer(distanceToPlayer);
            }
            else
            {
                isPatrolling = true;
                Debug.Log("patrol when player is detect");
                PatrolBetweenPoints();
            }
        }
        else if (canMove)
        {
            Debug.Log("patrol when player is not detect");
            // If no player is found yet, still patrol
            PatrolBetweenPoints();
        }
    }

    private void ChasePlayer(float distanceToPlayer)
    {
        if (distanceToPlayer > minAttackRange) // Move closer if outside attack range
        {
            moveDir = (player.transform.position - transform.position).normalized;
        }
        //else if (distanceToPlayer < retreatRange) // Move back if too close
        //{
        //    moveDir = (transform.position - player.transform.position).normalized;
        //}
        else
        {
            moveDir = Vector2.zero; // Stop moving
        }

        transform.position += (Vector3)(moveDir * moveSpeed * Time.deltaTime);
        UpdateAnimation();
    }

    private void PatrolBetweenPoints()
    {
        // Calculate direction to current patrol target
        Vector3 directionToTarget = (currentPatrolTarget - transform.position).normalized;
        moveDir = new Vector2(directionToTarget.x, directionToTarget.y);

        // Move toward the current patrol point
        transform.position += (Vector3)(moveDir * patrolSpeed * Time.deltaTime);


        // Check if close enough to current target to switch directions
        float distanceToTarget = Vector3.Distance(transform.position, currentPatrolTarget);
        if (distanceToTarget < 0.5f)
        {
            // Switch target points
            currentPatrolTarget = (currentPatrolTarget == patrolRightPoint) ? patrolLeftPoint : patrolRightPoint;

            // Update sprite direction
            sr.flipX = currentPatrolTarget == patrolLeftPoint;
        }

        UpdateAnimation();
    }

    private void Flip()
    {
        sr.flipX = !sr.flipX;
    }

    private IEnumerator FindPlayer()
    {
        while (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            yield return null;
        }
    }

    public override void DisableMovement()
    {
        canMove = false;
        moveDir = Vector2.zero;
        UpdateAnimation();
    }
    public override void EnableMovement()
    {
        canMove = true;
    }

    private void UpdateAnimation()
    {
        if (animator == null) return;

        // If moving, play running animation; otherwise, idle
        animator.SetBool("IsWalking", moveDir.magnitude > 0.1f);

        // Flip sprite if moving left/right
        if (moveDir.x > 0.1f)
            sr.flipX = false;
        else if (moveDir.x < -0.1f)
            sr.flipX = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(patrolLeftPoint, 0.2f);
        Gizmos.DrawWireSphere(patrolRightPoint, 0.2f);
        Gizmos.DrawLine(patrolLeftPoint, patrolRightPoint);
    }
}
