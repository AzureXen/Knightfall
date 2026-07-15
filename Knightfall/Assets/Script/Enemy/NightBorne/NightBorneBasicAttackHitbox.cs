using System;
using UnityEngine;

public class NightBorneBasicAttackHitbox : MonoBehaviour
{
    public Vector3 damageSourcePosition = Vector3.zero;

    [Header("Visual Debugging")]
    public bool showHitbox = true;

    [SerializeField] private bool hitBoxActive = false;
    [SerializeField] private bool playerIsInHitbox = false;
    private bool damagedPlayer = false;

    private SpriteRenderer sr;
    private PlayerManager playerManager;


    private int damage = 10;
    private float knockbackForce = 2f;
    private float knockbackDuration = 0.2f;

    private void Start()
    {
        GameObject player = GameManagerScript.Instance.player;
        if (player == null)
        {
            GameManagerScript.OnPlayerReady += RegisterPlayer;
        }else playerManager = player.GetComponent<PlayerManager>();

        sr = GetComponent<SpriteRenderer>();

        NightBorneAttack nba = GetComponentInParent<NightBorneAttack>();

        damage = nba.damage;
        knockbackForce = nba.knockbackForce;
        knockbackDuration = nba.knockbackDuration;
    }

    private void Update()
    {
        if (sr != null)
        {
            if (showHitbox)
            {
                if (hitBoxActive)
                {
                    sr.color = new Color(200, 0, 0, 0.1f);
                }
                else sr.color = new Color(255, 255, 255, 0.3f);
            }
            else
            {
                sr.color = new Color(0, 0, 0, 0);
            }
        }

        damageSourcePosition = transform.position;

        if (playerIsInHitbox && !damagedPlayer && hitBoxActive && playerManager != null)
        {
            bool successHit = playerManager.TakeMeleeHit(damage, damageSourcePosition, knockbackForce, knockbackDuration, null);

            if (successHit)
            {
                damagedPlayer = true;
            }
        }
    }
    private void RegisterPlayer(GameObject player)
    {
        this.playerManager = player.GetComponent<PlayerManager>();
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

    private void OnDisable()
    {
        playerIsInHitbox = false;
        damagedPlayer = false;
    }
}