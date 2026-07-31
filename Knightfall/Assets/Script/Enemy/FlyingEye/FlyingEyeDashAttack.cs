using Assets.Script.Enemy.FlyingEye;
using System.Collections;
using UnityEngine;

public class FlyingEyeDashAttack : MonoBehaviour
{
    private Rigidbody2D rb;
    private FlyingEyeAnimator animator;
    private GameObject player;

    private Coroutine attackCoroutine;
    private Coroutine dashCoroutine;


    [SerializeField] private float dashSpeed = 1f;
    [SerializeField] private float dashMaxDistance = 4f;

    [SerializeField] private int damage = 5;
    [SerializeField] private float knockbackDuration = 0.3f;
    [SerializeField] private int knockbackForce = 2;

    [SerializeField] private GameObject DashAttackZoneGameObject;
    private DashAttackZone attackZoneScript;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<FlyingEyeAnimator>();
        player = GameManagerScript.Instance.player;
        if (player == null) GameManagerScript.OnPlayerReady += RegisterPlayer;

        if (DashAttackZoneGameObject == null) Debug.LogWarning("FlyingEyeDashAttack: DashAttackZone is not initiated");
        else
        {
            attackZoneScript = DashAttackZoneGameObject.GetComponent<DashAttackZone>();
            attackZoneScript.damage = damage;
            attackZoneScript.knockbackDuration = knockbackDuration;
            attackZoneScript.knockbackForce = knockbackForce;
            attackZoneScript.damageSourcePosition = transform.position;
        }
            
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
    }


    private IEnumerator AttackCoroutine()
    {
        try
        {
            animator.PlayAnimation(FlyingEyeAnimator.ATTACK_4);
            yield return new WaitUntil(() => animator.IsAnimationFinished(FlyingEyeAnimator.ATTACK_4));
            yield return new WaitForSeconds(0.1f);
        }
        finally
        {
            attackZoneScript.DisableDamage();
            animator.PlayAnimation(FlyingEyeAnimator.IDLE);
        }
    }

    public void EnableDashAttackHitbox()
    {
        attackZoneScript.EnableDamage();
    }
    public void DisableDashAttackHitbox()
    {
        attackZoneScript.DisableDamage();
    }

     public void DashToPlayer()
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
        Collider2D playerBodyCollider = player != null ? player.GetComponent<Collider2D>() : null;

        if (player == null) yield break;

        Vector3 startingPosition = transform.position;

        Vector3 directionToPlayer = transform.DirectionTo(player);

        Vector3 fullDestination = startingPosition + (directionToPlayer * dashMaxDistance);
        Vector3 targetDestination = fullDestination;

        float enemyRadius = enemyBodyCollider != null ? enemyBodyCollider.bounds.extents.x : 0.5f;

        RaycastHit2D wallHit = Physics2D.CircleCast(
            startingPosition,
            enemyRadius,
            directionToPlayer,
            dashMaxDistance,
            LayerMask.GetMask("Wall")
        );

        if (wallHit.collider != null)
        {
            float safeDistance = Mathf.Max(0f, wallHit.distance - 0.05f);
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
