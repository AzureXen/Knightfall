using System.Collections;
using UnityEngine;

public class Trap : MonoBehaviour
{
    public GameObject player;
    [SerializeField] public int attackDamage = 15;

    private bool isPlayerInside = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    // Check if the object that collided is the player
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        StartCoroutine(AttackPlayerForTrap(collision.gameObject));
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.gameObject;
            isPlayerInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInside = false;
            player = null; // Reset player reference
        }
    }

    private void AttackPlayerForTrap()
    {

        if (isPlayerInside && player != null)
        {
            EntityManager playerManager = player.GetComponent<EntityManager>();
            if (playerManager != null)
            {
                playerManager.TakeMeleeHit(attackDamage, transform.position, 0.5f, 0.5f, null);
            }
        }
    }
}
