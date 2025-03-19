using System.Collections;
using UnityEngine;

public class NecromancerAttack : MonoBehaviour
{
    private GameObject player;
    public GameObject attackHitbox;
    public GameObject skeleton;
    private Animator animator;
    private UndeadHealth undeadHealth;

    NecromancerManager manager;

    public float attackDuration = 1f;
    public int Damage;
    public float knockbackForce;
    public float knockbackDuration;

    public Transform attackTransform;

    public float cooldownTime = 2f;
    public float cooldownTimer = 0f;

    public bool IsOnCooldown = false;
    public bool NecroIsAttacking = false;
    public bool NecroIsSummoning = false;

    private GameObject attackInstance;
    private GameObject summonInstance;

    private Coroutine attackCoroutine;
    private Coroutine summonCoroutine;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        undeadHealth = GetComponent<UndeadHealth>();
        manager = GetComponent<NecromancerManager>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (undeadHealth.inflictPain && attackInstance!=null) { Destroy(attackInstance); }
        if (IsOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                IsOnCooldown = false; // Reset cooldown
                cooldownTimer = 0f;
            }
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
            NecroIsAttacking = false;
            attackCoroutine = null;
            StopCoroutine(attackCoroutine);
        }
        StartCooldown();
        manager.DepletingMana();
        attackCoroutine = StartCoroutine(AttackCoroutine());
    }
    public IEnumerator AttackCoroutine()
    {
        if (attackInstance != null)
        {
            Destroy(attackInstance);
        }

        animator.SetTrigger("Attacking");

        attackInstance = Instantiate(attackHitbox, transform.position, Quaternion.identity, attackTransform);
        attackInstance.transform.localScale = new Vector3(1f, 1f, 1);
        attackInstance.transform.position += new Vector3(0f, 0f, 0f);


        Rigidbody2D projectile = attackInstance.GetComponent<Rigidbody2D>();
        projectile.linearVelocity = Vector3.zero;

        RangedDamageHitboxScript rangedDamageHitboxScript = attackInstance.GetComponent<RangedDamageHitboxScript>();
        rangedDamageHitboxScript.damage = Damage;
        rangedDamageHitboxScript.knockbackForce = knockbackForce;
        rangedDamageHitboxScript.knockbackDuration = knockbackDuration;

        attackInstance.transform.parent = null;
        
        yield return new WaitForSeconds(3.2f);
        rangedDamageHitboxScript.EnableDamage();
        Vector3 playerPos = player.transform.position;
        Vector3 attackDir = (attackTransform.position - playerPos).normalized;
        float rotateZ = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;
        attackInstance.transform.RotateAround(attackTransform.position, Vector3.forward, rotateZ);

        projectile.linearVelocity = (playerPos - attackTransform.position) * 1f;
        yield return new WaitForSeconds(1f);
        rangedDamageHitboxScript.DisableDamage();
        if (attackInstance)
        {
            Destroy(attackInstance);
        }
        yield return new WaitForSeconds(attackDuration - 4.5f);
        if (manager.curMana <= 0) { manager.DepletedMana(); }
        NecroIsAttacking = false;
    }

    public void Attack2()
    {
        if (summonCoroutine != null)
        {
            NecroIsSummoning = false;
            StopCoroutine(summonCoroutine);
        }
        StartCooldown();
        manager.DepletingMana();
        summonCoroutine = StartCoroutine(SummonCoroutine());
    }

    public IEnumerator SummonCoroutine()
    {
        animator.SetTrigger("Attacking2");
        yield return new WaitForSeconds(0.8f);

        summonInstance = Instantiate(skeleton, transform.position, Quaternion.identity, attackTransform);
        summonInstance.transform.position += new Vector3(0, 0.5f, 0);

        summonInstance.transform.parent = null;

        yield return new WaitForSeconds(attackDuration - 2.7f - 0.8f);
        if (manager.curMana <= 0) { manager.DepletedMana(); }
        NecroIsSummoning = false;
    }
}
