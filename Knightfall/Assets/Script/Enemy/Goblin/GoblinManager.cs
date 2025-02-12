using System;
using UnityEngine;
/// <summary>
/// EnemyManager inherits from EntityManager
/// </summary>
public class GoblinManager : EntityManager
{
    [SerializeField] private Boolean touchDamageEnabled = true;
    public int touchDamage = 5;
    public int touchKnockbackForce = 5;
    public float touchKnockbackDuration = 1.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        entityHealth = GetComponent<Health>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        entityMovement = GetComponent<GoblinMovement>();
        isKnockingBack = false;
        takeDamageCooldown = 0.2f;
        flashDuration = 2.5f;
        EntityName = "Goblin";
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
