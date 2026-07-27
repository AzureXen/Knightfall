using System;
using UnityEngine;

public class SingleSlashAttackZone : AttackZoneScript
{
    public RoninSFX roninSFX;
    [SerializeField] GameObject attackHitParticle;
    [SerializeField] GameObject visualPrefab;
    private GameObject visualInstance;
    protected override void Start()
    {
        playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        damageSourcePosition = -(transform.position + transform.up * (transform.localScale.y));
        sr = GetComponentInChildren<SpriteRenderer>();
        originalColor = sr.color;
        player = GameManagerScript.Instance.player;
        if (player == null) GameManagerScript.OnPlayerReady += RegisterPlayer;
        else playerManager = player.GetComponent<PlayerManager>();
    }
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
                hitEffect.transform.localScale = new Vector3(1f, 1f, 1f);
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

    public override void EnableDamage()
    {
        hitBoxActive = true;
        if (visualInstance != null)
        {
            Destroy(visualInstance);
        }
        visualInstance = Instantiate(visualPrefab, transform.position, Quaternion.identity, transform);
        Vector3 transformScale = transform.localScale;
        visualInstance.transform.localScale = new Vector3(transformScale.x, transformScale.y * 0.25f, transformScale.z);
    }
    public override void DisableDamage()
    {
        hitBoxActive = false;
        damagedPlayer = false;
        Destroy(visualInstance);
        visualInstance = null;
    }

    protected override void OnDestroy()
    {
        playerIsInHitbox = false;
        damagedPlayer = false;
        if (visualInstance != null)
        {
            Destroy(visualInstance);
        }
    }
}
