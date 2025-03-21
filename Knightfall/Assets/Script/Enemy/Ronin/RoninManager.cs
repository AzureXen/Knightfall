using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// What does Ronin Manager manages?
/// Parry - Stun Management
/// Animation Management
/// Boss Health/Stamina UI Managemet
/// Ronin SFX
/// Ronin BGM ( battle music)
/// Health
/// </summary>
public class RoninManager : EntityManager
{
    private RoninFlashSlash flashSlash;
    private RoninBGM roninBGM;

    // for UI
    [SerializeField] private GameObject RoninCanvasPrefab;
    private GameObject roninCanvasInstance = null;
    private RoninCanvasScript roninCanvasScript = null;


    [SerializeField] private float maxMotivation;
    [SerializeField] private float curMotivation;
    [SerializeField] private float motivationRegenRate;
    [SerializeField] private int maxAbsoluteDefenseStacks;
    [SerializeField] private int curAbsoluteDefenseStacks;
    [SerializeField] private float absoluteDefenseKnockbackForce;
    [SerializeField] private float absoluteDefenseKknockbackDuration;
    [SerializeField] private float stunDuration;

    [SerializeField] private float parryStateDuration = 0.75f;
    [SerializeField] public float parryStateTimer = 0;
    [SerializeField] private int resilience = 0;
    [SerializeField] private int resilienceStacksToAbsoluteDefense = 2;
    public Boolean isInParryState = false;
    private Boolean isStunned = false;

    public AudioSource audioSource;
    public AudioClip[] audioClips;

    private RoninAction roninAction;

    private RoninSFX roninSFX;

    public Boolean canParry = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        entityHealth = GetComponent<Health>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        entityMovement = GetComponent<RoninMovement>();
        isKnockingBack = false;
        takeDamageCooldown = 0.2f;
        flashDuration = 2.5f;
        EntityName = "Ronin";
        defaultColor = new Color(1, 1, 1, 1);

        curMotivation = maxMotivation;
        curAbsoluteDefenseStacks = 1;
        flashSlash = GetComponent<RoninFlashSlash>();
        roninAction = GetComponent<RoninAction>();
        roninSFX = GetComponent<RoninSFX>();
        roninBGM = GetComponent<RoninBGM>();

        roninCanvasInstance = Instantiate(RoninCanvasPrefab);
        roninCanvasScript = roninCanvasInstance.GetComponent<RoninCanvasScript>();
    }

    private void Update()
    {
        if(roninCanvasScript != null)
        {
            roninCanvasScript.healthPercentage = Mathf.Clamp((float)entityHealth.health/entityHealth.maxHealth,0,1);
            roninCanvasScript.motivationPercentage = Mathf.Clamp(curMotivation/maxMotivation,0,1);
        }
        if (parryStateTimer > 0)
        {
            parryStateTimer -= Time.deltaTime;
            isInParryState = true;
        }
        else
        {
            isInParryState = false;
            resilience = 0;
        }

        if(curMotivation < 0 && !isStunned)
        {
            StartCoroutine(parryBreakStun(stunDuration));
        }
        if(curMotivation < maxMotivation && !isStunned)
        {
            curMotivation += Time.deltaTime;
            curMotivation = Mathf.Clamp(curMotivation, 0, maxMotivation);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            flashSlash.Attack();
        }
    }

    public override Boolean TakeMeleeHit(int damage, Vector3 target, float force, float knockBackDuration, EntityManager source)
    {
        if (canDamage)
        {
            // If ronin is in the starting idle (not attacking at all) and takes a hit, he will start attacking now.
            if(roninAction.currentAction == RoninAction.RoninActions.STARTING_IDLE)
            {
                roninAction.currentAction = RoninAction.RoninActions.IDLE;
                roninBGM.playBattleBGM();
                roninCanvasScript.showCanvas();
                Debug.Log("Ronin is starting to attack player.");
                Debug.Log("Ronin's current action: " + roninAction.currentAction);
            }


            if(curAbsoluteDefenseStacks > 0 && canParry)
            {
                // Activating superParry will immediately end ParryState.
                if(parryStateTimer > 0)
                {
                    parryStateTimer = 0;
                }
                roninAction.EnterAbsoluteDefense();
                // super parry does NOT consume motivation.
                TakeHitKnockback(0, target, 0, 0);
                source.TakeKnockBack(transform.position, absoluteDefenseKnockbackForce, absoluteDefenseKknockbackDuration);
                curAbsoluteDefenseStacks--;

                roninSFX.playDeflect();
                return false;
            }
            else if(curMotivation > 0 && canParry)
            {
                // If Ronin can parry an attack, and currently is not in Parry State, enter Parry State
                if(!isInParryState)
                {
                    parryStateTimer = parryStateDuration;
                    isInParryState = true;
                    Debug.Log("Entering Parry state.");
                    roninAction.EnterParryState();
                }
                // If Ronin is in Parry State (the parryStateDuration is > 0) start accumulating Anger.
                if(parryStateTimer > 0)
                {
                    resilience++;
                    parryStateTimer = parryStateDuration;
                    if (resilience >= resilienceStacksToAbsoluteDefense)
                    {
                        resilience = 0;
                        curAbsoluteDefenseStacks++;
                        curAbsoluteDefenseStacks = Mathf.Clamp(curAbsoluteDefenseStacks, 0, maxAbsoluteDefenseStacks);
                    }
                }
                curMotivation -= damage;
                TakeHitKnockback(0, target, force * 0.25f, knockBackDuration * 0.25f);
                source.TakeKnockBack(transform.position, force * 0.75f, knockBackDuration * 0.75f);

                roninSFX.playBlock();
                return false;

            }
            // If motivation is > 0 but currently cannot parry, reduce 50% damage and 50% knockback.
            else if(curMotivation > 0)
            {
                TakeHitKnockback(damage/2, target, force * 0.5f, knockBackDuration * 0.5f);
                return true;
            }
            else
            {
                TakeHitKnockback(damage, target, force, knockBackDuration);
                return true;
            }
        }
        return false;
    }

    public override void TakeRangedHit(int damage, Vector3 target, float force, float knockBackDuration, BulletScript bullet)
    {
        if (canDamage)
        {
            // If ronin is in the starting idle (not attacking at all) and takes a hit, he will start attacking now.
            if (roninAction.currentAction == RoninAction.RoninActions.STARTING_IDLE)
            {
                roninAction.currentAction = RoninAction.RoninActions.IDLE;
                roninBGM.playBattleBGM();
                roninCanvasScript.showCanvas();
                Debug.Log("Ronin is starting to attack player.");
                Debug.Log("Ronin's current action: " + roninAction.currentAction);
            }
            if (curMotivation > 0 && canParry)
            {
                if (!isInParryState)
                {
                    parryStateTimer = parryStateDuration;
                    isInParryState = true;
                    Debug.Log("Entering Parry state.");
                    roninAction.EnterParryState();
                }
                // If Ronin is in Parry State (the parryStateDuration is > 0) start accumulating Anger.
                // Ranged Hits will not accumulate resilience
                if (parryStateTimer > 0)
                {
                    parryStateTimer = parryStateDuration;
                    if (resilience >= resilienceStacksToAbsoluteDefense)
                    {
                        resilience = 0;
                        curAbsoluteDefenseStacks++;
                        curAbsoluteDefenseStacks = Mathf.Clamp(curAbsoluteDefenseStacks, 0, maxAbsoluteDefenseStacks);
                    }
                }
                TakeHitKnockback(0, target, 0, 0);
                roninSFX.playBlock();
                //play audio
                //audioSource.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
                //if (audioClips.Length > 0)
                //{
                //    int random = UnityEngine.Random.Range(0, audioClips.Length);
                //    audioSource.clip = audioClips[random];
                //    audioSource.Play();
                //}
                //else
                //{
                //    Debug.LogWarning("Audio for ParryHit not found.");
                //}

                bullet.BreakBullet();
            }
            else
            {
                TakeHitKnockback(damage, target, force, knockBackDuration);
                bullet.DestroyBullet();
            }
        }
    }

    protected IEnumerator parryBreakStun(float duration)
    {
        isStunned = true;
        roninAction.TakeStun(duration);
        yield return new WaitForSeconds(duration);
        isStunned = false;
        curMotivation = maxMotivation;
        curAbsoluteDefenseStacks++;
        curAbsoluteDefenseStacks = Mathf.Clamp(curAbsoluteDefenseStacks, 0, maxAbsoluteDefenseStacks);
    }

}
