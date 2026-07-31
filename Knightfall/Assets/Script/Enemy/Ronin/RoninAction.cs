using System;
using System.Collections;
using UnityEngine;
public class RoninAction : MonoBehaviour
{
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
        BASIC_ATTACK,
        FLASHSLASH,
        SEVERANCE,
        STORMSLASH,
        PARRY,
        ABSOLUTE_DEFENSE,
        FOLLOW_UP_FLASH_SLASH,
        RETRIBUTION_SLASH,
        SUPERPARRY,
        STUNNED,
        DEFEATED,
    }
    public RoninActions currentAction;

    // Cooldown between each attacks from ronin. During this time he may only+ move around.
    public float attackCooldown = 2f;
    [SerializeField] private float attackCooldownTimer = 2f;

    private RoninBasicAttack roninBasicAttack;

    private RoninMovement roninMovement;
    private RoninManager roninManager;
    RoninSFX roninSFX;
    private RoninFlashSlash roninFlashSlash;

    private RoninHealth roninHealth;

    private RoninRetributionSlash roninRetributionSlash;

    private RoninSeverance roninSeverance;

    private RoninStormSlash roninStormSlash;

    private float stunTimer;

    private Coroutine ActionCoroutine;
    [SerializeField] private Boolean canDecide = true;

    [SerializeField] private float parryStateTimer;
    void Start()
    {
        roninHealth = GetComponent<RoninHealth>();
        roninAnimator = GetComponent<RoninAnimator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentAction = RoninActions.STARTING_IDLE;
        roninBasicAttack = GetComponent<RoninBasicAttack>();
        roninFlashSlash = GetComponent<RoninFlashSlash>();
        roninRetributionSlash = GetComponent<RoninRetributionSlash>();
        roninSeverance = GetComponent<RoninSeverance>();
        roninStormSlash = GetComponent<RoninStormSlash>();
        roninMovement = GetComponent<RoninMovement>();
        roninManager = GetComponent<RoninManager>();
        roninSFX = GetComponent<RoninSFX>();
        am = GetComponentInChildren<Animator>();
        
        ChangeAnimationState(RONIN_KNEEL);

        attackCooldownTimer = 2f;
    }
    private void Update()
    {
        if (player == null)
        {
            if(currentAction != RoninActions.IDLE)
            StopCurrentAction();
            canDecide = false;
            currentAction = RoninActions.IDLE;
        }
        

        parryStateTimer = roninManager.parryStateTimer;
        if (player != null)
        {
            distanceFromPlayer = Vector3.Distance(transform.position, player.position);
        }

        // If Ronin is attacking, he cannot parry.
        if (currentAction == RoninActions.STARTING_IDLE || 
            currentAction == RoninActions.MOVE ||  currentAction == RoninActions.PARRY)
        {
            roninManager.canParry = true;
        }
        else roninManager.canParry = false;
        
        if(attackCooldownTimer > 0 && currentAction != RoninActions.STARTING_IDLE)
        {
            attackCooldownTimer -= Time.deltaTime;
            attackCooldownTimer = Mathf.Clamp(attackCooldownTimer, 0, attackCooldown);
        }
        if(currentAction == RoninActions.IDLE && canDecide)
        {
            canDecide = false;
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
        if(ActionCoroutine != null)
        {
            StopCurrentAction();
        }

        // This is to prevent spam.
        yield return new WaitForSeconds(0.1f);

        // if is not on attack cooldown, attempt to attack. Else move.
        if (attackCooldownTimer <= 0) 
        {       
            //Attempt to attack.
            //if (roninStormSlash.attackCoolDownTimer <= 0)
            //{
            //    currentAction = RoninActions.STORMSLASH;
            //    ActionCoroutine = StartCoroutine(StormSlash());
            //    canDecide = true;
            //    yield break;
            //}
            //else
            if (roninSeverance.attackCoolDownTimer <= 0)
            {
                currentAction = RoninActions.SEVERANCE;
                ActionCoroutine = StartCoroutine(Severance());
                canDecide = true;
                yield break;
            }
            else if (roninFlashSlash.attackCoolDownTimer <= 0)
            {
                currentAction = RoninActions.FLASHSLASH;
                ActionCoroutine = StartCoroutine(FlashSlash());
                canDecide = true;
                yield break;
            }
            else
            {
                currentAction = RoninActions.BASIC_ATTACK;
                ActionCoroutine = StartCoroutine(BasicAttack());
                canDecide = true;
                yield break;
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
            if (roninManager.IsInAbsoluteOffenseState) attackCooldownTimer = 0;
            yield return new WaitForSeconds(0.5f);
        }
        finally
        {
            canDecide = true;
            currentAction = RoninActions.IDLE;
            ChangeAnimationState(RONIN_IDLE);
        }
    }


    public void EnterAbsoluteDefenseRetribution()
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
            if (roninManager.IsInAbsoluteOffenseState) attackCooldownTimer = 0;
            yield return new WaitForSeconds(0.5f);

            currentAction = RoninActions.FOLLOW_UP_FLASH_SLASH;
            yield return roninFlashSlash.AttackFollowUp();
            roninManager.curRetributionStacks--;

            attackCooldownTimer = attackCooldown;
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
            roninManager.isKnockbackResist = true;
            roninManager.IsInAbsoluteOffenseState = true;
            roninAnimator.canChangeDirection = false;
            canDecide = false;
            ChangeAnimationState(RONIN_STUNNED);
            currentAction = RoninActions.STUNNED;
            stunTimer = duration;
            while (stunTimer > 0)
            {
                if (roninManager.isDefeated)
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
            roninManager.isKnockbackResist = false;
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
            yield return roninFlashSlash.Attack();

            // IF RONIN HEALTH IS IN ABSOLUTE OFFENSE STATE, FLASH SLASH WILL NOT APLPLY ATTACK COOLDOWN
            // He is going craazy
            if(!roninManager.IsInAbsoluteOffenseState) attackCooldownTimer = attackCooldown;
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

    IEnumerator StormSlash()
    {
        try
        {
            attackCooldownTimer = attackCooldown;
            roninStormSlash.Attack();
            yield return null;
            while (roninStormSlash.isAttacking)
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

    IEnumerator BasicAttack()
    {
        Coroutine basicAtkCoroutine = null;
        try
        {
            // If player is too far away, chase after player until they are within range.
            // distance before stop chasing is randomized, giving less of a "robot" feeling.
            float stopChaseDistance = UnityEngine.Random.Range(2f, 3f);
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

            currentAction = RoninActions.BASIC_ATTACK;
            attackCooldownTimer = attackCooldown;
            basicAtkCoroutine = roninBasicAttack.Attack();
            yield return null;
            while (roninBasicAttack.isAttacking)
            {
                yield return null;
            }
            attackCooldownTimer = attackCooldown;
        }
        finally
        {
            if(basicAtkCoroutine != null) StopCoroutine(basicAtkCoroutine);
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

    
}
