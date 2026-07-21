using System;
using System.Collections;
using UnityEngine;
using static RoninAction;

public class RoninBasicAttack : MonoBehaviour
{
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
        roninAction = GetComponent<RoninAction>();
        animator = GetComponent<RoninAnimator>();
        player = GameManagerScript.Instance.player;
        if (player == null) GameManagerScript.OnPlayerReady += RegisterPlayer;
        attackZone1 = attackHitbox1.GetComponent<AttackZoneScript>();
        attackZone2 = attackHitbox2.GetComponent<AttackZoneScript>();

        attackZone1.damage = damage;
        attackZone1.knockbackDuration = knockbackDuration;
        attackZone1.knockbackForce = knockbackForce;
        attackZone1.damageSourcePosition = transform.position;

        attackZone2.damage = damage;
        attackZone2.knockbackDuration = knockbackDuration;
        attackZone2.knockbackForce = knockbackForce;
        attackZone2.damageSourcePosition = transform.position;
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

        Vector3 targetDestination;
        if (distanceToPlayer <= dashMaxDistance)
        {
            targetDestination = playerPosition;
        }
        else
        {
            targetDestination = startingPosition + (directionToPlayer * dashMaxDistance);
        }

        float totalDashDistance = Vector3.Distance(startingPosition, targetDestination);
        RaycastHit2D wallHit = Physics2D.Raycast(startingPosition, directionToPlayer, totalDashDistance, LayerMask.GetMask("Wall"));

        // --- VISUAL DEBUGGING START ---
        if (wallHit.collider != null && wallHit.collider.CompareTag("Wall"))
        {
            // Draw a RED line from the starting position to exactly where it hit the wall
            Debug.DrawLine(startingPosition, wallHit.point, Color.red, 2.0f);

            // Draw a green dot/crosshair at the exact collision point
            Debug.DrawRay(wallHit.point, Vector3.up * 0.2f, Color.green, 2.0f);
        }
        else
        {
            // Draw a GREEN line showing the full clear path of the dash if nothing was hit
            Debug.DrawLine(startingPosition, targetDestination, Color.green, 2.0f);
        }
        // --- VISUAL DEBUGGING END ---

        if (wallHit.collider != null && wallHit.collider.CompareTag("Wall"))
        {
            float safeDistance = Mathf.Max(0f, wallHit.distance - 5f);
            targetDestination = startingPosition + (directionToPlayer * safeDistance);
        }

        while (Vector3.Distance(transform.position, targetDestination) > 0.15f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetDestination,
                dashSpeed * Time.deltaTime 
            );
            yield return null;
        }

        if (enemyBodyCollider != null && playerBodyCollider != null)
        {
            Physics2D.IgnoreCollision(enemyBodyCollider, playerBodyCollider, false);
        }

        rb.linearVelocity = Vector2.zero;
        dashCoroutine = null;
    }
}
