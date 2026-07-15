using System;
using System.Collections;
using UnityEngine;
using static RoninAction;

public class RoninBasicAttack : MonoBehaviour
{
    Vector3 trueCenter;
    [SerializeField] private int damage = 5;
    [SerializeField] private float knockbackForce = 3f;
    [SerializeField] private float knockbackDuration = 0.5f;

    [SerializeField] private float dashMaxDistance = 5f;
    [SerializeField] private float dashSpeed = 3f;

    private Coroutine dashCoroutine;
    
    private Rigidbody2D rb;

    private Coroutine attackCoroutine;
    private RoninAnimator animator;
    private RoninAction roninAction;
    private GameObject player;
    public Boolean isAttacking = false;

    [SerializeField] private GameObject attackHitbox1;
    [SerializeField] private GameObject attackHitbox2;
    private AttackZoneScript attackZone1;
    private AttackZoneScript attackZone2;

    private RoninManager manager;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        manager = GetComponent<RoninManager>();
        trueCenter = GetComponent<SpriteRenderer>().bounds.center;
        roninAction = GetComponent<RoninAction>();
        animator = GetComponent<RoninAnimator>();
        player = GameManagerScript.Instance.player;
        if (player == null) GameManagerScript.OnPlayerReady += RegisterPlayer;
        attackZone1 = attackHitbox1.GetComponent<AttackZoneScript>();
        attackZone2 = attackHitbox2.GetComponent<AttackZoneScript>();

        attackZone1.damage = damage;
        attackZone1.knockbackDuration = knockbackDuration;
        attackZone1.knockbackForce = knockbackForce;
        attackZone1.damageSourcePosition = trueCenter;

        attackZone2.damage = damage;
        attackZone2.knockbackDuration = knockbackDuration;
        attackZone2.knockbackForce = knockbackForce;
        attackZone2.damageSourcePosition = trueCenter;
    }

    private void RegisterPlayer(GameObject player)
    {
        this.player = player;
    }
    public Coroutine Attack()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }
        return attackCoroutine = StartCoroutine(AttackCoroutine());
    }
    public void StopAttackImmediate()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }
        attackZone1.DisableDamage();
        attackZone2.DisableDamage();
    }


    private IEnumerator AttackCoroutine()
    {
        try
        {
            isAttacking = true;
            roninAction.ChangeAnimationState("RoninAttackCombo");
            yield return new WaitUntil(() => animator.IsAnimationFinished("RoninAttackCombo"));
            isAttacking = false;
        }
        finally
        {
            attackZone1.DisableDamage();
            attackZone2.DisableDamage();
            roninAction.ChangeAnimationState("RoninIdle");
            isAttacking = false;
        }
    }

    public void EnableHitbox1()
    {
        attackZone1.EnableDamage();
    }
    public void EnableHitbox2()
    {
        attackZone2.EnableDamage();
    }
    public void DisableHitbox1()
    {
        attackZone1.DisableDamage();
    }
    public void DisableHitbox2()
    {
        attackZone2.DisableDamage();
    }
    
    public void DashTowardsPlayer()
    {
        if(dashCoroutine != null)
        {
            StopCoroutine(dashCoroutine);
        }
        dashCoroutine = StartCoroutine(DashTowardsPlayerCoroutine());
    }

    private IEnumerator DashTowardsPlayerCoroutine()
    {
        Collider2D enemyBodyCollider = GetComponent<Collider2D>();

        Collider2D playerBodyCollider = player.GetComponent<Collider2D>();

        if (enemyBodyCollider != null && playerBodyCollider != null)
        {
            Physics2D.IgnoreCollision(enemyBodyCollider, playerBodyCollider, true);
        }


        Vector3 startingPosition = transform.position;
        Vector3 playerPosition = player.transform.position;

        Vector3 directionToPlayer = (playerPosition - startingPosition);
        float distanceToPlayer = directionToPlayer.magnitude;
        directionToPlayer.Normalize();
        Vector3 hitboxEnd;
        if (distanceToPlayer <= dashMaxDistance)
        {
            hitboxEnd = playerPosition;
        }
        else
        {
            hitboxEnd = startingPosition + (directionToPlayer * dashMaxDistance);
        }

        while (Vector3.Distance(transform.position, hitboxEnd) > 0.01f)
        {
            Vector2 newPos = Vector2.MoveTowards(
                rb.position,
                hitboxEnd,
                dashSpeed * Time.fixedDeltaTime
            );
            rb.MovePosition(newPos);
            yield return new WaitForFixedUpdate();
        }

        if (enemyBodyCollider != null && playerBodyCollider != null)
        {
            Physics2D.IgnoreCollision(enemyBodyCollider, playerBodyCollider, false);
        }

        rb.linearVelocity = Vector2.zero;
        dashCoroutine = null;
    }
}
