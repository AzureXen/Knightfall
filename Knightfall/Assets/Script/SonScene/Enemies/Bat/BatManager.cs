using System;
using UnityEngine;

public class BatManager : EntityManager
{
    [SerializeField] private Boolean touchDamageEnabled = true;
    public int touchDamage = 5;
    public float touchKnockbackForce = 0.5f;
    public float touchKnockbackDuration = 0.5f;

    //private EnemyHealth batHealth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        entityHealth = GetComponent<EnemyHealth>();
        //batHealth = GetComponent<EnemyHealth>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        entityMovement = GetComponent<BatMovement>();
        isKnockingBack = false;
        takeDamageCooldown = 0.2f;
        flashDuration = 2.5f;
        EntityName = "Bat";
        defaultColor = new Color(1, 1, 1, 1);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        GameObject target = collision.gameObject;
        if (target.CompareTag("Player") && touchDamageEnabled)
        {
            EntityManager targetManager = target.GetComponent<EntityManager>();
            targetManager.TakeMeleeHit(touchDamage, transform.position, touchKnockbackForce, touchKnockbackDuration, this);
        }
    }
}
