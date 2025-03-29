using System;
using UnityEngine;

public class BossDamageHitboxScript : MonoBehaviour
{
    private bool hitBoxActive = false;
    public int damage = 10;
    public Vector3 damageSourcePosition = Vector3.zero;

    public float knockbackForce = 1f;
    public float knockbackDuration = 1f;

    public bool playerIsInHitbox = false;

    private bool damagedPlayer = false;
    private PlayerManager playerManager;
    private Collider2D hitboxCollider;

    void Start()
    {
        playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        damageSourcePosition = transform.position;
        hitboxCollider = GetComponent<Collider2D>();
        hitboxCollider.enabled = false; // Start disabled
    }

    void Update()
    {
        if (playerIsInHitbox && !damagedPlayer && hitBoxActive)
        {
            playerManager.TakeMeleeHit(damage, damageSourcePosition, knockbackForce, knockbackDuration, null);
            damagedPlayer = true; // Ensure the player is hit only once per activation
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsInHitbox = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsInHitbox = false;
        }
    }

    private void OnEnable()
    {
        hitBoxActive = true;
        damagedPlayer = false;
    }

    private void OnDisable()
    {
        hitBoxActive = false;
        damagedPlayer = false;
    }
}