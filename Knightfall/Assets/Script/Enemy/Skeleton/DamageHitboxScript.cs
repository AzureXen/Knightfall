using System;
using UnityEngine;

public class DamageHitboxScript : MonoBehaviour
{
    private Boolean hitBoxActive = false;
    public int damage = 5;
    public Vector3 damageSourcePosition = Vector3.zero;

    public float knockbackForce = 1f;
    public float knockbackDuration = 1f;

    public Boolean showHitbox = true;
    public GameObject attackHitbox;

    public Boolean playerIsInHitbox = false;

    // Only damage player once.
    private Boolean damagedPlayer = false;

    private PlayerManager playerManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        damageSourcePosition = (transform.position + transform.right * (transform.localScale.x));
    }

    // Update is called once per frame
    void Update()
    {
        attackHitbox.SetActive(true);

        if (playerIsInHitbox && !damagedPlayer && hitBoxActive)
        {
            playerManager.TakeMeleeHit(damage, damageSourcePosition, knockbackForce, knockbackDuration, null);
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
