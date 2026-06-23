using System;
using UnityEngine;
/// <summary>
/// EnemyManager inherits from EntityManager
/// </summary>
public class GoblinManager : EnemyManager
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        EntityName = "Goblin";
        entityMovement = GetComponent<GoblinMovement>();
        isKnockingBack = false;

        touchDamageEnabled = true;
        touchDamage = 5;
        touchKnockbackForce = 5;
        touchKnockbackDuration = 0.2f;

        if (entityHealth != null)
        {
            entityHealth.OnDeath += HandleDeathEvent;
        }

        goldDrop = 15;
    }

    protected override void Die()
    {
        Debug.Log("Killed: " + EntityName);
        base.Die();
    }
}
