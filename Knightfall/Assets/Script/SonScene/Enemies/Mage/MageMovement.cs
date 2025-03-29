using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MageMovement : EntityMovement
{
    [SerializeField] public float rangeActive = 10f;
    [SerializeField] public bool isActive = false;

    public GameObject player;
    [SerializeField] public float moveSpeed = 2f;
    public bool canMove = true;
    public Vector2 moveDir;

    [SerializeField] private float safeDistance = 10f;
    [SerializeField] private float retreatDistance = 10f;
    [SerializeField] private float detectionRange = 15f;

    private Animator animator;
    private SpriteRenderer sr;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(FindPlayer());
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (player != null && canMove)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

            if (distanceToPlayer < rangeActive) isActive = true;
            if (!isActive) return;

            if (distanceToPlayer < detectionRange)
            {
                if (distanceToPlayer > safeDistance)
                {
                    moveDir = (player.transform.position - transform.position).normalized;
                }
                else if (distanceToPlayer < retreatDistance)
                {
                    moveDir = (transform.position - player.transform.position).normalized;
                }
                else
                {
                    moveDir = Vector2.zero;
                }

                transform.position += (Vector3)(moveDir * moveSpeed * Time.deltaTime);
                sr.flipX = !(moveDir.x < 0);
            }
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
    }
    public override void EnableMovement()
    {
    }
}
