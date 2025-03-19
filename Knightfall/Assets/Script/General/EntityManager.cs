using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public String EntityName { get; set; }

    protected Health entityHealth;
    protected EntityMovement entityMovement;

    protected SpriteRenderer sr;
    protected Rigidbody2D rb;

    protected float flashDuration = 1.5f;
    protected Coroutine flashCoroutine;

    public float takeDamageCooldown = 0.2f;
    protected Coroutine IFrameCoroutine;
    protected Boolean canDamage = true;

    float knockbackDuration = 2f;
    protected float knockbackTimer = 0f;
    protected Boolean isKnockingBack;
    protected Vector3 knockbackDirection;
    protected float knockbackForce;

    public Color defaultColor;
    public float knockbackResist = 0f;
    public virtual void Start()
    {
        entityHealth = GetComponent<Health>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        entityMovement = GetComponent<EntityMovement>();
        isKnockingBack = false;

        EntityName = "EntityNameNotSet";
        defaultColor = Color.white;
    }

    //protected virtual void FixedUpdate()
    //{
    //    if(isKnockingBack)
    //    {
    //        rb.linearVelocity = new Vector2(knockbackDirection.x * knockbackForce, knockbackDirection.y * knockbackForce);
    //        //rb.AddForce(new Vector2(knockbackDirection.x * knockbackForce, knockbackDirection.y * knockbackForce));
    //        knockbackTimer += Time.deltaTime;
    //        Debug.Log("Knocking back.");
    //        if(knockbackTimer >= knockbackDuration)
    //        {
    //            isKnockingBack=false;
    //        }
    //    }
    //}



    public virtual void TakeRangedHit(int damage, Vector3 target, float force, float knockBackDuration, BulletScript bullet)
    {
        if (canDamage)
        {
            TakeHitKnockback(damage, target, force, knockBackDuration);
            bullet.DestroyBullet();
        }
    }
    public virtual void TakeRangedHit(int damage, Vector3 target, float force, float knockBackDuration)
    {
        if (canDamage)
        {
            TakeHitKnockback(damage, target, force, knockBackDuration);
        }
    }
    public virtual Boolean TakeMeleeHit(int damage, Vector3 target, float force, float knockBackDuration, EntityManager source)
    {
        if (canDamage)
        {
            TakeHitKnockback(damage, target, force, knockBackDuration);
            return true;
        }
        return false;
    }

    public virtual void TakeHitKnockback(int damage, Vector3 target, float force, float duration)
    {
        TakeDamage(damage);
        float finalKnockbackForce = force - knockbackResist;
        if (finalKnockbackForce > 0)
        {
            StartCoroutine(KnockBack(target, finalKnockbackForce, duration));
        }
    }
    public virtual Boolean TakeHit(int damage)
    {
        if (canDamage)
        {
            TakeDamage(damage);
            return true;
        }
        return false;
    }

    protected virtual void TakeDamage(int damage)
    {
        entityHealth.TakeDamage(damage);
        if(damage>0)
        {
            if (flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine);
            }
            flashCoroutine = StartCoroutine(FlashRed());
            TakeIFrame(takeDamageCooldown);
        }
    }

    public void TakeIFrame(float duration)
    {
        if (IFrameCoroutine != null)
        {
            StopCoroutine(IFrameCoroutine);
        }
        IFrameCoroutine = StartCoroutine(IFrame(duration));
    }
    public void TakeIFrameNoCollision(float duration)
    {
        if (IFrameCoroutine != null)
        {
            StopCoroutine(IFrameCoroutine);
        }
        IFrameCoroutine = StartCoroutine(IFrameNoCollision(duration));
    }
    protected virtual IEnumerator IFrameNoCollision(float duration)
    {
        try
        {
            canDamage = false;
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);
            yield return new WaitForSeconds(duration);
        }
        finally
        {
            canDamage = true;
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
            IFrameCoroutine = null;
        }
    }
    protected virtual IEnumerator IFrame(float duration)
    {
        try
        {
            canDamage = false;
            yield return new WaitForSeconds(duration);
        }
        finally
        {
            canDamage = true;
            IFrameCoroutine = null;
        }
    }
    public void TakeKnockBack(Vector3 target, float force, float duration)
    {
        float finalKnockbackForce = force - knockbackResist;
        if (finalKnockbackForce > 0)
        {
            StartCoroutine(KnockBack(target, finalKnockbackForce, duration));
        }
    }
    //protected IEnumerator KnockBack(Vector3 target, float force, float duration)
    //{
    //    entityMovement.DisableMovement();
    //    yield return null;
    //    if (rb != null)
    //    {

    //    }
    //    Vector3 direction = (transform.position - target).normalized;
    //    knockbackDirection = direction;
    //    knockbackForce = force;
    //    knockbackDuration = duration;
    //    isKnockingBack = true;
    //    while (isKnockingBack)
    //    {
    //        yield return null;
    //    }
    //    knockbackTimer = 0;
    //    rb.linearVelocity = Vector2.zero;
    //    yield return null;
    //    entityMovement.EnableMovement();
    //}
    protected IEnumerator KnockBack(Vector3 target, float force, float duration)
    {
        entityMovement.DisableMovement();
        yield return null;

        if (rb != null)
        {
            Vector3 direction = (transform.position - target).normalized;
            rb.AddForce(direction * force, ForceMode2D.Impulse);
            yield return new WaitForSeconds(duration);
            rb.linearVelocity = Vector2.zero; // Stop movement after duration
        }

        entityMovement.EnableMovement();
    }

    protected virtual IEnumerator FlashRed()
    {
        sr.color = new Color(180f / 255f, 0f / 255f, 0f / 255f);
        yield return null;

        float timer = 0f;
        while (timer < flashDuration)
        {
            timer += Time.deltaTime;
            sr.color = Color.Lerp(sr.color, defaultColor, timer / flashDuration);
            yield return null;
        }

        sr.color = defaultColor;
        flashCoroutine = null;
    }
}
