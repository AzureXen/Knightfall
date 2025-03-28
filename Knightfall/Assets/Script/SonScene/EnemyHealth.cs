using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class EnemyHealth : Health  // Inherit from Health
{
    public Slider healthBar;  // Unique to EnemyHealth
    public GameObject enemyPopUpDamage;

    public GameObject heartPrefab;

    public event Action OnDeath;

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
            OnDeath?.Invoke();
            DropHeart();
            Destroy(gameObject);
        }
    }


    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);

        health = Mathf.Clamp(health, 0, maxHealth);

        if (healthBar != null)
        {
            healthBar.value = health;
        }


        if (enemyPopUpDamage != null)
        {
            GameObject popUpText = Instantiate(enemyPopUpDamage, transform.position, Quaternion.identity);
            popUpText.transform.localScale = Vector3.one * 2f;

            TextMeshPro damageDisplayMesh = popUpText.transform.GetChild(0).GetComponent<TextMeshPro>();
            damageDisplayMesh.text = amount.ToString();
            damageDisplayMesh.color = Color.red;
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
