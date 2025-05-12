using System;
using System.Collections;
using UnityEngine;
using static SkeletonActions;

public class SkullKnightActions : MonoBehaviour
{
    private Transform player;
    private SkullKnightMovement skullKnightMovement;
    private SkullKnightAttack skullKnightAttack;
    private UndeadBossHealth health;
    private Animator animator;

    public GameObject spawnCircle;
    private GameObject spawnInstance;
    public GameObject cutScene;
    private bool spawned = false;

    public enum SkullKnightState
    {
        IDLE,
        COMBATIDLE,
        CHASE,
        ATTACK,
        ATTACK2,
        DEAD,
    }
    public SkullKnightState currentState;

    private Coroutine actionCoroutine;
    private bool canDecide = true;
    public bool hasEnteredCombatIdle = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        skullKnightMovement = GetComponent<SkullKnightMovement>();
        skullKnightAttack = GetComponent<SkullKnightAttack>();
        health = GetComponent<UndeadBossHealth>();
        animator = GetComponent<Animator>();
        currentState = SkullKnightState.IDLE;
    }

    void Update()
    {
        if (!spawned)
        {
            StartCoroutine(SpawnState());
            spawned = true;
        }

        if (canDecide && !health.isDead && spawned)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            StartCoroutine(DecideNextAction(distance));
        }
        else if (health.isDead)
        {
            if (actionCoroutine != null)
            {
                StopCoroutine(actionCoroutine);
                currentState = SkullKnightState.IDLE;
                skullKnightMovement.StopMoving();
            }
            actionCoroutine = StartCoroutine(SkullKnightDead());
        }
    }

    private IEnumerator DecideNextAction(float distance)
    {
        if (actionCoroutine != null)
        {
            StopCoroutine(actionCoroutine);
        }
        yield return new WaitForSeconds(0.1f);

        if (!hasEnteredCombatIdle)
        {
            actionCoroutine = StartCoroutine(NoticedState());
        }
        if (skullKnightAttack.IsOnCooldown && hasEnteredCombatIdle)
        {
            actionCoroutine = StartCoroutine(IdleState());
        }
        else if (distance > 5f && hasEnteredCombatIdle && !skullKnightAttack.IsAttacking && !skullKnightAttack.Is2ndAttacking) // Chase until close to attack
        {
            actionCoroutine = StartCoroutine(ChaseState());
        }
        else if (!skullKnightAttack.IsOnCooldown && hasEnteredCombatIdle)
        {
            actionCoroutine = StartCoroutine(AttackState());
        }
    }

    private IEnumerator SpawnState()
    {
        canDecide = false;
        skullKnightMovement.StopMoving();
        spawnInstance = Instantiate(spawnCircle, transform.position, Quaternion.identity);
        spawnInstance.transform.position += new Vector3(0,-2,0);
        skullKnightAttack.IsAttacking = false;
        skullKnightAttack.Is2ndAttacking = false;
        cutScene.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 0;
        yield return new WaitForSeconds(2f);
        Time.timeScale = 1;
        cutScene.SetActive(false);
        Destroy(spawnInstance);
        canDecide = true;
    }

    private IEnumerator NoticedState()
    {
        canDecide = false;
        skullKnightMovement.StopMoving();
        currentState = SkullKnightState.IDLE;
        yield return new WaitForSeconds(2f); // 2s idle before drawing blade
        skullKnightMovement.NoticedPlayer();
        animator.SetInteger("SwordDrawn", 1);
        yield return new WaitForSeconds(1.6f); // Sword draw time
        currentState = SkullKnightState.COMBATIDLE;
        animator.SetInteger("SwordDrawn", 2);
        yield return new WaitForSeconds(4f);
        hasEnteredCombatIdle = true;

        canDecide = true;
    }

    private IEnumerator IdleState()
    {
        canDecide = false;
        currentState = SkullKnightState.COMBATIDLE;
        skullKnightMovement.StopMoving();
        yield return new WaitForSeconds(0.1f);
        skullKnightMovement.DisableMovement();
        yield return new WaitForSeconds(3f);
        canDecide = true;
    }

    private IEnumerator ChaseState()
    {
        canDecide = false;
        currentState = SkullKnightState.CHASE;
        yield return new WaitForSeconds(0.5f);
        skullKnightMovement.EnableMovement();
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Player"), true);
        skullKnightMovement.ChasePlayer();
        yield return new WaitForSeconds(1.3f);
        skullKnightMovement.StopMoving();
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Player"), false);

        canDecide = true;
    }

    private IEnumerator AttackState()
    {
        canDecide = false;
        animator.SetBool("Walk", false);
        skullKnightMovement.StopMoving();
        yield return new WaitForSeconds(0.2f);
        skullKnightMovement.DisableMovement();
        float roll = UnityEngine.Random.value;
        if (roll <= 0.6f && !skullKnightAttack.IsOnCooldown)
        {
            currentState = SkullKnightState.ATTACK;
            skullKnightAttack.Attack();
        }
        else if (roll > 0.6f && !skullKnightAttack.IsOnCooldown)
        {
            currentState = SkullKnightState.ATTACK2;
            skullKnightAttack.Attack2();
        }

        yield return new WaitForSeconds(2f);
        canDecide = true;
    }

    private IEnumerator SkullKnightDead()
    {
        canDecide = false;
        currentState = SkullKnightState.DEAD;
        skullKnightMovement.DisableMovement();
        yield return null;
    }
}