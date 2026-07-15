using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class NightBorneAttack : MonoBehaviour
{
    private NightBorneAnimator am;
    private NightBorneManager manager;

    [Header("Combat Settings")]
    public int damage = 10;
    public float knockbackForce = 2f;
    public float knockbackDuration = 0.2f;

    public float dashSpeed = 2f;
    public float dashMaxDistance = 2f;

    private Rigidbody2D rb;

    private Coroutine AttackCoroutineInstance;

    private void Start()
    {
        am = GetComponent<NightBorneAnimator>();
        manager = GetComponent<NightBorneManager>();
        rb = GetComponent<Rigidbody2D>();
    }

    public Coroutine Attack()
    {
        if (AttackCoroutineInstance != null)
        {
            StopCoroutine(AttackCoroutineInstance);
        }
        AttackCoroutineInstance = StartCoroutine(AttackCoroutine());
        return AttackCoroutineInstance;
    }

    public IEnumerator AttackCoroutine()
    {
        if (manager == null || manager.Player == null || rb == null || manager.Player.transform == null) yield break;

        am.PlayAnimation(NightBorneAnimator.NIGHT_BORNE_ATTACK);
        am.canChangeDirection = false;

        // wait for the moment in the animation where the enemy swings
        yield return new WaitForSeconds(0.35f);


        Collider2D enemyBodyCollider = GetComponent<Collider2D>();

        Collider2D playerBodyCollider = manager.Player.GetComponent<Collider2D>();

        if (enemyBodyCollider != null && playerBodyCollider != null)
        {
            Physics2D.IgnoreCollision(enemyBodyCollider, playerBodyCollider, true);
        }


        Vector3 startingPosition = transform.position;
        Vector3 playerPosition = manager.Player.transform.position;
        
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
        yield return new WaitForSeconds(1f);
        //WaitUntil(() => am.IsAnimationFinished(NightBorneAnimator.NIGHT_BORNE_ATTACK));

        if (enemyBodyCollider != null && playerBodyCollider != null)
        {
            Physics2D.IgnoreCollision(enemyBodyCollider, playerBodyCollider, false);
        }

        rb.linearVelocity = Vector2.zero;
        am.PlayAnimation(NightBorneAnimator.NIGHT_BORNE_IDLE);
        AttackCoroutineInstance = null;
        am.canChangeDirection = true;
        yield return new WaitForSeconds(1);
    }
}
