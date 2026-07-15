using System;
using Unity.VisualScripting;
using UnityEngine;

public class AttackZoneScript : MonoBehaviour
{
    protected Boolean hitBoxActive = false;
    public int damage = 5;
    public Vector3 damageSourcePosition = Vector3.zero;
    public float knockbackForce = 1f;
    public float knockbackDuration = 1f;

    public Boolean showHitbox = true;
    public GameObject attackShow;

    protected Boolean playerIsInHitbox = false;

    // Only damage player once.
    protected Boolean damagedPlayer = false;

    protected Color originalColor;
    protected SpriteRenderer sr;

    protected GameObject player;
    protected PlayerManager playerManager;
    protected virtual void Start()
    {
        playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        damageSourcePosition = -(transform.position + transform.up * (transform.localScale.y));
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
        player = GameManagerScript.Instance.player;
        if (player == null) GameManagerScript.OnPlayerReady += RegisterPlayer;
        else playerManager = player.GetComponent<PlayerManager>();
    }
    protected virtual void Update()
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
            }
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsInHitbox = true;
        }
    }
    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsInHitbox = false;
        }
    }
    public virtual void EnableDamage()
    {
        hitBoxActive = true;
    }
    public virtual void DisableDamage()
    {
        hitBoxActive = false;
        damagedPlayer = false;
    }
    protected virtual void OnDestroy()
    {
        playerIsInHitbox = false;
        damagedPlayer = false;
    }

    private void RegisterPlayer(GameObject player)
    {
        this.player = player;
        playerManager = player.GetComponent<PlayerManager>();
        if (playerManager == null)
        {
            Debug.LogError("AttackZoneScript: playermanager component not found!");
        }
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
