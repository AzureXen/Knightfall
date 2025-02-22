using System;
using Unity.VisualScripting;
using UnityEngine;

public class AttackZoneScript : MonoBehaviour
{
    private Boolean hitBoxActive = false;
    public int damage = 5;
    public Vector3 damageSourcePosition = Vector3.zero;
    public float knockbackForce = 1f;
    public float knockbackDuration = 1f;

    public Boolean showHitbox = true;
    public GameObject attackShow;

    private Boolean playerIsInHitbox = false;

    // Only damage player once.
    private Boolean damagedPlayer = false;

    private PlayerManager playerManager;
    private void Start()
    {
        playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        damageSourcePosition = -(transform.position + transform.up * (transform.localScale.y));
    }
    private void Update()
    {
        if (hitBoxActive && showHitbox)
        {
            attackShow.SetActive(true);
        }
        else attackShow.SetActive(false);

        if(playerIsInHitbox && !damagedPlayer && hitBoxActive)
        {
            playerManager.TakeMeleeHit(damage, damageSourcePosition, knockbackForce, knockbackDuration, null);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsInHitbox = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsInHitbox = false;
        }
    }
    public void EnableDamage()
    {
        hitBoxActive = true;
    }
    public void DisableDamage()
    {
        hitBoxActive = false;
        damagedPlayer = false;
    }
    private void OnDestroy()
    {
        playerIsInHitbox = false;
        damagedPlayer = false;
    }

    // On Trigger Stay only execute if it detects movement, so right now, if player stands still, they wont take damage
    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    if (hitBoxActive)
    //    {
    //        if (collision.CompareTag("Player"))
    //        {
    //            EntityManager entityManager = collision.gameObject.GetComponent<EntityManager>();

    //            if (entityManager != null)
    //            {
    //                entityManager.TakeMeleeHit(damage, damageSourcePosition, knockbackForce, knockbackDuration, null);
    //            }
    //        }
    //    }
    //}


}
