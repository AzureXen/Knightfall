using System;
using System.Collections;
using UnityEngine;

public class EnemyBullet : BulletScript
{
    protected Vector2 direction;

    protected override void Start()
    {
        Destroy(gameObject, despawnTime);
    }

    public void Init(Vector2 direction, float speed, int damage, float knockbackForce, float knockbackDuration)
    {
        SetDirection(direction);
        force = speed;
        this.damage = damage;
        this.knockbackForce = knockbackForce;
        this.knockbackDuration = knockbackDuration;
    }

    public virtual void SetDirection(Vector2 dir)
    {
        direction = dir;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void Update()
    {
        transform.position += (Vector3)direction * force * Time.deltaTime;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (!disabledDamage && collision.CompareTag("Player"))
        {
            EntityManager player = collision.GetComponent<EntityManager>();
            if (player != null)
            {
                if(player.TakeRangedHit(damage, transform.position, knockbackForce, knockbackDuration, this)){
                    Destroy(gameObject);
                }
            }
        }
    }
}
