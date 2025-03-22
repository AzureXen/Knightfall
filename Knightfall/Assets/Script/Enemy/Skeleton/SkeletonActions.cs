using System;
using System.Collections;
using UnityEngine;
using static NecromancerActions;
using static UnityEngine.Rendering.DebugUI;

public class SkeletonActions : MonoBehaviour
{
    private Transform player;
    private SkeletonMovement skeletonMovement;
    private SkeletonAttack skeletonAttack;
    private UndeadHealth health;

    [SerializeField] private float detectionRange = 10f;  // When Skeleton starts chasing
    [SerializeField] private float approachRange = 3f;   // When Skeleton starts slowing down
    [SerializeField] private float attackRange = 1f;   // When Skeleton should stop moving to attack
    
    private Boolean isSpotted = false;
    public enum SkeletonState { 
        IDLE, 
        CHASE, 
        APPROACH, 
        ATTACK, ATTACK2,
        BACKOFF,
        DEAD,
    }
    public SkeletonState currentState;
    
    private Coroutine actionCoroutine;
    [SerializeField] private Boolean canDecide = true;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentState = SkeletonState.IDLE;
        skeletonMovement = GetComponent<SkeletonMovement>();
        skeletonAttack = GetComponent<SkeletonAttack>();
        health = GetComponent<UndeadHealth>();
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (canDecide && !health.isDead)
        {
            StartCoroutine(DecideNextAction(distance));
        }
        else if (health.isDead) {
            if (actionCoroutine != null)
            {
                StopCoroutine(actionCoroutine);
                currentState = SkeletonState.IDLE;
                skeletonMovement.StopMoving();
            }
            actionCoroutine = StartCoroutine(SkeletonDead());
        }
    }

    private IEnumerator DecideNextAction(float distance)
    {
        if (actionCoroutine != null)
        {
            StopCoroutine(actionCoroutine);
        }

        yield return new WaitForSeconds(0.1f); // Small delay to prevent spam

        if (distance > detectionRange && !isSpotted)
        {
            actionCoroutine = StartCoroutine(IdleState());
        }
        else if (distance > approachRange && (distance <= detectionRange || isSpotted) && skeletonAttack.cooldownTimer == 0)
        {
            actionCoroutine = StartCoroutine(ChaseState());
        }
        else if (distance > attackRange && distance <= approachRange && skeletonAttack.cooldownTimer == 0)
        {
            actionCoroutine = StartCoroutine(ApproachState());
        }
        else if (distance <= attackRange && skeletonAttack.cooldownTimer == 0)
        {
            actionCoroutine = StartCoroutine(AttackState());
        }
        else if (distance <= attackRange && skeletonAttack.cooldownTimer > 0 && !skeletonAttack.IsAttacking && !skeletonAttack.Is2ndAttacking)
        {
            actionCoroutine = StartCoroutine(BackOffState());
        }

    }

    private IEnumerator IdleState()
    {
        canDecide = false;
        currentState = SkeletonState.IDLE;
        skeletonMovement.StopMoving();
        yield return null;

        canDecide = true;
    }

    private IEnumerator ChaseState()
    {
        canDecide = false;
        isSpotted = true;
        currentState = SkeletonState.CHASE;
        skeletonMovement.ChasePlayer();
        yield return new WaitForSeconds(0.1f);
        canDecide = true;
    }

    private IEnumerator ApproachState()
    {
        canDecide = false;
        currentState = SkeletonState.APPROACH;
        skeletonMovement.ApproachPlayer();
        yield return new WaitForSeconds(0.1f);
        canDecide = true;
    }

    private IEnumerator AttackState()
    {
        canDecide = false;
        skeletonMovement.StopMoving();
        float diceRoll = UnityEngine.Random.value; // 50/50 chance
        if (diceRoll <= 0.5f && !skeletonAttack.IsOnCooldown)
        {
            currentState = SkeletonState.ATTACK;
            skeletonAttack.Attack();
        }
        else if (diceRoll > 0.5f && !skeletonAttack.IsOnCooldown)
        {
            currentState = SkeletonState.ATTACK2;
            skeletonAttack.Attack2();
        }
        yield return new WaitForSeconds(1f);
        canDecide = true;
    }

    private IEnumerator BackOffState()
    {
        canDecide = false;
        currentState = SkeletonState.BACKOFF;
        skeletonMovement.BackOffPlayer();
        yield return new WaitForSeconds(0.1f);
        canDecide = true;
    }

    private IEnumerator SkeletonDead()
    {
        canDecide = false;
        currentState = SkeletonState.DEAD;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);
        skeletonMovement.StopMoving();
        yield return null;
    }
}
