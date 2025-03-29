using UnityEngine;

public class NhatBullet : MonoBehaviour
{
    public int damage = 15; // Sát thương của viên đạn
    public float lifetime = 3f; // Tự hủy sau 3 giây

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Đạn chạm vào Player!"); // Kiểm tra xem có chạm chưa

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
