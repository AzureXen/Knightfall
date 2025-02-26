using System;
using System.Collections;
using UnityEngine;


public class RoninManager : EntityManager
{
    private RoninFlashSlash flashSlash;

    [SerializeField] private float maxMotivation;
    [SerializeField] private float curMotivation;
    [SerializeField] private float motivationRegenRate;
    [SerializeField] private int maxSuperParry;
    [SerializeField] private int superParry;
    [SerializeField] private float stunDuration;    

    private Boolean isStunned = false;

    public AudioSource audioSource;
    public AudioClip[] audioClips;

    private RoninAction roninAction;
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
        superParry = 1;
        flashSlash = GetComponent<RoninFlashSlash>();
        roninAction = GetComponent<RoninAction>();
    }

    private void Update()
    {
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

    public override void TakeMeleeHit(int damage, Vector3 target, float force, float knockBackDuration, EntityManager source)
    {
        if (canDamage)
        {
            // If ronin is in the starting idle (not attacking at all) and takes a hit, he will start attacking now.
            if(roninAction.currentAction == RoninAction.RoninActions.STARTING_IDLE)
            {
                roninAction.currentAction = RoninAction.RoninActions.IDLE;
                Debug.Log("Ronin is starting to attack player.");
                Debug.Log("Ronin's current action: " + roninAction.currentAction);
            }
            if(superParry > 0)
            {
                // super parry does NOT consume motivation.
                TakeHitKnockback(0, target, 0, 0);
                source.TakeKnockBack(transform.position, 15, 0.4f);
                superParry--;
                audioSource.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
                if (audioClips.Length > 0)
                {
                    int random = UnityEngine.Random.Range(0, audioClips.Length);
                    audioSource.clip = audioClips[random];
                    audioSource.Play();
                }
                else
                {
                    Debug.LogWarning("Audio for ParryHit not found.");
                }

            }
            else if(curMotivation > 0)
            {
                curMotivation -= damage;
                TakeHitKnockback(0, target, force * 0.25f, knockBackDuration * 0.25f);
                source.TakeKnockBack(transform.position, force * 0.75f, knockBackDuration * 0.75f);
                audioSource.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
                if (audioClips.Length > 0)
                {
                    int random = UnityEngine.Random.Range(0, audioClips.Length);
                    audioSource.clip = audioClips[random];
                    audioSource.Play();
                }
                else
                {
                    Debug.LogWarning("Audio for ParryHit not found.");
                }
            }
            else
            {
                TakeHitKnockback(damage, target, force, knockBackDuration);
            }
        }
    }

    public override void TakeRangedHit(int damage, Vector3 target, float force, float knockBackDuration, BulletScript bullet)
    {
        if (canDamage)
        {
            // If ronin is in the starting idle (not attacking at all) and takes a hit, he will start attacking now.
            if (roninAction.currentAction == RoninAction.RoninActions.STARTING_IDLE)
            {
                roninAction.currentAction = RoninAction.RoninActions.IDLE;
                Debug.Log("Ronin is starting to attack player.");
                Debug.Log("Ronin's current action: " + roninAction.currentAction);
            }
            if (curMotivation > 0)
            {
                TakeHitKnockback(0, target, 0, 0);
                audioSource.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
                if (audioClips.Length > 0)
                {
                    int random = UnityEngine.Random.Range(0, audioClips.Length);
                    audioSource.clip = audioClips[random];
                    audioSource.Play();
                }
                else
                {
                    Debug.LogWarning("Audio for ParryHit not found.");
                }
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
        superParry++;
        superParry = Mathf.Clamp(superParry, 0, maxSuperParry);
    }

}
