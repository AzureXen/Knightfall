using UnityEngine;

public class MonsterAttack : MonoBehaviour
{
    public int damageAmount = 10;

    private void OnTriggerEnter2D(Collider2D other)
    {
        HealthGauge playerHealth = other.GetComponent<HealthGauge>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damageAmount);
        }
    }
}