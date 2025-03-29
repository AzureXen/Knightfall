using System.Collections;
using UnityEngine;

public class SkeletonHeadAttack : MonoBehaviour
{
    private GameObject player;
    public GameObject attackHitbox;
    private Animator animator;
    private UndeadHealth undeadHealth;

    public float attackDuration = 1f;
    public int Damage;
    public float knockbackForce;
    public float knockbackDuration;

    public Transform attackTransform;

    public float cooldownTime = 2f;
    public float cooldownTimer = 0f;

    public bool IsOnCooldown = false;
    public bool IsAttacking = false;
    public bool Is2ndAttacking = false;

    private GameObject attackInstance;
    private Coroutine attackCoroutine;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        undeadHealth = GetComponent<UndeadHealth>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (undeadHealth.inflictPain) { Destroy(attackInstance); }

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
            IsAttacking = false;
            StopCoroutine(attackCoroutine);
        }
        StartCooldown();
        IsAttacking = true;
        attackCoroutine = StartCoroutine(AttackCoroutine());
    }
    public IEnumerator AttackCoroutine()
    {
        if (attackInstance != null)
        {
            Destroy(attackInstance);
        }
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.2f);

        attackInstance = Instantiate(attackHitbox, transform.position, Quaternion.identity, attackTransform);
        attackInstance.transform.localScale = new Vector3(8f, 4f, 1);
        attackInstance.transform.position += new Vector3(0.5f, 0f, 0f);

        Vector3 playerPos = player.transform.position;
        Vector3 attackDir = (attackTransform.position - playerPos).normalized;
        float rotateZ = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;
        attackInstance.transform.RotateAround(attackTransform.position, Vector3.forward, rotateZ+180);

        HeadbuttHitboxScript damageHitboxScript = attackInstance.GetComponent<HeadbuttHitboxScript>();
        damageHitboxScript.damage = Damage;
        damageHitboxScript.knockbackForce = knockbackForce;
        damageHitboxScript.knockbackDuration = knockbackDuration;

        attackInstance.transform.parent = null;
        
        damageHitboxScript.EnableDamage();
        yield return new WaitForSeconds(0.4f);
        damageHitboxScript.DisableDamage();
        if (attackInstance)
        {
            Destroy(attackInstance);
        }
        yield return new WaitForSeconds(attackDuration - 0.6f);
        IsAttacking = false;
    }
}
