using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class SOrcMovement : EntityMovement
{
    public GameObject player;
    [SerializeField] private float moveSpeed = 2f;
    public bool canMove = true;
    private Animator animator;
    private SpriteRenderer sr;

    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float chaseRange = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(FindPlayer());
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (player != null && canMove)
        {
            //float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

            //if (distanceToPlayer > attackRange && distanceToPlayer < chaseRange)
            //{
                ChasePlayer();
            //}
            
        }

    }

    private void ChasePlayer()
    {
        Vector2 moveDir = (player.transform.position - transform.position).normalized;
        transform.position += (Vector3)(moveDir * moveSpeed * Time.deltaTime);

        animator.SetBool("IsRunning", true);

        // Flip sprite based on movement direction
        sr.flipX = !(moveDir.x < 0);
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
    }
    public override void EnableMovement()
    {

    }
}
