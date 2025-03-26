using System;
using System.Collections;
using UnityEngine;

public class SkeletonManager : EntityManager
{
    [SerializeField] private float stunDuration;

    public override void Start()
    {
        entityHealth = GetComponent<Health>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        entityMovement = GetComponent<SkeletonMovement>();
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
