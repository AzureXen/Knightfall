using System;
using Unity.VisualScripting;
using UnityEngine;

public class AttackZoneScript : MonoBehaviour
{
    // For now, the "AttackZoneType" is purely for playing attack_hit_sfx accordingly.
    // If you want to add SFX to your attacks, its better NOT to use my way, its unnecessarily complicated.
    public enum AttackZoneType
    {
        None,
        FlashSlash,
    }
    // By default, the AttackZoneType is None. You can still use AttackZoneScript without declaring one.
    public AttackZoneType attackZoneType = AttackZoneType.None;

    public RoninSFX roninSFX;

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

    private Color originalColor;
    private SpriteRenderer sr;

    private PlayerManager playerManager;
    private void Start()
    {
        playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        damageSourcePosition = -(transform.position + transform.up * (transform.localScale.y));
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;

    }
    private void Update()
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
                if (attackZoneType == AttackZoneType.FlashSlash)
                {
                    if (roninSFX != null)
                    {
                        Debug.Log("AttackZoneScript: Playing FlashSlashHit");
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
