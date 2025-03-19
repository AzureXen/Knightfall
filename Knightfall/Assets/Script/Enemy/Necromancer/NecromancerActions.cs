using System;
using System.Collections;
using UnityEngine;
using static RoninAction;
using static SkeletonActions;
using static UnityEngine.Rendering.DebugUI;

public class NecromancerActions : MonoBehaviour
{
    private Transform player;
    private NecromancerMovement necromancerMovement;
    private NecromancerAttack necromancerAttack;
    private UndeadHealth health;

    [SerializeField] private float detectionRange = 10f;  // When Skeleton starts chasing
    [SerializeField] private float approachRange = 3f;   // When Skeleton starts slowing down
    [SerializeField] private float attackRange = 1f;   // When Skeleton should stop moving to attack

    public GameObject spawnCircle;
    private GameObject spawnInstance;
    private Boolean spawned = false;

    private Boolean isSpotted = false;
    public enum NecromancerState
    { 
        IDLE, 
        CHASE, 
        APPROACH, 
        ATTACK, ATTACK2,
        BACKOFF,
        OOM,
        DEAD,
    }
    public NecromancerState currentState;
    
    private Coroutine actionCoroutine;
    [SerializeField] private Boolean canDecide = true;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentState = NecromancerState.IDLE;
        necromancerMovement = GetComponent<NecromancerMovement>();
        necromancerAttack = GetComponent<NecromancerAttack>();
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
                currentState = NecromancerState.IDLE;
                necromancerMovement.StopMoving();
            }
            actionCoroutine = StartCoroutine(NecromancerDead());
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
        if (!isSpotted && distance <= detectionRange)
        {
            actionCoroutine = StartCoroutine(NoticedState());
        }
        else if (distance > approachRange && (distance <= detectionRange || isSpotted) && necromancerAttack.cooldownTimer == 0)
        {
            actionCoroutine = StartCoroutine(ChaseState());
        }
        else if (distance > attackRange && distance <= approachRange && necromancerAttack.cooldownTimer == 0)
        {
            actionCoroutine = StartCoroutine(ApproachState());
        }
        else if (distance <= attackRange && necromancerAttack.cooldownTimer == 0)
        {
            actionCoroutine = StartCoroutine(AttackState());
        }
        else if (distance <= attackRange && necromancerAttack.cooldownTimer > 0 && !necromancerAttack.NecroIsAttacking && !necromancerAttack.NecroIsSummoning)
        {
            actionCoroutine = StartCoroutine(BackOffState());
        }
    }

    private IEnumerator SpawnState()
    {
        canDecide = false;
        necromancerMovement.StopMoving();
        spawnInstance = Instantiate(spawnCircle, transform.position, Quaternion.identity);
        spawnInstance.transform.position += new Vector3(0, -2f, 0);

        spawnInstance.transform.parent = null;

        yield return new WaitForSeconds(1f);

        Destroy(spawnInstance);

        canDecide = true;
    }

    private IEnumerator IdleState()
    {
        canDecide = false;
        currentState = NecromancerState.IDLE;
        necromancerMovement.StopMoving();
        yield return null;

        canDecide = true;
    }

    private IEnumerator NoticedState()
    {
        canDecide = false;
        currentState = NecromancerState.IDLE;
        necromancerMovement.NoticedPlayer();
        yield return new WaitForSeconds(1f);
        isSpotted = true;

        canDecide = true;
    }

    private IEnumerator ChaseState()
    {
        canDecide = false;
        currentState = NecromancerState.CHASE;
        necromancerMovement.ChasePlayer();
        yield return new WaitForSeconds(0.1f);
        canDecide = true;
    }

    private IEnumerator ApproachState()
    {
        canDecide = false;
        currentState = NecromancerState.APPROACH;
        necromancerMovement.ApproachPlayer();
        yield return new WaitForSeconds(0.1f);
        canDecide = true;
    }

    private IEnumerator AttackState()
    {
        canDecide = false;
        necromancerMovement.StopMoving();
        float diceRoll = UnityEngine.Random.value; // 80/20 chance
        if (diceRoll <= 0.4f && !necromancerAttack.IsOnCooldown)
        {
            currentState = NecromancerState.ATTACK;
            necromancerAttack.NecroIsAttacking = true;
            necromancerAttack.Attack();
            yield return new WaitForSeconds(necromancerAttack.attackDuration);
            necromancerAttack.NecroIsAttacking = false;
            currentState = NecromancerState.IDLE;
        }
        else if (diceRoll > 0.4f && !necromancerAttack.IsOnCooldown)
        {
            currentState = NecromancerState.ATTACK2;
            necromancerAttack.NecroIsSummoning = true;
            necromancerAttack.Attack2();
            yield return new WaitForSeconds(necromancerAttack.attackDuration - 2.7f);
            necromancerAttack.NecroIsSummoning = false;
            currentState = NecromancerState.IDLE;
        }
        canDecide = true;
    }

    private IEnumerator BackOffState()
    {
        canDecide = false;
        currentState = NecromancerState.BACKOFF;
        necromancerMovement.BackOffPlayer();
        yield return new WaitForSeconds(0.1f);
        canDecide = true;
    }

    private IEnumerator NecromancerDead()
    {
        canDecide = false;
        currentState = NecromancerState.DEAD;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Enemy"), true);
        necromancerMovement.StopMoving();
        yield return null;
    }

    public void OutOfMana(float duration)
    {
        StartCoroutine(Manaless(duration));
    }
    private IEnumerator Manaless(float duration)
    {
        canDecide = false;
        currentState = NecromancerState.OOM;
        necromancerMovement.StopMoving();
        Animator animator = GetComponent<Animator>();
        animator.SetTrigger("ManaRecover");

        if (actionCoroutine != null)
        {
            StopCoroutine(actionCoroutine);
            necromancerMovement.StopMoving();
        }
        yield return new WaitForSeconds(duration-0.1f);
        canDecide = true;
    }
}
