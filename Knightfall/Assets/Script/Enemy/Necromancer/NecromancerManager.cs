using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class NecromancerManager : EntityManager
{
    [SerializeField] private int maxMana;
    public int curMana;
    [SerializeField] private float manaExhaustDuration;

    private Boolean isOOM = false;

    private NecromancerActions actions;

    public override void Start()
    {
        entityHealth = GetComponent<UndeadHealth>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        entityMovement = GetComponent<NecromancerMovement>();
        isKnockingBack = false;
        takeDamageCooldown = 0.4f;
        EntityName = "Necromancer";
        defaultColor = new Color(1, 1, 1, 1);

        actions = GetComponent<NecromancerActions>();
    }

    void Update()
    {

    }

    public void DepletingMana()
    {
        if (curMana > 0 && !isOOM)
        {
            curMana -= 1;
        }
    }

    public void DepletedMana()
    {
        StartCoroutine(ManaExhaust(manaExhaustDuration));
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
