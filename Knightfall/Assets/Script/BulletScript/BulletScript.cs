using System;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    protected Vector3 mousePos;
    protected Camera mainCam;
    protected Rigidbody2D rb;
    public float force = 0;
    public float despawnTime = 10f;
    public int damage = 10;
    public float knockbackForce = 0f;
    public float knockbackDuration = 1.5f;

    public bool disabledDamage = false;
    protected virtual void Start()
    {
        Destroy(gameObject, despawnTime);
    }


    // DEFAULT BULLET SCRIPT WILL DAMAGE ENEMIES
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!disabledDamage && collision.CompareTag("Enemy"))
        {
            EntityManager enemyManager = collision.gameObject.GetComponent<EntityManager>();
            if (enemyManager != null)
            {
                enemyManager.TakeRangedHit(damage, transform.position, knockbackForce, knockbackDuration, this);
            }
        }
    }
    // For DestroyBullet, you may implement your custom bullet destruction after collision
    public virtual void DestroyBullet()
    {
        disabledDamage = true;
        Destroy(gameObject);
    }

    // But for DestroyBulletInstant, you may NOT implement any custom bullet destruction, for its purpose is to instantly delete the bullet from the game.
    public virtual void DestroyBulletInstant()
    {
        disabledDamage = true;
        Destroy(gameObject);
    }
    // For BreakBullet, you may implement your custom bullet break after getting Parried or whatever
    public virtual void BreakBullet()
    {
        disabledDamage = true;
        Destroy(gameObject);
    }
}
