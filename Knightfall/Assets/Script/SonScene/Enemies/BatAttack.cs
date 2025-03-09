using UnityEngine;

public class BatAttack : MonoBehaviour
{
    public int damageAmount = 10; // Sát thương gây ra khi va chạm

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damageAmount);
        }
    }
}
