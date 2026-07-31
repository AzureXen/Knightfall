using Assets.Script.Enemy.FlyingEye;
using System;
using System.Collections;
using Unity.AppUI.Core;
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
    [SerializeField] private int knockbackForce = 2;

    private bool hasHitPlayer = false;

    private void Start()
    {
        manager = GetComponent<FlyingEyeManager>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<FlyingEyeAnimator>();
        player = GameManagerScript.Instance.player;
        if (player == null) GameManagerScript.OnPlayerReady += RegisterPlayer;

        if (manager != null) manager.OnTouchDamage += HandleTouchDamage;
    }

    private void HandleTouchDamage()
    {
        if (chaseCoroutine != null)
        {
            hasHitPlayer = true;
        }
    }

    private void OnDestroy()
    {
        if (manager != null)
        {
            manager.OnTouchDamage -= HandleTouchDamage;
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
        hasHitPlayer = false;
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
        int oldKnockBack = manager.touchKnockbackForce;
        float oldKnockBackDuration = manager.touchKnockbackDuration;
        try
        {
            manager.touchDamage = damage;
            manager.touchKnockbackForce = knockbackForce;
            manager.touchKnockbackDuration = knockbackDuration;

            manager.touchDamageEnabled = true;

            if (player == null) yield break;
            float timer = 0f;
            rb.constraints = RigidbodyConstraints2D.None;
            animator.ChangeDirectionNone();
            transform.localScale = Vector3.one;

            while (timer < chaseDuration && !hasHitPlayer)
            {
                if (player == null) yield break;

                Vector2 direction = ((Vector2)player.transform.position - rb.position).normalized;
                Vector2 nextPosition = rb.position + direction * chaseSpeed * Time.fixedDeltaTime;
                rb.MovePosition(nextPosition);

                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                
                transform.rotation = Quaternion.Euler(0, 0, angle);

                timer += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
        }
        finally
        {
            animator.ChangeDirectionFaceMovement();
            transform.rotation = Quaternion.Euler(0, 0, 0);
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.linearVelocity = Vector2.zero;
            chaseCoroutine = null;
            manager.touchDamageEnabled = false;
            manager.touchDamage = oldTouchDamage;
            manager.touchKnockbackForce = oldKnockBack;
            manager.touchKnockbackDuration = oldKnockBackDuration;
        }
    }

}
