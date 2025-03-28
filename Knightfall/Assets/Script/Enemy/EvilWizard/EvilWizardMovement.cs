using UnityEngine;
using UnityEngine.U2D.Animation;
using System.Collections;


public class EvilWizardMovement : EntityMovement
{
    public GameObject player;
    public float speed = 3f;
    public float minDistance = 2f;
    public float detectionRange = 12f;
    private Animator animator;
    private bool isMoving = false;
    private SpriteRenderer spriteRenderer;
    private float lastXPosition;
    private bool isFacingRight = true;
    private Rigidbody2D rb;

    void Start()
    {
        StartCoroutine(FindPlayer());
        animator = GetComponent<Animator>(); 
        spriteRenderer = GetComponent<SpriteRenderer>();
        lastXPosition = transform.position.x;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (player == null) return;
        UpdateDirection();
        Vector3 playerPosition = player.transform.position;
        Vector3 direction = playerPosition - transform.position;
        float distance = direction.magnitude;
        bool shouldMove = false;
        Vector3 moveDirection = Vector3.zero;
        float currentXPosition = transform.position.x;
        bool movingRight = currentXPosition > lastXPosition;

        if(distance <= detectionRange)
        {
            if (distance > minDistance + 0.5f)
            {
                Vector3 moveTo = Vector3.Lerp(transform.position, playerPosition, Time.deltaTime * speed);
                rb.MovePosition(moveTo);
                shouldMove = true;
            }
            else if (distance < minDistance)
            {
                Vector3 moveAway = transform.position - direction.normalized * Time.deltaTime * speed;
                rb.MovePosition(moveAway);
                shouldMove = true;
            }
        }

        //if (shouldMove)
        //{
        //    if (movingRight && !isFacingRight)
        //    {
        //        isFacingRight = true;
        //        spriteRenderer.flipX = false; 
        //    }
        //    else if (!movingRight && isFacingRight)
        //    {
        //        isFacingRight = false;
        //        spriteRenderer.flipX = true; 
        //    }
        //}
        //else
        //{
        //    bool playerIsRight = direction.x > 0;
        //    if (playerIsRight && !isFacingRight)
        //    {
        //        isFacingRight = true;
        //        spriteRenderer.flipX = false;
        //    }
        //    else if (!playerIsRight && isFacingRight)
        //    {
        //        isFacingRight = false;
        //        spriteRenderer.flipX = true;
        //    }
        //}

        //lastXPosition = currentXPosition;

        if (isMoving != shouldMove)
        {
            isMoving = shouldMove;
            animator.SetBool("isRun", isMoving);

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
    public override void DisableMovement()
    {
        enabled = false;
        animator.SetBool("isRun", false);
    }

    public override void EnableMovement()
    {
        enabled = true;
        animator.SetBool("isRun", true);
    }

    public void UpdateDirection()
    {
        Vector3 playerPosition = player.transform.position;
        Vector3 direction = playerPosition - transform.position;
        if(direction.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }
    }
}
