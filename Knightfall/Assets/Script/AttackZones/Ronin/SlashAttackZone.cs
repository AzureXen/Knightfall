using System;
using UnityEngine;

public class SlashAttackZone : AttackZoneScript
{
    public RoninSFX roninSFX;
    [SerializeField] GameObject attackHitParticle;
    protected override void Update()
    {
        if (!showHitbox)
        {
            sr.color = new Color(0, 0, 0, 0);
        }
        else sr.color = originalColor;
        if (hitBoxActive && showHitbox)
        {
            attackShow.SetActive(true);
        }
        else attackShow.SetActive(false);

        if (playerIsInHitbox && !damagedPlayer && hitBoxActive)
        {
            Boolean successHit = playerManager.TakeMeleeHit(damage, damageSourcePosition, knockbackForce, knockbackDuration, null);
            if (successHit)
            {
                damagedPlayer = true;
                GameObject hitEffect = Instantiate(attackHitParticle, player.transform.position, Quaternion.identity, player.transform);
                hitEffect.transform.localPosition = Vector3.zero;
                hitEffect.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                Destroy(hitEffect, 1f);
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
}
