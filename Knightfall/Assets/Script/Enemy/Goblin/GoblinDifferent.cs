using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
/// <summary>
/// EnemyManager inherits from EntityManager
/// </summary>
public class GoblinDifferent : EntityManager
{
    [SerializeField] private Boolean touchDamageEnabled = true;
    public int touchDamage = 5;
    public int touchKnockbackForce = 5;
    public float touchKnockbackDuration = 1.5f;

    [SerializeField] private int maxParry = 5;
    [SerializeField] private int curParry;
    [SerializeField] private float parryCooldown = 5f;
    [SerializeField] private float parryCooldownTimer;
    [SerializeField] private Boolean isRegeningParry = false;

    public float parryDuration;
    private float parryTimer;
    [SerializeField] private Boolean isParrying;

    [SerializeField] private Boolean canParry = true;
    public AudioSource audioSource;
    public AudioClip[] audioClips;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        curParry = maxParry;
        isParrying = false;

        entityHealth = GetComponent<Health>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        entityMovement = GetComponent<GoblinMovement>();
        isKnockingBack = false;
        takeDamageCooldown = 0.2f;
        flashDuration = 2.5f;
        audioSource = GetComponent<AudioSource>();

        EntityName = "Goblin Different";
        defaultColor = new Color(210f / 255f, 7f / 255f, 255f / 255f);
    }

    private void Update()
    {
        if(curParry < maxParry && !isRegeningParry)
        {
            isRegeningParry = true;
            parryCooldownTimer = parryCooldown;
        }

        if (isRegeningParry)
        {
            if(parryCooldownTimer == 0)
            {
                curParry++;
                isRegeningParry = false;
            }
            else
            {
                parryCooldownTimer -= Time.deltaTime;
                parryCooldownTimer = Mathf.Clamp(parryCooldownTimer, 0, parryCooldown);
            }
        }

        if(parryTimer > 0)
        {
            parryTimer -= Time.deltaTime;
            parryTimer = Mathf.Clamp(parryTimer, 0, parryDuration);
            if(parryTimer == 0) isParrying = false;
        }
    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        GameObject target = collision.gameObject;
        if (target.CompareTag("Player") && touchDamageEnabled)
        {
            PlayerManager targetManager = target.GetComponent<PlayerManager>();
            targetManager.TakeMeleeHit(touchDamage, transform.position, touchKnockbackForce, touchKnockbackDuration, this);
        }
    }
    public override void TakeRangedHit(int damage, Vector3 target, float force, float knockBackDuration, BulletScript bullet)
    {
        if (canDamage)
        {
            if (isParrying)
            {
                TakeHitKnockback(0, target, force, knockBackDuration);
                parryTimer = parryDuration;
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
                if(curParry > 0)
                {
                    TakeHitKnockback(0, target, force, knockBackDuration);
                    parryTimer = parryDuration;
                    isParrying = true;
                    curParry--;
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
    }
    public override void TakeMeleeHit(int damage, Vector3 target, float force, float knockBackDuration, EntityManager source)
    {
        if (canDamage)
        {
            if (isParrying)
            {
                TakeHitKnockback(0, target, force * 0.5f, knockBackDuration * 0.5f);
                source.TakeKnockBack(transform.position, force * 0.5f, knockBackDuration * 0.5f);
                parryTimer = parryDuration;
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
                if (curParry > 0)
                {
                    TakeHitKnockback(0, target, force * 0.5f, knockBackDuration * 0.5f);
                    source.TakeKnockBack(transform.position, force * 0.5f, knockBackDuration * 0.5f);
                    parryTimer = parryDuration;
                    isParrying = true;
                    curParry--;
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
    }
    public override void TakeHitKnockback(int damage, Vector3 target, float force, float duration)
    {
        TakeDamage(damage);
        float finalKnockbackForce = (force - knockbackResist);
        if (finalKnockbackForce > 0)
        {
            StartCoroutine(KnockBack(target, finalKnockbackForce, duration));
        }
    }
    public override Boolean TakeHit(int damage)
    {
        if (canDamage)
        {
            if (canParry)
            {
                TakeDamage(0);
                return false;
            }
            else
            {
                TakeDamage(damage);
                return true;
            }
        }
        return false;
    }
}
