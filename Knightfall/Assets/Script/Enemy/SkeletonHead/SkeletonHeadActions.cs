using System;
using System.Collections;
using UnityEngine;
using static NecromancerActions;
using static UnityEngine.Rendering.DebugUI;

public class SkeletonHeadActions : MonoBehaviour
{
    private Transform player;
    private SkeletonHeadMovement skeletonMovement;
    private SkeletonHeadAttack skeletonAttack;
    private UndeadHealth health;

    [SerializeField] private float detectionRange = 10f;  // When Skeleton starts chasing
    [SerializeField] private float approachRange = 3f;   // When Skeleton starts slowing down
    [SerializeField] private float attackRange = 1f;   // When Skeleton should stop moving to attack

    public GameObject spawnCircle;
    private GameObject spawnInstance;
    private Boolean spawned = false;

    public Boolean isSpotted = false;
    public enum SkeletonHeadState
    { 
        IDLE, 
        CHASE, 
        APPROACH, 
        ATTACK,
        BACKOFF,
        DEAD,
    }
    public SkeletonHeadState currentState;
    
    private Coroutine actionCoroutine;
    [SerializeField] private Boolean canDecide = true;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentState = SkeletonHeadState.IDLE;
        skeletonMovement = GetComponent<SkeletonHeadMovement>();
        skeletonAttack = GetComponent<SkeletonHeadAttack>();
        health = GetComponent<UndeadHealth>();
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (!spawned)
        {
            StartCoroutine(SpawnState());
            spawned = true;
        }

        if (canDecide && !health.isDead && spawned)
        {
            StartCoroutine(DecideNextAction(distance));
        }
        else if (health.isDead) {
            if (actionCoroutine != null)
            {
                StopCoroutine(actionCoroutine);
                currentState = SkeletonHeadState.IDLE;
                skeletonMovement.StopMoving();
            }
            actionCoroutine = StartCoroutine(SkeletonDead());
        }
    }

    private IEnumerator DecideNextAction(float distance)
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Enemy"), false);
        if (actionCoroutine != null)
        {
            StopCoroutine(actionCoroutine);
        }

        yield return new WaitForSeconds(0.1f); // Small delay to prevent spam

        if (distance > detectionRange && !isSpotted)
        {
            actionCoroutine = StartCoroutine(IdleState());
        }
        if (!isSpotted && distance <= detectionRange)
        {
            actionCoroutine = StartCoroutine(NoticedState());
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
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Enemy"), true);
            actionCoroutine = StartCoroutine(AttackState());
        }
        else if (distance <= attackRange && skeletonAttack.cooldownTimer > 0 && !skeletonAttack.IsAttacking && !skeletonAttack.Is2ndAttacking)
        {
            actionCoroutine = StartCoroutine(BackOffState());
        }

    }

    private IEnumerator SpawnState()
    {
        canDecide = false;
        skeletonMovement.StopMoving();
        spawnInstance = Instantiate(spawnCircle, transform.position, Quaternion.identity);
        spawnInstance.transform.position += new Vector3(0, -1f, 0);

        spawnInstance.transform.parent = null;

        yield return new WaitForSeconds(1f);

        Destroy(spawnInstance);

        canDecide = true;
    }
    private IEnumerator IdleState()
    {
        canDecide = false;
        currentState = SkeletonHeadState.IDLE;
        skeletonMovement.StopMoving();
        yield return null;

        canDecide = true;
    }

    private IEnumerator NoticedState()
    {
        canDecide = false;
        currentState = SkeletonHeadState.IDLE;
        skeletonMovement.NoticedPlayer();
        yield return new WaitForSeconds(1f);
        isSpotted = true;

        canDecide = true;
    }

    private IEnumerator ChaseState()
    {
        canDecide = false;
        currentState = SkeletonHeadState.CHASE;
        skeletonMovement.ChasePlayer();
        yield return new WaitForSeconds(0.1f);
        canDecide = true;
    }

    private IEnumerator ApproachState()
    {
        canDecide = false;
        currentState = SkeletonHeadState.APPROACH;
        skeletonMovement.ApproachPlayer();
        yield return new WaitForSeconds(0.1f);
        canDecide = true;
    }

    private IEnumerator AttackState()
    {
        canDecide = false;
        skeletonMovement.StopMoving();
        currentState = SkeletonHeadState.ATTACK;
        skeletonAttack.Attack();
        yield return new WaitForSeconds(1f);
        canDecide = true;
    }

    private IEnumerator BackOffState()
    {
        canDecide = false;
        currentState = SkeletonHeadState.BACKOFF;
        skeletonMovement.BackOffPlayer();
        yield return new WaitForSeconds(0.1f);
        canDecide = true;
    }

    private IEnumerator SkeletonDead()
    {
        canDecide = false;
        currentState = SkeletonHeadState.DEAD;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Enemy"), true);
        skeletonMovement.StopMoving();
        yield return null;
    }
}
