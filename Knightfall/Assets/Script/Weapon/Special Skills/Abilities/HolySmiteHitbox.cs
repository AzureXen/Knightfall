using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HolySmiteHitbox : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private float knockbackForce;
    [SerializeField] private float knockbackDuration;
    [SerializeField] private float damageInterval = 0.3f; // Time between damage ticks

    private PlayerManager player;
    private HashSet<GameObject> activeEnemies = new HashSet<GameObject>(); // Track enemies inside
    private Dictionary<GameObject, Coroutine> damageCoroutines = new Dictionary<GameObject, Coroutine>(); // Track ongoing damage

    private void OnEnable()
    {
        activeEnemies.Clear();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !activeEnemies.Contains(collision.gameObject))
        {
            activeEnemies.Add(collision.gameObject);
            Coroutine damageCoroutine = StartCoroutine(DamageEnemyOverTime(collision.gameObject));
            damageCoroutines[collision.gameObject] = damageCoroutine;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && activeEnemies.Contains(collision.gameObject))
        {
            activeEnemies.Remove(collision.gameObject);

            // Stop damage coroutine if enemy leaves
            if (damageCoroutines.TryGetValue(collision.gameObject, out Coroutine damageCoroutine))
            {
                StopCoroutine(damageCoroutine);
                damageCoroutines.Remove(collision.gameObject);
            }
        }
    }

    private IEnumerator DamageEnemyOverTime(GameObject enemy)
    {
        EntityManager target = enemy.GetComponent<EntityManager>();

        while (activeEnemies.Contains(enemy))
        {
            target.TakeMeleeHit(damage, transform.position, knockbackForce, knockbackDuration, player);
            yield return new WaitForSeconds(damageInterval);
        }
    }
}
