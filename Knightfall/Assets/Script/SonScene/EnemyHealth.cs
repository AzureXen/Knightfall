using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class EnemyHealth : Health  // Inherit from Health
{
    public Slider healthBar;  // Unique to EnemyHealth
    public GameObject enemyPopUpDamage;

    public GameObject heartPrefab;

    public override void Start()
    {
        base.Start(); // Call the base class Start method

        if (healthBar != null)
        {
            healthBar.minValue = 0;
            healthBar.maxValue = maxHealth;
            healthBar.value = health;
        }
    }

    // Override Update to handle enemy death
    public override void Update()
    {
        if (health <= 0)
        {
            DropHeart(); // Drop a heart before destroying the object
            Destroy(gameObject);
        }
    }


    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount); // Call the base class TakeDamage method

        // Ensure health does not go below 0
        health = Mathf.Clamp(health, 0, maxHealth);

        // Update health bar if available
        if (healthBar != null)
        {
            healthBar.value = health;
        }

        // 🛠️ Create a separate pop-up for EnemyHealth
        if (enemyPopUpDamage != null)
        {
            GameObject popUpText = Instantiate(enemyPopUpDamage, transform.position, Quaternion.identity);
            popUpText.transform.localScale = Vector3.one * 2f;  // Make it bigger

            TextMeshPro damageDisplayMesh = popUpText.transform.GetChild(0).GetComponent<TextMeshPro>();
            damageDisplayMesh.text = amount.ToString();
            damageDisplayMesh.color = Color.red; // Make it red for enemies
        }
    }

    private void DropHeart()
    {
        if (heartPrefab != null)
        {
            GameObject heart = Instantiate(heartPrefab, transform.position, Quaternion.identity);
            heart.transform.localScale = new Vector3(8f, 8f, 1f);
        }
    }
}
