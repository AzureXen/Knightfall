using Assets.Script.Enemy.FlyingEye;
using System.Collections;
using UnityEngine;
using static RoninAction;

public class FlyingEyeBite : MonoBehaviour
{
    private Rigidbody2D rb;
    private FlyingEyeAnimator animator;
    private GameObject player;
    [SerializeField] private GameObject biteAttackHitbox;
    private BiteAttackZone biteAttackZone;

    private Coroutine attackCoroutine;
    private Coroutine dashCoroutine;

    [SerializeField] private float dashMaxDistance = 2f;
    [SerializeField] private float dashSpeed = 1f;

    public bool isAttacking = false;

    [SerializeField] private int damage = 5;
    [SerializeField] private float knockbackDuration = 0.3f;
    [SerializeField] private float knockbackForce = 2f;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<FlyingEyeAnimator>();
        player = GameManagerScript.Instance.player;
        if (player == null) GameManagerScript.OnPlayerReady += RegisterPlayer;
        biteAttackZone = biteAttackHitbox.GetComponent<BiteAttackZone>();

        biteAttackZone.damage = damage;
        biteAttackZone.knockbackDuration = knockbackDuration;
        biteAttackZone.knockbackForce = knockbackForce;
        biteAttackZone.damageSourcePosition = transform.position;

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
        biteAttackZone.DisableDamage();
    }


    private IEnumerator AttackCoroutine()
    {
        try
        {
            animator.PlayAnimation(FlyingEyeAnimator.ATTACK_4);
            yield return new WaitUntil(() => animator.IsAnimationFinished(FlyingEyeAnimator.ATTACK_4));
            yield return new WaitForSeconds(0.1f);
            isAttacking = false;
        }
        finally
        {
            biteAttackZone.DisableDamage();
            animator.PlayAnimation(FlyingEyeAnimator.IDLE);
            isAttacking = false;
        }
    }

    public void EnableBiteHitbox()
    {
        biteAttackZone.EnableDamage();
    }
    public void DisableBiteHitbox()
    {
        biteAttackZone.DisableDamage();
    }

    public void DashTowardsPlayer()
    {
        if (dashCoroutine != null)
        {
            StopCoroutine(dashCoroutine);
        }
        dashCoroutine = StartCoroutine(DashTowardsPlayerCoroutine());
    }

    private IEnumerator DashTowardsPlayerCoroutine()
    {
        Collider2D enemyBodyCollider = GetComponent<Collider2D>();
        Collider2D playerBodyCollider = player.GetComponent<Collider2D>();

        Vector3 startingPosition = transform.position;
        Vector3 playerPosition = player.transform.position;
        Vector3 directionToPlayer = (playerPosition - startingPosition).normalized;
        float distanceToPlayer = Vector3.Distance(startingPosition, playerPosition);

        Vector3 targetDestination = distanceToPlayer <= dashMaxDistance
            ? playerPosition
            : startingPosition + (directionToPlayer * dashMaxDistance);

        float totalDashDistance = Vector3.Distance(startingPosition, targetDestination);

        float enemyRadius = enemyBodyCollider != null ? enemyBodyCollider.bounds.extents.x : 0.5f;
        RaycastHit2D wallHit = Physics2D.CircleCast(
            startingPosition,
            enemyRadius,
            directionToPlayer,
            totalDashDistance,
            LayerMask.GetMask("Wall")
        );

        if (wallHit.collider != null)
        {
            float safeDistance = Mathf.Max(0f, wallHit.distance - enemyRadius + 0.5f);
            targetDestination = startingPosition + (directionToPlayer * safeDistance);
        }
        while (Vector3.Distance(rb.position, targetDestination) > 0.1f)
        {
            Vector2 newPos = Vector2.MoveTowards(
                rb.position,
                targetDestination,
                dashSpeed * Time.fixedDeltaTime
            );
            rb.MovePosition(newPos);
            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(targetDestination);

        rb.linearVelocity = Vector2.zero;
        dashCoroutine = null;
    }
}
