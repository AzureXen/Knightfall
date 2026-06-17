using UnityEngine;
using System.Collections.Generic;

public class SwordHitbox : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private float knockbackForce;
    [SerializeField] private float knockbackDuration;
    [SerializeField] private float hitboxDelay; 

    public GameObject player;

    private PlayerManager playerManager;
    private Collider2D hitboxCollider;
    private PlayerSFX playerSFX;
    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();

    private void Awake()
    {
        hitboxCollider = GetComponent<Collider2D>();
        playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        playerSFX = player.GetComponent<PlayerSFX>();
    }

    void Start()
    {
        var playerSword = transform.parent.parent.GetComponent<PlayerSword>();
        if (playerSword != null)
        {
            damage = playerSword.meleeDamage;
            knockbackForce = playerSword.knockbackForce;
            knockbackDuration = playerSword.knockbackDuration; 
        }
    }

    private void OnEnable()
    {
        hitEnemies.Clear();
        if (hitboxCollider != null)
        {
            hitboxCollider.enabled = true;
        }
    }

    private void OnDisable()
    {
        if (hitboxCollider != null)
        {
            hitboxCollider.enabled = false;
        }
        hitEnemies.Clear(); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !hitEnemies.Contains(collision.gameObject))
        {
            EntityManager target = collision.GetComponent<EntityManager>();
            if (target != null)
            {
                bool hitSuccess = target.TakeMeleeHit(damage, transform.position, knockbackForce, knockbackDuration, playerManager);
                if (hitSuccess)
                {
                    hitEnemies.Add(collision.gameObject);
                    if (playerSFX != null) playerSFX.playMeleeHit();
                }
            }
        }
    }
}