using UnityEngine;

public class NhatBullet : MonoBehaviour
{
    public int damage = 15; 
    public float lifetime = 3f; 

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Đạn chạm vào Player!"); 

            Health playerHealth = collision.GetComponent<Health>();
            if (playerHealth != null)
            {
                Debug.Log("Player nhận sát thương: " + damage);
                playerHealth.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}
