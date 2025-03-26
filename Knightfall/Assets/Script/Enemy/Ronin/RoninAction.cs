using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class RoninAction : MonoBehaviour
{
    // FOR DEATH SCENE
    private Health roninHealth;
    private int currentHealth;
    public Boolean isDefeated = false;


    // FOR ANIMATOR
    private string RONIN_IDLE = "RoninIdle";
    private string RONIN_RUN = "RoninRun";
    private string RONIN_RUN_SLOW = "RoninRunSlow";
    //private string RONIN_ATTACK_1 = "RoninAttack1";
    private string RONIN_RETRIBUTIONSLASH_WARNING = "RoninRetributionSlashWarning";
    private string RONIN_ATTACK_1_QUICK = "RoninAttack1Quick";
    private string RONIN_ATTACK_2 = "RoninAttack2";
    //private string RONIN_ATTACK_COMBO = "RoninAttackCombo";
    private string RONIN_DEATH = "RoninDeath";
    //private string RONIN_UNSHEATHE_1 = "RoninUnsheathe1";
    private string RONIN_UNSHEATHE_2 = "RoninUnsheathe2";
    //private string RONIN_FLASHSHASH_WARNING = "RoninFlashSlashWarning";
    private string RONIN_STUNNED = "RoninStunned";
    private string RONIN_KNEEL = "RoninKneel";
    //private string RONIN_SEVERANCE_WARNING = "RoninSeveranceWarning";

    private RoninAnimator roninAnimator;
    private string currentState;


    private Animator am;
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
        FLASHSLASH,
        SEVERANCE,
        PARRY,
        ABSOLUTE_DEFENSE,
        RETRIBUTION_SLASH,
        SUPERPARRY,
        STUNNED,
        DEFEATED,
    }
    public RoninActions currentAction;

    // Cooldown between each attacks from ronin. During this time he may only chase or move around.
    public float attackCooldown = 2f;
    private float attackCooldownTimer;

    private RoninMovement roninMovement;
    private RoninManager roninManager;
    RoninSFX roninSFX;
    private RoninFlashSlash roninFlashSlash;
    private float flashSlashCooldown;

    private RoninRetributionSlash roninRetributionSlash;

    private RoninSeverance roninSeverance;
    private float severanceCoolDown;

    private float stunTimer;

    private Coroutine ActionCoroutine;
    [SerializeField] private Boolean canDecide = true;

    [SerializeField] private float parryStateTimer;
    void Start()
    {
        roninAnimator = GetComponent<RoninAnimator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentAction = RoninActions.STARTING_IDLE;
        roninFlashSlash = GetComponent<RoninFlashSlash>();
        roninRetributionSlash = GetComponent<RoninRetributionSlash>();
        roninSeverance = GetComponent<RoninSeverance>();
        roninMovement = GetComponent<RoninMovement>();
        roninManager = GetComponent<RoninManager>();
        roninSFX = GetComponent<RoninSFX>();
        am = GetComponent<Animator>();
        roninHealth = GetComponent<Health>();
        ChangeAnimationState(RONIN_KNEEL);
    }
    private void Update()
    {
        // FOR DEATH SCENE
        if (roninHealth != null)
        {
            currentHealth = roninHealth.health;
            if (currentHealth <= 0)
            {
                isDefeated = true;
                roninManager.canDamage = false;
            }
        }
        if(isDefeated && currentAction != RoninActions.DEFEATED)
        {
            EnterDeath();
        }

        parryStateTimer = roninManager.parryStateTimer;
        if (player != null)
        {
            distanceFromPlayer = Vector3.Distance(transform.position, player.position);
        }
        flashSlashCooldown = roninFlashSlash.attackCoolDownTimer;
        severanceCoolDown = roninSeverance.attackCoolDownTimer;

        // If Ronin is attacking, he cannot parry.
        if (currentAction == RoninActions.IDLE || 
            currentAction == RoninActions.STARTING_IDLE || 
            currentAction == RoninActions.MOVE || 
            currentAction == RoninActions.CHASE || currentAction == RoninActions.PARRY)
        {
            roninManager.canParry = true;
        }
        else roninManager.canParry = false;
        
        if(attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
            attackCooldownTimer = Mathf.Clamp(attackCooldownTimer, 0, attackCooldown);
        }
        if(currentAction == RoninActions.IDLE && canDecide)
        {
            StartCoroutine(DecideNextAction());
        }

        // Timers
        if(stunTimer > 0)
        {
            stunTimer -= Time.deltaTime;
            stunTimer = Mathf.Clamp(stunTimer, 0, 999);
        }
    }

    // ANIMATION
    public void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;

        am.Play(newState);

        currentState = newState;
    }   


    private IEnumerator DecideNextAction()
    {
        Debug.Log("Ronin is deciding next action.");
        canDecide = false;
        if(ActionCoroutine != null)
        {
            StopCurrentAction();
        }

        // This is to prevent spam.
        yield return new WaitForSeconds(0.1f);

        // if is not on attack cooldown, attempt to attack. Else move.
        if (attackCooldownTimer <= 0) 
        {
            // Attempt to attack.
            if (severanceCoolDown <= 0)
            {
                ActionCoroutine = StartCoroutine(Severance());
                canDecide = true;
                yield break;
            }
            else if (flashSlashCooldown <= 0)
            {

                ActionCoroutine = StartCoroutine(FlashSlash());
                canDecide = true;
                yield break;
            }
            // If attempted to attack, all attack moves are on cool down, move.
            else
            {
                ActionCoroutine = StartCoroutine(MoveAround());
                canDecide = true;
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
        if(currentAction == RoninActions.FLASHSLASH)
        {
            roninFlashSlash.StopAttackImmediate();
        }
        StopCoroutine(ActionCoroutine);
        currentAction = RoninActions.IDLE;
        ActionCoroutine = null;
        roninMovement.isMoving = false;
    }

    public void TakeStun(float duration)
    {
        if (ActionCoroutine != null)
        {
            StopCurrentAction();
        }
        ActionCoroutine = StartCoroutine(Stun(duration));
    }
    public void EnterParryState()
    {
        if (currentAction == RoninAction.RoninActions.PARRY) return;
        if (ActionCoroutine != null)
        {
            StopCurrentAction();
        }
        ActionCoroutine = StartCoroutine(ParryState());
    }
    public IEnumerator ParryState()
    {
        try
        {
            roninAnimator.canChangeDirection = true;
            canDecide = false;
            yield return null;
            ChangeAnimationState(RONIN_UNSHEATHE_2);
            currentAction = RoninActions.PARRY;
            while (parryStateTimer > 0)
            {
                yield return null;
                if (currentAction != RoninActions.PARRY)
                {
                    EnterParryState();
                }
            }
        }
        finally
        {
            Debug.Log("Exiting Parry State");
            roninAnimator.canChangeDirection=false;
            canDecide = true;
            currentAction = RoninActions.IDLE;
            ChangeAnimationState(RONIN_IDLE);
        }
    }
    public void EnterAbsoluteDefense()
    {
        if (ActionCoroutine != null && currentAction != RoninActions.ABSOLUTE_DEFENSE)
        {
            StopCurrentAction();
        }
        ActionCoroutine = StartCoroutine(AbsoluteDefense());
    }
    private IEnumerator AbsoluteDefense()
    {
        try
        {
            canDecide = false;
            currentAction = RoninActions.ABSOLUTE_DEFENSE;
            ChangeAnimationState(RONIN_ATTACK_1_QUICK);
            yield return new WaitForSeconds(0.5f);
        }
        finally
        {
            canDecide = true;
            currentAction = RoninActions.IDLE;
            ChangeAnimationState(RONIN_IDLE);
        }
    }


    public void EnterAbsoluteDefenseRetribtion()
    {
        if(currentAction != RoninActions.ABSOLUTE_DEFENSE && currentAction != RoninActions.RETRIBUTION_SLASH)
        {
            if (ActionCoroutine != null)
            {
                StopCurrentAction();
            }
            ActionCoroutine = StartCoroutine(AbsoluteDefenseRetribution());
        }
    }

    private IEnumerator AbsoluteDefenseRetribution()
    {
        try
        {
            canDecide = false;
            currentAction = RoninActions.ABSOLUTE_DEFENSE;
            ChangeAnimationState(RONIN_ATTACK_1_QUICK);
            yield return new WaitForSeconds(0.5f);

            currentAction = RoninActions.RETRIBUTION_SLASH;
            ChangeAnimationState(RONIN_RETRIBUTIONSLASH_WARNING);
            roninSFX.playAttackWarning(2);
            yield return new WaitForSeconds(1f);

            ChangeAnimationState(RONIN_ATTACK_2);
            roninSFX.playRetributionSlash();
            yield return new WaitForSeconds(0.35f);

            roninRetributionSlash.Attack();
            roninManager.curRetributionStacks--;
            yield return new WaitForSeconds(1.5f);
        }
        finally
        {
            canDecide = true;
            currentAction = RoninActions.IDLE;
            ChangeAnimationState(RONIN_IDLE);
        }
    }

    private IEnumerator Stun(float duration)
    {
        try
        {
            roninAnimator.canChangeDirection = false;
            canDecide = false;
            ChangeAnimationState(RONIN_STUNNED);
            currentAction = RoninActions.STUNNED;
            stunTimer = duration;
            while (stunTimer > 0)
            {
                if (isDefeated)
                {
                    stunTimer = 0;
                    yield break;
                }
                yield return null;
                if(currentAction != RoninActions.STUNNED)
                {
                    TakeStun(stunTimer);
                }
            }
        }
        finally
        {
            attackCooldownTimer = 1f;
            canDecide = true;
            ChangeAnimationState(RONIN_IDLE);
            currentAction = RoninActions.IDLE;
        }
    }


    public void EnterDeath()
    {
        if(currentAction != RoninActions.DEFEATED)
        {
            if (ActionCoroutine != null)
            {
                StopCurrentAction();
            }
            ActionCoroutine = StartCoroutine(DeathScene());
        }
        openGate();
    }

    IEnumerator DeathScene()
    {   
        try
        {
            roninAnimator.canChangeDirection = false;
            canDecide = false;
            ChangeAnimationState(RONIN_DEATH);
            currentAction = RoninActions.DEFEATED;
            roninManager.canDamage = false;
            roninManager.canParry = false;
            attackCooldownTimer = 9999f;
            yield return new WaitForSeconds(9999f);
        }
        finally
        {
            attackCooldownTimer = 1f;
            ChangeAnimationState(RONIN_IDLE);
            currentAction = RoninActions.IDLE;
        }
    }

    IEnumerator FlashSlash()
    {
        try
        {
            // If player is too far away, chase after player until they are within range.
            // distance before stop chasing is randomized, giving less of a "robot" feeling.
            float stopChaseDistance = UnityEngine.Random.Range(8f, 10f);
            while (distanceFromPlayer > stopChaseDistance)
            {
                ChangeAnimationState(RONIN_RUN);
                roninAnimator.canChangeDirection = true;
                currentAction = RoninActions.CHASE;
                roninMovement.isMoving = true;
                roninMovement.isChasing = true;
                yield return null;
            }
            roninAnimator.canChangeDirection = false;
            ChangeAnimationState(RONIN_IDLE);
            roninMovement.isMoving = false;
            roninMovement.isChasing = false;

            currentAction = RoninActions.FLASHSLASH;
            attackCooldownTimer = attackCooldown;
            roninFlashSlash.Attack();
            yield return null;
            while (roninFlashSlash.isAttacking)
            {
                yield return null;
            }
        }
        finally
        {
            currentAction = RoninActions.IDLE;
            ActionCoroutine = null;
        }
    }

    IEnumerator Severance()
    {
        try
        {
            currentAction = RoninActions.SEVERANCE;
            attackCooldownTimer = attackCooldown;
            roninSeverance.Attack();
            yield return null;
            while (roninSeverance.isAttacking)
            {
                yield return null;
            }
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
            roninAnimator.canChangeDirection = true;
            ChangeAnimationState(RONIN_RUN_SLOW);
            currentAction = RoninActions.MOVE;
            roninMovement.isMoving = true;
            yield return new WaitForSeconds(3f);
        }
        finally
        {
            roninAnimator.canChangeDirection = false;
            roninMovement.isMoving = false;
            ChangeAnimationState(RONIN_IDLE);
            currentAction = RoninActions.IDLE;
            ActionCoroutine = null;
        }
    }

    void openGate()
    {
        GameObject map = GameObject.FindGameObjectWithTag("Map");
        GameObject spikes = map.transform.Find("Spikes").gameObject;
        if (spikes != null)
        {
            spikes.SetActive(false);
        }
        else
        {
            Debug.Log("Cannot find gate.");
        }
    }
}
