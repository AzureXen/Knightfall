using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class HolySmiteHitbox : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private float knockbackForce;
    [SerializeField] float knockbackDuration;
    private PlayerManager player;

    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();
    private void OnEnable()
    {
        hitEnemies.Clear();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
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
