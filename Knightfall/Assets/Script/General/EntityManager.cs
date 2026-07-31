using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public bool isKnockbackResist = false;
    public bool isAlive = true;
    public String EntityName { get; set; }

    public Health entityHealth;
    protected EntityMovement entityMovement;

    protected SpriteRenderer sr;
    protected Rigidbody2D rb;

    protected float flashDuration = 1.5f;
    protected Coroutine flashCoroutine;

    public float takeDamageCooldown = 0.2f;
    protected Coroutine IFrameCoroutine;
    public Boolean canDamage = true;

    public float knockbackDuration = 0.5f;
    protected float knockbackTimer = 0f;
    public Boolean isKnockingBack;
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

        if (entityHealth != null)
        {
            entityHealth.OnDeath += HandleDeathEvent;
        }
    }

    public virtual Boolean TakeRangedHit(int damage, Vector3 target, float force, float knockBackDuration, BulletScript bullet)
    {
        if (isAlive && canDamage)
        {
            TakeHitKnockback(damage, target, force, knockBackDuration);
            bullet.DestroyBullet();
            return true;
        }
        return false;
    }
    public virtual void TakeRangedHit(int damage, Vector3 target, float force, float knockBackDuration)
    {
        if (isAlive && canDamage)
        {
            TakeHitKnockback(damage, target, force, knockBackDuration);
        }
    }

    // target: the target who dealt this knock back to this entity
    // source: the manager of the entity that dealt this knock back to this entity
    // why? damage reflection. there maybe times where if we deal damage to a certain entity at a certain situation, they will reflect that damage back.
    // optional. in this code, you can see that the EntityManager is unused. but you must specify the entitymanager source every time, in case a certain entity requires it.
    public virtual Boolean TakeMeleeHit(int damage, Vector3 target, float force, float knockBackDuration, EntityManager source)
    {
        if (isAlive && canDamage)
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
        else Debug.Log(EntityName +": knock back force is less than 0: " + finalKnockbackForce);
    }
    public virtual Boolean TakeHit(int damage)
    {
        if (isAlive && canDamage)
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
    //        Vector3 direction = (transform.position - target).normalized;
    //        rb.AddForce(direction * force, ForceMode2D.Impulse);
    //        yield return new WaitForSeconds(duration);
    //        rb.linearVelocity = Vector2.zero; // Stop movement after duration
    //    }

    //    entityMovement.EnableMovement();
    //}




    // THIS KNOCK BACK USES ADD FORCE
    //protected IEnumerator KnockBack(Vector3 target, float force, float duration)
    //{
    //    entityMovement.DisableMovement();
    //    yield return null;

    //    if (rb != null)
    //    {
    //        rb.linearVelocity = Vector2.zero; // Clear momentum

    //        // 1. Calculate the raw distance vector from the attack point (target) to this entity
    //        Vector3 directionToFly = transform.position - target;


    //        if (directionToFly.magnitude < 0.05f)
    //        {
    //            float defaultX = (transform.position.x >= target.x) ? 1f : -1f;
    //            directionToFly = new Vector3(defaultX, 0.2f, 0f);
    //        }

    //        // 2. Safely normalize now that we guaranteed it's not zero
    //        Vector2 finalDirection = ((Vector2)directionToFly).normalized;

    //        // 3. Launch away from the attack origin point!
    //        rb.AddForce(finalDirection * force, ForceMode2D.Impulse);

    //        yield return new WaitForSeconds(duration);

    //        rb.linearVelocity = Vector2.zero;
    //    }

    //    entityMovement.EnableMovement();
    //}


    // THIS KNOCK BACK USES LINEAR.VELOCITY
    protected IEnumerator KnockBack(Vector3 target, float force, float duration)
    {
        if (!isKnockbackResist)
        {
            isKnockingBack = true;
            entityMovement.DisableMovement();
            yield return null;
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;

                Vector3 directionToFly = transform.position - target;
                if (directionToFly.magnitude < 0.05f)
                {
                    float defaultX = (transform.position.x >= target.x) ? 1f : -1f;
                    directionToFly = new Vector3(defaultX, 0.2f, 0f);
                }

                Vector2 finalDirection = ((Vector2)directionToFly).normalized;

                float elapsed = 0f;
                while (elapsed < duration)
                {
                    rb.linearVelocity = finalDirection * force;
                    elapsed += Time.deltaTime;
                    yield return null;
                }

                rb.linearVelocity = Vector2.zero;
            }
            entityMovement.EnableMovement();
            isKnockingBack = false;
        }
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

    protected virtual void HandleDeathEvent()
    {
        Die();
        StartCoroutine(DelayedDisappear());
    }

    protected virtual void Die()
    {
        isAlive = false;
    }

    protected virtual IEnumerator DelayedDisappear()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
}
