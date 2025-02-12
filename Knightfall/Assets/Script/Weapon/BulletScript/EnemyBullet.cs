using System;
using UnityEngine;

public class EnemyBullet : BulletScript
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collided with tag: " + collision.gameObject.tag);
        if (collision.gameObject.tag == "Player")
        {
            PlayerManager playerManager = collision.gameObject.GetComponent<PlayerManager>();
            if (playerManager != null)
            {
                Boolean hitSuccess = playerManager.TakeHit(damage);
                if (hitSuccess)
                {
                    Destroy(gameObject);
                }
            }
        }
        else
        {
            Debug.Log("Hm, the bullet might've gone through something.");
        }
    }
}
