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
        ATTACK_SPECIAL
    }

    [SerializeField] private FlyingEyeActions currentAction = FlyingEyeActions.IDLE;
    private GameObject player;
    private FlyingEyeManager flyingEyeManager;
    private FlyingEyeMovement flyingEyeMovement;
    private FlyingEyeBite flyingEyeBite;
    private FlyingEyeAnimator flyingEyeAnimator;

    [SerializeField] private float attackCooldown= 3f;

    private float attackCooldownTimer;

    private Coroutine actionCoroutine;

    [SerializeField] private float moveAroundTime = 2f;


    private void Start()
    {
        flyingEyeManager = GetComponent<FlyingEyeManager>();
        flyingEyeMovement = GetComponent<FlyingEyeMovement>();
        flyingEyeBite = GetComponent<FlyingEyeBite>();
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
            actionCoroutine = StartCoroutine(AttackBite());
        }
        else actionCoroutine = StartCoroutine(MoveAround());
    }


    private IEnumerator AttackBite()
    {
        try
        {
            currentAction = FlyingEyeActions.ATTACK_CLOSE;
            flyingEyeBite.Attack();
            flyingEyeBite.isAttacking = true;
            while (flyingEyeBite.isAttacking) yield return null;
        }
        finally
        {
            currentAction = FlyingEyeActions.IDLE;
            attackCooldownTimer = attackCooldown;
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
