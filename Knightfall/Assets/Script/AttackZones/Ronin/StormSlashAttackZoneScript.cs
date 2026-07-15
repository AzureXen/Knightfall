using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class StormSlashAttackZoneScript : AttackZoneScript
{

    public RoninSFX roninSFX;
    [SerializeField] GameObject attackHitParticle;

    [SerializeField] private GameObject attackVisual;

    private GameObject attackVisualInstance;

    public float attackVisualSize = 1f;
    protected override void Update()
    {
        if(!showHitbox)
        {
            sr.color = new Color(0,0,0,0);
        }
        else sr.color = originalColor;
        if (hitBoxActive && showHitbox)
        {
            attackShow.SetActive(true);
        }
        else attackShow.SetActive(false);

        if(playerIsInHitbox && !damagedPlayer && hitBoxActive)
        {
            Boolean successHit = playerManager.TakeMeleeHit(damage, damageSourcePosition, knockbackForce, knockbackDuration, null);
            if (successHit)
            {
                damagedPlayer = true;
                if (attackHitParticle != null)
                {
                    GameObject hitEffect = Instantiate(attackHitParticle, player.transform.position, Quaternion.identity, player.transform);
                    hitEffect.transform.localPosition = Vector3.zero;
                    hitEffect.transform.localScale = new Vector3(1f, 1f, 1f);
                    Destroy(hitEffect, 1f);
                }
                if (roninSFX != null)
                {
                    roninSFX.playFlashSlashHit();
                }
                else
                {
                    Debug.LogWarning("roninSFX has not been assigned.");
                }
            }
        }
    }

    public override void EnableDamage()
    {
        hitBoxActive = true;
        if(attackVisualInstance != null)
        {
            Destroy(attackVisualInstance);
            attackVisualInstance = null;
        }
        attackVisualInstance = Instantiate(attackVisual, transform.position, Quaternion.identity, transform);
        attackVisualInstance.transform.localScale = new Vector3(attackVisualSize,attackVisualSize,attackVisualSize);
        Destroy(attackVisualInstance, 2f);
        StartCoroutine(DestroyAndNullVisualAfterDelay(attackVisualInstance, 2f));
    }
    public override void DisableDamage()
    {
        hitBoxActive = false;
        damagedPlayer = false;

        Destroy(attackVisualInstance);
        attackVisualInstance = null;
    }
    protected override void OnDestroy()
    {
        playerIsInHitbox = false;
        damagedPlayer = false;

        Destroy(attackVisualInstance);
        attackVisualInstance = null;
    }
    private IEnumerator DestroyAndNullVisualAfterDelay(GameObject instanceToDestroy, float delay)
    {
        yield return new WaitForSeconds(delay);

        // Safety check: Make sure it wasn't already destroyed manually by another action
        if (instanceToDestroy != null)
        {
            Destroy(instanceToDestroy);

            // Only set it to null if it's still pointing to the active instance we spawned
            if (attackVisualInstance == instanceToDestroy)
            {
                attackVisualInstance = null;
            }
        }
    }
}
