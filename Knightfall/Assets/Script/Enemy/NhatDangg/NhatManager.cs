using System;
using UnityEngine;

public class NhatManager : EntityManager
{
    [SerializeField] private bool touchDamageEnabled = true;
    public int touchDamage = 5;
    public int touchKnockbackForce = 5;
    public float touchKnockbackDuration = 1.5f;

    private NhatAttack attackScript;
    private EnemyDialogueTrigger dialogueTrigger;
    private Health healthScript;

    private int lastHealth = -1;
    private bool isDead = false;

    public override void Start()
    {
        base.Start();
        attackScript = GetComponent<NhatAttack>();
        dialogueTrigger = GetComponent<EnemyDialogueTrigger>();
        healthScript = GetComponent<Health>();
        entityHealth = healthScript;
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        entityMovement = GetComponent<NhatMovement>();
        isKnockingBack = false;
        takeDamageCooldown = 0.2f;
        flashDuration = 2.5f;
        EntityName = "NhatDang01";
        defaultColor = new Color(1, 1, 1, 1);

        lastHealth = healthScript.health;
    }

    private void Update()
    {
        if (healthScript == null || isDead) return;

        if (healthScript.health < lastHealth)
        {
            dialogueTrigger?.TriggerHitDialogue();
            lastHealth = healthScript.health;
        }

        if (healthScript.health <= 0 && !isDead)
        {
            isDead = true;
            dialogueTrigger?.TriggerDeathDialogue();
        }
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
