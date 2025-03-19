using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class SwordHitbox : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private float knockbackForce;
    [SerializeField] float knockbackDuration;
    [SerializeField] private float hitboxDelay;
    private PlayerManager player;
    private Collider2D hitboxCollider;

    private HashSet<GameObject> hitEnemies = new HashSet<GameObject> ();
    private void OnEnable()
    {
        hitEnemies.Clear ();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();

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
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !hitEnemies.Contains(collision.gameObject))
        {
            EntityManager target = collision.GetComponent<EntityManager>();
            target.TakeMeleeHit(damage, transform.position, knockbackForce, knockbackDuration, player);
            hitEnemies.Add(collision.gameObject);
        }
    }
}
