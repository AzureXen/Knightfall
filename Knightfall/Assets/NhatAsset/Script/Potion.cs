using UnityEngine;

public class Potion : MonoBehaviour
{
    public int healAmount = 30; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            HealthGauge playerHealth = collision.GetComponent<HealthGauge>();
            if (playerHealth != null)
            {
                playerHealth.Heal(healAmount);
                Destroy(gameObject); 
            }
        }
    }
}
