using Assets.Script.Enemy.FlyingEye;
using System.Collections;
using UnityEngine;
using static RoninAction;

public class FlyingEyeAction : MonoBehaviour
{
    // FINDING_PLAYER is finding player in the system itself
    public enum FlyingEyeActions
    {
        FINDING_PLAYER,
        IDLE,
        DECIDING,
        CHASE,
        MOVE,
        ATTACK_CLOSE,
        ATTACK_RANGED,
        ATTACK_SPECIAL,
        ATTACK_DASH,
    }

    [SerializeField] private FlyingEyeActions currentAction = FlyingEyeActions.IDLE;
    private GameObject player;
    private FlyingEyeManager flyingEyeManager;
    private FlyingEyeMovement flyingEyeMovement;
    private FlyingEyeBite flyingEyeBite;
    private FlyingEyeShoot flyingEyeShoot;
    private FlyingEyeSpecial flyingEyeSpecial;
    private FlyingEyeDashAttack flyingEyeDashAttack;
    private FlyingEyeAnimator flyingEyeAnimator;

    [SerializeField] private float attackCooldown= 3f;

    private float attackCooldownTimer;

    private Coroutine actionCoroutine;

    [SerializeField] private float moveAroundTime = 2f;

    [SerializeField] private float distanceFromPlayer;


    private void Start()
    {
        flyingEyeManager = GetComponent<FlyingEyeManager>();
        flyingEyeMovement = GetComponent<FlyingEyeMovement>();
        flyingEyeBite = GetComponent<FlyingEyeBite>();
        flyingEyeShoot = GetComponent<FlyingEyeShoot>();
        flyingEyeSpecial = GetComponent<FlyingEyeSpecial>();
        flyingEyeDashAttack = GetComponent<FlyingEyeDashAttack>();
        flyingEyeAnimator = GetComponent<FlyingEyeAnimator>();
        player = flyingEyeManager.Player;
        if(player == null)
        {
            flyingEyeManager.OnPlayerFound += RegisterPlayer;
        }
        attackCooldownTimer = attackCooldown;
    }

    private void Update()
    {
        if(player == null)
        {
            currentAction = FlyingEyeActions.FINDING_PLAYER;
            return;
        }

        if (player != null)
        {
            distanceFromPlayer = Vector3.Distance(transform.position, player.transform.position);
        }

        switch (currentAction) {
            case FlyingEyeActions.FINDING_PLAYER:
                if (player != null) currentAction = FlyingEyeActions.IDLE;
                break;

            case FlyingEyeActions.IDLE:
                currentAction = FlyingEyeActions.DECIDING;
                DecideNextAction();
                break;
            default:
                break;
        }

        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
            attackCooldownTimer = Mathf.Clamp(attackCooldownTimer, 0f, attackCooldown);
        }

    }

    private void DecideNextAction()
    {
        if (attackCooldownTimer == 0)
        {
            actionCoroutine = StartCoroutine(AttackDash());
        }
        else actionCoroutine = StartCoroutine(MoveAround());
    }


    private IEnumerator AttackBite()
    {
        Coroutine attackBiteCoroutine = null;
        try
        {
            // If player is too far away, chase after player until they are within range.
            // distance before stop chasing is randomized, giving less of a "robot" feeling.
            float stopChaseDistance = UnityEngine.Random.Range(2f, 3f);
            while (distanceFromPlayer > stopChaseDistance)
            {
                flyingEyeAnimator.ChangeDirectionFacePlayer();
                currentAction = FlyingEyeActions.CHASE;
                flyingEyeMovement.isMoving = true;
                flyingEyeMovement.isChasingPlayer = true;
                yield return null;
            }
            flyingEyeMovement.isMoving = false;
            flyingEyeMovement.isChasingPlayer = false;

            currentAction = FlyingEyeActions.ATTACK_DASH;
            yield return flyingEyeDashAttack.Attack();
            attackCooldownTimer = attackCooldown;
        }
        finally
        {
            if (attackBiteCoroutine != null) StopCoroutine(attackBiteCoroutine);
            currentAction = FlyingEyeActions.IDLE;
            actionCoroutine = null;
        }
    }

    private IEnumerator AttackDash()
    {
        Coroutine attackBiteCoroutine = null;
        try
        {
            // If player is too far away, chase after player until they are within range.
            // distance before stop chasing is randomized, giving less of a "robot" feeling.
            float stopChaseDistance = UnityEngine.Random.Range(6f, 9f);
            while (distanceFromPlayer > stopChaseDistance)
            {
                flyingEyeAnimator.ChangeDirectionFacePlayer();
                currentAction = FlyingEyeActions.CHASE;
                flyingEyeMovement.isMoving = true;
                flyingEyeMovement.isChasingPlayer = true;
                yield return null;
            }
            flyingEyeMovement.isMoving = false;
            flyingEyeMovement.isChasingPlayer = false;

            currentAction = FlyingEyeActions.ATTACK_DASH;
            yield return flyingEyeBite.Attack();
            attackCooldownTimer = attackCooldown;
        }
        finally
        {
            if (attackBiteCoroutine != null) StopCoroutine(attackBiteCoroutine);
            currentAction = FlyingEyeActions.IDLE;
            actionCoroutine = null;
        }
    }

    private IEnumerator AttackShoot()
    {
        try
        {
            currentAction = FlyingEyeActions.ATTACK_RANGED;
            yield return flyingEyeShoot.Attack();
            attackCooldownTimer = attackCooldown;
        }
        finally
        {
            currentAction = FlyingEyeActions.IDLE;
            actionCoroutine = null;
        }
    }

    private IEnumerator AttackSpecial()
    {
        try
        {
            currentAction = FlyingEyeActions.ATTACK_SPECIAL;
            yield return flyingEyeSpecial.Attack();
            attackCooldownTimer = attackCooldown;
        }
        finally
        {
            currentAction = FlyingEyeActions.IDLE;
            actionCoroutine = null;
        }
    }

    private IEnumerator MoveAround()
    {
        try
        {
            currentAction = FlyingEyeActions.MOVE;
            flyingEyeAnimator.ChangeDirectionFaceMovement();
            flyingEyeMovement.isMoving = true;
            yield return new WaitForSeconds(moveAroundTime);
        }
        finally
        {
            flyingEyeAnimator.ChangeDirectionFacePlayer();
            flyingEyeMovement.isMoving = false;
            currentAction = FlyingEyeActions.IDLE;
        }
    }

    private void RegisterPlayer(GameObject player)
    {
        this.player = player;
    }
}
