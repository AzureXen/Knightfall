using System;
using UnityEngine;
/// <summary>
/// EnemyManager inherits from EntityManager
/// </summary>
public class EnemyManager : EntityManager
{
    [SerializeField] private Boolean touchDamageEnabled = true;
    public int touchDamage = 5;
    public float touchKnockbackForce = 5f;
    public float touchKnockbackDuration = 1.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        entityHealth = GetComponent<Health>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        // TODO: Implement Enemy Movement, and assign the enttiyMovement.
        entityMovement = null;
        isKnockingBack = false;
        takeDamageCooldown = 0.2f;
        flashDuration = 2.5f;
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        GameObject target = collision.gameObject;
        if (target.CompareTag("Player") && touchDamageEnabled)
        {
            PlayerManager targetManager = target.GetComponent<PlayerManager>();
            targetManager.TakeHitKnockback(touchDamage, transform.position, touchKnockbackForce, touchKnockbackDuration);
        }
    }
}
