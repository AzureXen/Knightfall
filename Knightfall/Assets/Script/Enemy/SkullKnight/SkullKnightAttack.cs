using System.Collections;
using UnityEngine;

public class SkullKnightAttack : MonoBehaviour
{
    private GameObject player;
    public GameObject hitboxHandler;  // Reference to the always-active HitboxHandler
    private Animator hitboxAnimator; // Animator on the HitboxHandler
    private UndeadBossHealth undeadHealth;

    public float attack1Duration = 2.4f; // First attack animation duration
    public float attack2Duration = 1.6f; // Second attack animation duration

    public float cooldownTime = 4f;
    private float cooldownTimer = 0f;

    public bool IsOnCooldown = false;
    public bool IsAttacking = false;
    public bool Is2ndAttacking = false;

    private Coroutine attackCoroutine;
    private Animator animator;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        undeadHealth = GetComponent<UndeadBossHealth>();
        animator = GetComponent<Animator>();
        hitboxAnimator = hitboxHandler.GetComponent<Animator>();
    }

    void Update()
    {
        if (IsOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                IsOnCooldown = false;
                cooldownTimer = 0f;
            }
        }
        if (undeadHealth.isDead)
        {
            IsAttacking = false;
            Is2ndAttacking= false;
        }
    }

    public void StartCooldown()
    {
        IsOnCooldown = true;
        cooldownTimer = cooldownTime;
    }

    public void Attack()
    {
        if (attackCoroutine != null)
        {
            IsAttacking = false;
            Is2ndAttacking = false;
            StopCoroutine(attackCoroutine);
        }
        StartCooldown();
        IsAttacking = true;
        attackCoroutine = StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        animator.SetTrigger("attack1");
        hitboxAnimator.SetTrigger("attack1"); // Trigger attack1 animation
        BossDamageHitboxScript damageHitboxScript = hitboxHandler.GetComponent<BossDamageHitboxScript>();
        damageHitboxScript.damage = 10;
        damageHitboxScript.knockbackForce = 5;
        damageHitboxScript.knockbackDuration = 2;
        yield return new WaitForSeconds(0.6f);
        VanSoundManager.PlaySound(SoundType.BOSSSLICE);
        yield return new WaitForSeconds(attack1Duration-0.6f);
        IsAttacking = false;
    }

    public void Attack2()
    {
        if (attackCoroutine != null)
        {
            IsAttacking = false;
            Is2ndAttacking = false;
            StopCoroutine(attackCoroutine);
        }
        StartCooldown();
        Is2ndAttacking = true;
        attackCoroutine = StartCoroutine(AttackComboCoroutine());
    }

    private IEnumerator AttackComboCoroutine()
    {
        animator.SetTrigger("attack2");
        hitboxAnimator.SetTrigger("attack2"); // Trigger attack2 animation
        BossDamageHitboxScript damageHitboxScript = hitboxHandler.GetComponent<BossDamageHitboxScript>();
        damageHitboxScript.damage = 20;
        damageHitboxScript.knockbackForce = 10;
        damageHitboxScript.knockbackDuration = 2;
        yield return new WaitForSeconds(0.4f);
        yield return new WaitForSeconds(attack2Duration);
        Is2ndAttacking = false;
    }
}