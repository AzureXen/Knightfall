using System;
using Unity.VisualScripting;
using UnityEngine;

public class AttackZoneScript : MonoBehaviour
{
    public Boolean hitBoxActive = false;
    public int damage = 5;
    public Vector3 damageSourcePosition = Vector3.zero;
    public float knockbackForce = 1f;
    public float knockbackDuration = 1f;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (hitBoxActive)
        {
            if (collision.CompareTag("Player"))
            {
                EntityManager entityManager = collision.gameObject.GetComponent<EntityManager>();
                
                if (entityManager != null)
                {
                    entityManager.TakeMeleeHit(damage, damageSourcePosition, knockbackForce, knockbackDuration, null);
                }
            }
        }
    }
}
