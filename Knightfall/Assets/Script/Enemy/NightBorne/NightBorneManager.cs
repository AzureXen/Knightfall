using UnityEngine;

public class NightBorneManager : EnemyManager
{
    public override void Start()
    {
        base.Start();
        EntityName = "Night Borne";
        entityMovement = GetComponent<NightBorneMovement>();
        isKnockingBack = false;

        touchDamageEnabled = false;
        touchDamage = 5;
        touchKnockbackForce = 5;
        touchKnockbackDuration = 0.2f;

        if (entityHealth != null)
        {
            entityHealth.OnDeath += HandleDeathEvent;
        }

        goldDrop = 50;
        rb.mass = 10000;
        rb.linearDamping = 10;
    }

    protected override void Die()
    {
        Debug.Log("Killed: " + EntityName);
        base.Die();
        sr.color = Color.black;
    }
}
