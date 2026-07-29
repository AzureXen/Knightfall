using System;
using System.Collections;
using UnityEngine;
using static RoninAction;
using static Unity.VisualScripting.Member;

/// <summary>
/// What does Ronin Manager manages?
/// Parry - Stun Management
/// Boss Health/Stamina UI Managemet
/// Ronin SFX
/// Ronin BGM ( battle music)
/// Health
/// </summary>
public class RoninManager : EnemyManager
{
    // FOR DEATH SCENE
    private Health roninHealth;
    private int currentHealth;
    public Boolean isDefeated = false;

    private RoninAlphaLevel roninLevelManager;

    [SerializeField] private GameObject parryEffectTransform;
    [SerializeField] private GameObject parryEffectPrefab;

    [SerializeField] private int maxRetributionStacks;
    [SerializeField] private int resolveStacksToRetribution = 2;
    public int curRetributionStacks;
    private float resolve;

    public GameObject deflectedBullet;
    private RoninSeverance roninSeverance;
    private RoninBGM roninBGM;

    // for UI
    [SerializeField] private GameObject RoninCanvasPrefab;
    public GameObject roninCanvasInstance = null;
    private RoninCanvasScript roninCanvasScript = null;

    private GameObject player;

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
    [SerializeField] private float resilience = 0;
    [SerializeField] private int resilienceStacksToAbsoluteDefense = 2;
    public Boolean isInParryState = false;
    private Boolean isStunned = false;

    public AudioSource audioSource;
    public AudioClip[] audioClips;

    private RoninAction roninAction;

    private RoninSFX roninSFX;

    private RoninManager roninManager;

    public Boolean canParry = false;

    // SINGLE SLASH TEST
    [SerializeField] private GameObject singleSlashAttackZone;
    public override void Start()
    {
        touchDamageEnabled = false;
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
        roninSeverance = GetComponent<RoninSeverance>();
        roninAction = GetComponent<RoninAction>();
        roninSFX = GetComponent<RoninSFX>();
        roninBGM = GetComponent<RoninBGM>();

        roninCanvasInstance = Instantiate(RoninCanvasPrefab);
        roninCanvasScript = roninCanvasInstance.GetComponent<RoninCanvasScript>();
        
        roninManager = GetComponent<RoninManager>();

        roninHealth = GetComponent<Health>();

        player = GameManagerScript.Instance.player;
        if (player == null) GameManagerScript.OnPlayerReady += RegisterPlayer;

        GameObject levelManager = GameObject.FindGameObjectWithTag("LevelManager");
        if (levelManager == null)
        {
            Debug.Log("Level Manager not found!");
        }
        else
        {
            roninLevelManager = levelManager.GetComponent<RoninAlphaLevel>();
            if (roninLevelManager == null)
            {
                Debug.Log("Ronin Level Manager component not found!");
            }
        }

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

        if(curMotivation < 0 && (!isStunned || roninAction.currentAction != RoninAction.RoninActions.STUNNED) && !isDefeated)
        {
            StartCoroutine(parryBreakStun(stunDuration));
        }
        if(curMotivation < maxMotivation && !isStunned)
        {
            curMotivation += Time.deltaTime;
            curMotivation = Mathf.Clamp(curMotivation, 0, maxMotivation);
        }
        if (roninHealth != null)
        {
            currentHealth = roninHealth.health;
            if (currentHealth <= 0)
            {
                isDefeated = true;
                roninManager.canDamage = false;
            }
        }
        if (isDefeated && roninAction.currentAction != RoninActions.DEFEATED)
        {
            EnterDeath();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            StartCoroutine(SpawnSingleAttackZoneTest());
        }
    }

    public override Boolean TakeMeleeHit(int damage, Vector3 target, float force, float knockBackDuration, EntityManager source)
    {
        if (canDamage)
        {
            // If ronin is in the starting idle (not attacking at all) and takes a hit, he will start attacking now.
            if(roninAction.currentAction == RoninAction.RoninActions.STARTING_IDLE)
            {
                EnterBattle();
                //Debug.Log("Ronin is starting to attack player.");
                //Debug.Log("Ronin's current action: " + roninAction.currentAction);
            }

            if (curAbsoluteDefenseStacks > 0 && canParry && curRetributionStacks > 0)
            {
                // Activating superParry will immediately end ParryState.
                if (parryStateTimer > 0)
                {
                    parryStateTimer = 0;
                }

                SpawnAbsoluteDefenseParryEffect();
                roninAction.EnterAbsoluteDefenseRetribution();

                // super parry does NOT consume motivation.
                TakeHitKnockback(0, target, 0, 0);
                source.TakeKnockBack(transform.position, absoluteDefenseKnockbackForce, absoluteDefenseKknockbackDuration);
                // curRetributionStacks is removed at RoninAction, at roninAction.EnterAbsoluteDefenseRetribution(); .
                curAbsoluteDefenseStacks--;

                roninSFX.playDeflect();
                return false;
            }


            else if (curAbsoluteDefenseStacks > 0 && canParry)
            {
                // Activating superParry will immediately end ParryState.
                if(parryStateTimer > 0)
                {
                    parryStateTimer = 0;
                }

                SpawnAbsoluteDefenseParryEffect();
                roninAction.EnterAbsoluteDefense();

                // super parry does NOT consume motivation.
                TakeHitKnockback(0, target, 0, 0);
                source.TakeKnockBack(transform.position, absoluteDefenseKnockbackForce, absoluteDefenseKknockbackDuration);
                curAbsoluteDefenseStacks--;
                resolve++;
                if(resolve >= resolveStacksToRetribution)
                {
                    resolve = 0;
                    curRetributionStacks++;
                }
                curRetributionStacks = Mathf.Clamp(curRetributionStacks, 0, maxRetributionStacks);
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
                TakeHitKnockback(0, target, force * 0.5f, knockBackDuration * 0.5f);
                source.TakeKnockBack(transform.position, force * 0.5f, knockBackDuration * 0.5f);

                SpawnParryEffect();
                roninSFX.playBlock();
                return false;

            }
            // If motivation is > 0 but currently cannot parry, reduce 60% damage and 60% knockback.
            else if(curMotivation > 0)
            {
                int reducedDamage = Mathf.RoundToInt(damage * 0.4f);
                TakeHitKnockback(reducedDamage, target, force * 0.4f, knockBackDuration * 0.4f);
                return true;
            }
            // If motivation <= 0 (stunned), take 50% more damage 
            else
            {
                int increasedDamage = Mathf.RoundToInt(damage * 1.5f);
                TakeHitKnockback(increasedDamage, target, force, knockBackDuration);
                return true;
            }
        }
        return false;
    }

    public override Boolean TakeRangedHit(int damage, Vector3 target, float force, float knockBackDuration, BulletScript bullet)
    {
        if (canDamage)
        {
            // If ronin is in the starting idle (not attacking at all) and takes a hit, he will start attacking now.
            if (roninAction.currentAction == RoninAction.RoninActions.STARTING_IDLE)
            {
                EnterBattle();
                //Debug.Log("Ronin is starting to attack player.");
                //Debug.Log("Ronin's current action: " + roninAction.currentAction);
            }

            if (curAbsoluteDefenseStacks > 0 && canParry)
            {
                // Activating superParry will immediately end ParryState.
                if (parryStateTimer > 0)
                {
                    parryStateTimer = 0;
                }
                roninAction.EnterAbsoluteDefense();
                curAbsoluteDefenseStacks--;
                resolve++;
                if (resolve >= 2)
                {
                    resolve = 0;
                    curRetributionStacks++;
                }
                curRetributionStacks = Mathf.Clamp(curRetributionStacks, 0, maxRetributionStacks);
                GameObject instance = Instantiate(deflectedBullet, transform.position, Quaternion.identity);
                DeflectedBulletScript enemyBullet = instance.GetComponent<DeflectedBulletScript>();
                enemyBullet.Init(Utils.DirectionTo(transform, player), bullet.force, bullet.damage, bullet.knockbackForce, bullet.knockbackDuration);

                bullet.DestroyBulletInstant();
                roninSFX.playDeflectRanged();
                return false;
            }
            else if (curMotivation > 0 && canParry)
            {
                if (!isInParryState)
                {
                    parryStateTimer = parryStateDuration;
                    isInParryState = true;
                    Debug.Log("Entering Parry state.");
                    roninAction.EnterParryState();
                }
                // If Ronin is in Parry State (the parryStateDuration is > 0) start accumulating Resilience.
                // Ranged Hits accumulate resilience 50% slower
                if (parryStateTimer > 0)
                {
                    resilience = resilience+0.5f;
                    parryStateTimer = parryStateDuration;
                    if (resilience >= resilienceStacksToAbsoluteDefense)
                    {
                        resilience = 0;
                        curAbsoluteDefenseStacks++;
                        curAbsoluteDefenseStacks = Mathf.Clamp(curAbsoluteDefenseStacks, 0, maxAbsoluteDefenseStacks);
                    }
                }

                SpawnParryEffect();

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
                return false;
            }
            // If motivation is > 0 but currently cannot parry, reduce 80% damage and 80% knockback.
            else if (curMotivation > 0)
            {
                int reducedDamage = Mathf.RoundToInt(damage * 0.2f);
                TakeHitKnockback(reducedDamage, target, force * 0.2f, knockBackDuration * 0.2f);
                bullet.DestroyBullet();
                return true;
            }
            // If motivation <= 0, take 50% more damage
            else
            {
                int increasedDamage = Mathf.RoundToInt(damage * 1.5f);
                TakeHitKnockback(increasedDamage, target, force, knockBackDuration);
                bullet.DestroyBullet();
                return true;
            }
        }
        return false;
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

    private void RegisterPlayer(GameObject player)
    {
        this.player = player;
        Debug.Log("Ronin manager: registered player.");
    }
    private void SpawnAbsoluteDefenseParryEffect()
    {
        StartCoroutine(SpawnAbsoluteDefenseParryEffectCoroutine());
    }
    private IEnumerator SpawnAbsoluteDefenseParryEffectCoroutine()
    {

        Vector3 playerPos = player.transform.position;
        Vector3 enemyPos = transform.position;
        Vector3 middlePosition = (playerPos + enemyPos) / 2f;

        GameObject effect = Instantiate(parryEffectPrefab, middlePosition, Quaternion.identity);
        SpriteRenderer sr = effect.GetComponent<SpriteRenderer>();
        sr.color = new Color32(255, 251, 225, 255);

        yield return new WaitForSeconds(2f);

        Destroy(effect);
    }


    private void SpawnParryEffect()
    {
        StartCoroutine(SpawnParryEffectCoroutine());
    }
    private IEnumerator SpawnParryEffectCoroutine()
    {
        GameObject effect = Instantiate(parryEffectPrefab, parryEffectTransform.transform.position, Quaternion.identity, parryEffectTransform.transform);

        effect.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        yield return new WaitForSeconds(2f);

        Destroy(effect);
    }

    private void EnterBattle()
    {
        roninLevelManager.closeEntranceGate();
        roninAction.currentAction = RoninAction.RoninActions.IDLE;
        roninBGM.playBattleBGM();
        roninCanvasScript.showCanvas();
    }

    private void EnterDeath()
    {
        // OPEN GATE
        roninLevelManager.openExitGate();
        roninLevelManager.openEntranceGate();
        // CHANGE BGM
        RoninBGM roninBGM = GetComponent<RoninBGM>();
        if (roninBGM != null)
        {
            roninBGM.playBattleEndBGM();
        }
        else Debug.Log("Cannot find roninBGM.");
        // DISABLE RONIN'S HEALTH / STAMINA UI
        GameObject roninCanvasInstance = roninManager.roninCanvasInstance;
        if (roninCanvasInstance != null)
        {
            roninCanvasInstance.SetActive(false);
        }
        else Debug.Log("RoninAction: Cannot find roninCanvasInstance");

        roninAction.EnterDeath();
    }

    private IEnumerator SpawnSingleAttackZoneTest()
    {
        GameObject attackInstance = Instantiate(singleSlashAttackZone, player.transform.position, Quaternion.identity);
        attackInstance.transform.localScale = new Vector3(1f, 5f, 1f);
        yield return new WaitForSeconds(2);
        SingleSlashAttackZone attackZoneScript = attackInstance.GetComponent<SingleSlashAttackZone>();
        if (attackZoneScript != null)
        {
            attackZoneScript.EnableDamage();
            yield return new WaitForSeconds(0.5f);
            attackZoneScript.DisableDamage();
            Destroy(attackInstance);
        }
        else
        {
            Debug.Log("spawnSingleAttackZoneTest: attackZoneScript not found!");
        }
            yield return null;
    }

}
