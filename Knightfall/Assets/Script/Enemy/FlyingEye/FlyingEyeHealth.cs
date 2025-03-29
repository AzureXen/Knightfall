using UnityEngine;

public class FlyingEyeHealth : Health
{
    private float healChance = 0.3f;
    private int healAmount = 20;

    public override void Update()
    {
        if (health <= 0)
        {
            TryHealPlayer();
            Destroy(gameObject);
        }
    }

    private void TryHealPlayer()
    {
        float rand = UnityEngine.Random.value;
        if (rand <= healChance)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Health playerHealth = player.GetComponent<Health>();
                playerHealth.health += healAmount;
                Debug.Log($"Player healed by {healAmount} HP!");
            }
        }
    }
}
