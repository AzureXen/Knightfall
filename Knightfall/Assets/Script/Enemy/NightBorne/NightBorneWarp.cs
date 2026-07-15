using System.Collections;
using UnityEngine;

public class NightBorneWarp : MonoBehaviour
{
    private NightBorneAnimator am;
    private NightBorneManager manager;
    private Rigidbody2D rb;

    [Header("Teleport Settings")]
    [SerializeField] private float offsetDistance = 2f; // How far behind the player to warp
    [SerializeField] private LayerMask collisionLayer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        am = GetComponent<NightBorneAnimator>();
        manager = GetComponent<NightBorneManager>();
    }
    public Coroutine Execute()
    {
        return StartCoroutine(Warp());
    }
    private IEnumerator Warp()
    {
        if (manager == null || manager.Player == null || rb == null || manager.Player.transform == null) yield break;

        Collider2D enemyBodyCollider = GetComponent<Collider2D>();
        Collider2D playerBodyCollider = manager.Player.GetComponent<Collider2D>();

        if (enemyBodyCollider != null && playerBodyCollider != null)
        {
            Physics2D.IgnoreCollision(enemyBodyCollider, playerBodyCollider, true);
        }

        manager.canDamage = false;
        // For teleport shaking effects
        rb.bodyType = RigidbodyType2D.Kinematic;
        am.PlayAnimation(NightBorneAnimator.NIGHT_BORNE_TELEPORT);
        yield return new WaitForSeconds(2f);
        

        Vector3 playerPos = manager.Player.transform.position;

        Vector3 targetPosition = playerPos + (Vector3.left * offsetDistance);

        Collider2D hitWall = Physics2D.OverlapCircle(targetPosition, 0.2f, collisionLayer);

        if (hitWall != null)
        {
            targetPosition = playerPos + (Vector3.right * offsetDistance);

            Collider2D hitRightWall = Physics2D.OverlapCircle(targetPosition, 0.2f, collisionLayer);
            if (hitRightWall != null)
            {
                targetPosition = playerPos;
            }
        }
        transform.position = targetPosition;

        am.PlayAnimation(NightBorneAnimator.NIGHT_BORNE_APPEAR_QUICK);

        yield return new WaitUntil(() => am.IsAnimationFinished(NightBorneAnimator.NIGHT_BORNE_APPEAR_QUICK));

        manager.canDamage = true;

        if (enemyBodyCollider != null && playerBodyCollider != null)
        {
            Physics2D.IgnoreCollision(enemyBodyCollider, playerBodyCollider, false);
        }

        am.PlayAnimation(NightBorneAnimator.NIGHT_BORNE_IDLE);
        rb.bodyType = RigidbodyType2D.Dynamic;
    }
}
