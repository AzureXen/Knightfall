using System;
using UnityEngine;
using UnityEngine.UI;

public class EvilWizardManager : EntityManager
{
    [SerializeField] private Boolean touchDamageEnabled = true;
    public int touchDamage = 10;
    public int touchKnockbackForce = 10;
    public float touchKnockbackDuration = 1.5f;
    private EvilWizardMovement movement;
    private Animator animator;

    public GameObject fireballPrefab;
    public GameObject magicBlastPrefab;
    public Transform fireballSpawnPoint;


    public float attackCooldown = 1f;
    private float attackTimer = 0f;

    private bool isAttacking = false;
    public float attackRange = 10f;

    public Image healthImage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        entityHealth = GetComponent<Health>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<EvilWizardMovement>();
        isKnockingBack = false;
        takeDamageCooldown = 0.2f;
        flashDuration = 2.5f;
        EntityName = "EvilWizard";
        defaultColor = new Color(1, 1, 1, 1);
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        attackTimer -= Time.deltaTime;

        float distanceToPlayer = Vector2.Distance(transform.position, movement.player.transform.position);

        if (!isAttacking && attackTimer <= 0 && distanceToPlayer <= attackRange)
        {
            StartRandomAttack();
        }

        HealthChange();
    }

    private void StartRandomAttack()
    {
        isAttacking = true;
        movement.DisableMovement();
        animator.SetBool("isAttacking", true);

        int attackType = UnityEngine.Random.Range(0, 3); 

        switch (attackType)
        {
            case 0:
                animator.SetTrigger("Attack1");
                Invoke(nameof(ShootFireball), 0.5f);
                break;
            case 1:
                animator.SetTrigger("Attack2");
                Invoke(nameof(ShootTripleFireball), 0.7f);
                break;
            case 2:
                animator.SetTrigger("Attack3");
                Invoke(nameof(CastMagicBlast), 1f);
                break;
        }
    }

    private void ShootFireball()
    {
        if (fireballPrefab != null && fireballSpawnPoint != null)
        {
            GameObject fireball = Instantiate(fireballPrefab, fireballSpawnPoint.position, Quaternion.identity);
            FireBall fireBall = fireball.GetComponent<FireBall>();
            if (fireBall != null)
            {
                Vector2 direction = (movement.player.transform.position - transform.position).normalized;
                fireBall.SetDirection(direction);
            }
        }

        EndAttack();
    }

    private void ShootTripleFireball()
    {
        if (fireballPrefab != null && fireballSpawnPoint != null)
        {
            Vector2 direction = (movement.player.transform.position - transform.position).normalized;

            SpawnFireball(direction);
            SpawnFireball(Quaternion.Euler(0, 0, 20) * direction);
            SpawnFireball(Quaternion.Euler(0, 0, -20) * direction);
        }
        EndAttack();
    }

    private void SpawnFireball(Vector2 direction)
    {
        GameObject fireball = Instantiate(fireballPrefab, fireballSpawnPoint.position, Quaternion.identity);
        fireball.transform.right = direction;
        FireBall fireBall = fireball.GetComponent<FireBall>();
        if (fireBall != null)
        {
            fireBall.SetDirection(direction);
        }
    }

    private void CastMagicBlast()
    {
        if (magicBlastPrefab != null && fireballSpawnPoint != null)
        {
            Instantiate(magicBlastPrefab, movement.player.transform.position, Quaternion.identity);
        }
        EndAttack();
    }

    private void EndAttack()
    {
        isAttacking = false;
        movement.EnableMovement();
        animator.SetBool("isAttacking", false);
        attackTimer = attackCooldown; 
    }

    public void HealthChange()
    {
        healthImage.fillAmount = entityHealth.health / 100f;
    }
}
