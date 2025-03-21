using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
public class SwordHitbox : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private float knockbackForce;
    [SerializeField] float knockbackDuration;
    [SerializeField] private float hitboxDelay;
    private PlayerManager playerManager;
    private Collider2D hitboxCollider;
    public GameObject player;
    private PlayerSFX playerSFX;

    private HashSet<GameObject> hitEnemies = new HashSet<GameObject> ();
    private void OnEnable()
    {
        hitEnemies.Clear ();
        playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();

        hitboxCollider = GetComponent<Collider2D>();
        if (hitboxCollider != null)
        {
            hitboxCollider.enabled = false;
            StartCoroutine(EnableColliderAfterDelay(hitboxDelay)); // Delay to sync with animation
        }
    }

    // Coroutine to enable collider after delay
    private IEnumerator EnableColliderAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        hitboxCollider.enabled = true;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        damage = transform.parent.parent.GetComponent<PlayerSword>().meleeDamage;
        knockbackForce = transform.parent.parent.GetComponent<PlayerSword>().knockbackForce;
        knockbackDuration = transform.parent.parent.GetComponent<PlayerSword>().knockbackDuratiton;
        playerSFX = player.GetComponent<PlayerSFX>();
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !hitEnemies.Contains(collision.gameObject))
        {
            EntityManager target = collision.GetComponent<EntityManager>();
            Boolean hitSuccess = target.TakeMeleeHit(damage, transform.position, knockbackForce, knockbackDuration, playerManager);
            if (hitSuccess)
            {
                hitEnemies.Add(collision.gameObject);
                playerSFX.playMeleeHit();
            }
        }
    }
}
