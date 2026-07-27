using Assets.Script.Enemy.FlyingEye;
using System.Collections;
using UnityEngine;
using static RoninAction;

public class FlyingEyeBite : MonoBehaviour
{
    private Rigidbody2D rb;
    private FlyingEyeManager manager;
    private FlyingEyeAction eyeAction;
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
        manager = GetComponent<FlyingEyeManager>();
        eyeAction = GetComponent<FlyingEyeAction>();
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
            // i set isAttacking in FlyingEyeAction immediately after calling Attack() function now
            //isAttacking = true;
            animator.PlayAnimation(FlyingEyeAnimator.ATTACK_1);
            yield return new WaitUntil(() => animator.IsAnimationFinished(FlyingEyeAnimator.ATTACK_1));
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
