using System;
using System.Collections;
using UnityEngine;

public class SkullKnightManager : EntityManager
{
    [SerializeField] private float stunDuration;

    public override void Start()
    {
        entityHealth = GetComponent<UndeadBossHealth>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        entityMovement = GetComponent<SkullKnightMovement>();
        isKnockingBack = false;
        takeDamageCooldown = 0.4f;
        EntityName = "Skeleton";
        defaultColor = new Color(1, 1, 1, 1);

    }

    void Update()
    {
        
    }

    protected override void TakeDamage(int damage)
    {
        entityHealth.TakeDamage(damage);
        if (damage > 0)
        {
            TakeIFrame(takeDamageCooldown);
        }
    }

}
