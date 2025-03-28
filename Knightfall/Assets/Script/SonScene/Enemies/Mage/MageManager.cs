using UnityEngine;
using System.Collections;

public class MageManager : EntityManager
{
    [SerializeField] public float rangeActive = 10f;
    [SerializeField] public bool isActive = false;

    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private GameObject orcPrefab;

    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform spawnPoint;

    [SerializeField] private float attackCooldown = 15f;
    [SerializeField] private float summonCooldown = 10f;

    private float nextAttackTime = 0;
    private float nextSummonTime = 0;

    private MageMovement movement;
    private Animator animator;
    private GameObject currentOrc;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        entityHealth = GetComponent<EnemyHealth>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        isKnockingBack = false;
        takeDamageCooldown = 0.2f;
        flashDuration = 2.5f;
        EntityName = "Orc";
        defaultColor = new Color(1, 1, 1, 1);
        movement = GetComponent<MageMovement>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (movement.player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, movement.player.transform.position);

        if (distanceToPlayer < rangeActive) isActive = true;
        if (!isActive) return;

        if (Time.time >= nextAttackTime)
        {
            StartCoroutine(ShootFireball());
            nextAttackTime = Time.time + attackCooldown;
        }

        if (Time.time >= nextSummonTime && currentOrc == null)
        {
            SummonOrc();
            nextSummonTime = Time.time + summonCooldown;
        }
    }

    private IEnumerator ShootFireball()
    {
        animator.SetBool("isAttacking", true);
        animator.SetTrigger("Shoot"); // Play attack animation

        // Wait for animation to finish
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        animator.SetBool("isAttacking", false);
    }

    public void FireFireball()
    {
        if (movement.player != null)
        {
            GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
            fireball.transform.localScale = new Vector3(18f, 18f, 1f);
            fireball.GetComponent<MageFireBall>().Initialize(movement.player.transform.position);
        }
    }

    private void SummonOrc()
    {
        if (orcPrefab != null && movement.player != null)
        {
            currentOrc = Instantiate(orcPrefab, spawnPoint.position, Quaternion.identity);

            // Set player target
            SOrcMovement orcMovement = currentOrc.GetComponent<SOrcMovement>();
            if (orcMovement != null)
            {
                orcMovement.player = movement.player;
            }

            // Track orc health to respawn when it dies
            EnemyHealth orcHealth = currentOrc.GetComponent<EnemyHealth>();
            if (orcHealth != null)
            {
                orcHealth.OnDeath += HandleOrcDeath;
            }
        }
    }

    private void HandleOrcDeath()
    {
        currentOrc = null;
        nextSummonTime = Time.time + summonCooldown;
    }
}
