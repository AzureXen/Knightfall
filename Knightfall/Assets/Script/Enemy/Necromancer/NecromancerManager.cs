using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static RoninAction;

public class NecromancerManager : EntityManager
{
    private NecromancerAttack necromancerAttack;

    [SerializeField] private float maxMana;
    [SerializeField] private float curMana;
    [SerializeField] private float manaExhaustDuration;

    private Boolean isOOM = false;

    private NecromancerActions actions;

    public override void Start()
    {
        entityHealth = GetComponent<Health>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        entityMovement = GetComponent<NecromancerMovement>();
        isKnockingBack = false;
        takeDamageCooldown = 0.4f;
        EntityName = "Necromancer";
        defaultColor = new Color(1, 1, 1, 1);

        necromancerAttack = GetComponent<NecromancerAttack>();
        actions = GetComponent<NecromancerActions>();
    }

    void Update()
    {
        if (curMana <= 0)
        {
            StartCoroutine(ManaExhaust(manaExhaustDuration));
        }
    }

    public void DepletingMana()
    {
        if (curMana > 0 && !isOOM)
        {
            isOOM = false;
            curMana--;
        }
    }

    protected IEnumerator ManaExhaust(float duration)
    {
        isOOM = true;
        actions.OutOfMana(duration);
        yield return new WaitForSeconds(duration);
        isOOM = false;
        curMana = maxMana;
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
