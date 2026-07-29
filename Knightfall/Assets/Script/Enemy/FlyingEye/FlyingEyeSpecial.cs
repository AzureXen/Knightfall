using Assets.Script.Enemy.FlyingEye;
using System.Collections;
using UnityEngine;

public class FlyingEyeSpecial : MonoBehaviour
{
    private Rigidbody2D rb;
    private FlyingEyeAnimator animator;
    private GameObject player;
    private FlyingEyeManager manager;

    private Coroutine attackCoroutine;

    private Coroutine chaseCoroutine;

    [SerializeField] private float chaseSpeed = 1f;
    [SerializeField] private float chaseDuration = 4f;

    [SerializeField] private int damage = 5;
    [SerializeField] private float knockbackDuration = 0.3f;
    [SerializeField] private float knockbackForce = 2f;

    private void Start()
    {
        manager = GetComponent<FlyingEyeManager>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<FlyingEyeAnimator>();
        player = GameManagerScript.Instance.player;
        if (player == null) GameManagerScript.OnPlayerReady += RegisterPlayer;

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
            animator.PlayAnimation(FlyingEyeAnimator.ATTACK_2_START);
            yield return new WaitUntil(() => animator.IsAnimationFinished(FlyingEyeAnimator.ATTACK_2_START));
            
            chaseCoroutine = StartCoroutine(ChaseCoroutine());
            animator.PlayAnimation(FlyingEyeAnimator.ATTACK_2_LOOP);
            
            if(chaseCoroutine != null) yield return chaseCoroutine;

            animator.PlayAnimation(FlyingEyeAnimator.ATTACK_2_END);
            yield return new WaitUntil(() => animator.IsAnimationFinished(FlyingEyeAnimator.ATTACK_2_END));
        }
        finally
        {
            animator.PlayAnimation(FlyingEyeAnimator.IDLE);
        }
    }

    private IEnumerator ChaseCoroutine()
    {
        int oldTouchDamage = manager.touchDamage;
        try
        {
            manager.touchDamage = damage;
            manager.touchDamageEnabled = true;

            if (player == null) yield break;
            float timer = 0f;
            while (timer < chaseDuration)
            {
                if (player == null) yield break;

                Vector2 direction = ((Vector2)player.transform.position - rb.position).normalized;
                Vector2 nextPosition = rb.position + direction * chaseSpeed * Time.fixedDeltaTime;
                rb.MovePosition(nextPosition);

                timer += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            rb.linearVelocity = Vector2.zero;
            chaseCoroutine = null;
        }
        finally
        {
            manager.touchDamageEnabled = false;
            manager.touchDamage = oldTouchDamage;
        }
    }

}
