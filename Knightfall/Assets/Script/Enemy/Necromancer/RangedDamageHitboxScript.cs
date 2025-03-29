using System;
using UnityEngine;

public class RangedDamageHitboxScript : MonoBehaviour
{
    private Boolean hitBoxActive = false;
    public int damage = 5;
    public Vector3 damageSourcePosition = Vector3.zero;

    public float knockbackForce = 1f;
    public float knockbackDuration = 1f;

    public Boolean showHitbox = true;
    public GameObject attackHitbox;

    private Boolean playerIsInHitbox = false;

    // Only damage player once.
    private Boolean damagedPlayer = false;

    private Transform player;
    private PlayerManager playerManager;
    private Rigidbody2D rb2;
    private Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        damageSourcePosition = (transform.position + transform.right * (transform.localScale.x));
        rb2 = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        attackHitbox.SetActive(true);

        if (playerIsInHitbox && !damagedPlayer && hitBoxActive)
        {
            playerManager.TakeRangedHit(damage, damageSourcePosition, knockbackForce, knockbackDuration, null);
        }

        Vector2 direction = (player.position - transform.position).normalized;
        if (direction.x > 0) transform.localScale = new Vector3(-1, 1, 1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsInHitbox = true;
            rb2.linearVelocity = Vector2.zero;
            animator.SetTrigger("Impact");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsInHitbox = false;
        }
    }

    public void EnableDamage()
    {
        hitBoxActive = true;
    }

    public void DisableDamage()
    {
        hitBoxActive = false;
        damagedPlayer = false;
    }
    private void OnDestroy()
    {
        playerIsInHitbox = false;
        damagedPlayer = false;
    }
}
