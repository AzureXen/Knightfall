using System;
using System.Collections;
using UnityEngine;

public class RoninAction : MonoBehaviour
{
    private Transform player;
    [SerializeField] private float distanceFromPlayer;
    // STARTING_IDLE: Stands completely still until player attacks, currently only applies at start.
    // IDLE: Boss is standing still and Require next action.
    public enum RoninActions
    {
        STARTING_IDLE,
        IDLE,
        CHASE,
        MOVE,
        ATTACK,
        STUNNED,
    }
    public RoninActions currentAction;

    // Cooldown between each attacks from ronin. During this time he may only chase or move around.
    public float attackCooldown = 2f;
    private float attackCooldownTimer;

    private RoninMovement roninMovement;
    private RoninFlashSlash roninFlashSlash;
    private float flashSlashCooldown;

    private Coroutine ActionCoroutine;
    [SerializeField] private Boolean canDecide = true;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentAction = RoninActions.STARTING_IDLE;
        roninFlashSlash = GetComponent<RoninFlashSlash>();
        roninMovement = GetComponent<RoninMovement>();
    }
    private void Update()
    {
        distanceFromPlayer = Vector3.Distance(transform.position, player.position);
        flashSlashCooldown = roninFlashSlash.attackCoolDownTimer;
        
        if(attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
            attackCooldownTimer = Mathf.Clamp(attackCooldownTimer, 0, attackCooldown);
        }
        if(currentAction == RoninActions.IDLE && canDecide)
        {
            StartCoroutine(DecideNextAction(distanceFromPlayer));
        }
    }

    private IEnumerator DecideNextAction(float distance)
    {
        Debug.Log("Ronin is deciding next action.");
        canDecide = false;
        if(ActionCoroutine != null)
        {
            StopCurrentAction();
        }

        // This is to prevent spam.
        yield return new WaitForSeconds(0.1f);

        if (attackCooldownTimer <= 0) 
        {
            if(flashSlashCooldown <= 0)
            {
                if (distance < 13f)
                {
                    ActionCoroutine = StartCoroutine(FlashSlash());
                    canDecide = true;
                    yield break;
                }
            }
        }
        else
        {
            ActionCoroutine = StartCoroutine(MoveAround());
            canDecide = true;
        }
    }
    private void StopCurrentAction()
    {
        StopCoroutine(ActionCoroutine);
        currentAction = RoninActions.IDLE;
        ActionCoroutine = null;
        roninMovement.isMoving = false;
    }

    public void TakeStun(float duration)
    {
        StartCoroutine(Stun(duration));
    }
    private IEnumerator Stun(float duration)
    {
        canDecide = false;
        if (ActionCoroutine != null)
        {
            StopCurrentAction();
        }
        currentAction = RoninActions.STUNNED;
        yield return new WaitForSeconds(duration);
        canDecide = true;
        currentAction = RoninActions.IDLE;
    }
    IEnumerator FlashSlash()
    {
        try
        {
            currentAction = RoninActions.ATTACK;
            attackCooldownTimer = attackCooldown;
            roninFlashSlash.Attack();
            yield return new WaitForSeconds(2f);
        }
        finally
        {
            currentAction = RoninActions.IDLE;
            ActionCoroutine = null;
        }
    }

    IEnumerator MoveAround()
    {
        try
        {
            currentAction = RoninActions.MOVE;
            roninMovement.isMoving = true;
            yield return new WaitForSeconds(3f);
        }
        finally
        {
            roninMovement.isMoving = false;
            currentAction = RoninActions.IDLE;
            ActionCoroutine = null;
        }
    }
}
