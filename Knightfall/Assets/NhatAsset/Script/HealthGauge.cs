using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthGauge : MonoBehaviour
{
    public int maxHealth = 100;
    public int health = 100;

 
    public Slider healthBar;

    private SpriteRenderer sr;
    private Color defaultColor;
    private Coroutine flashCoroutine;
    public float flashDuration = 0.2f;

 
    public GameObject popUpDamage;
    private GameObject popUpText;

    void Start()
    {
        health = maxHealth;
        if (healthBar != null)
        {
            healthBar.minValue = 0;   
            healthBar.maxValue = maxHealth;
            healthBar.value = health;
        }

        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            defaultColor = sr.color; 
        }
    }

    private void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (popUpText != null)
        {
            popUpText.transform.position = transform.position;
        }
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        health = Mathf.Clamp(health, 0, maxHealth);

        if (healthBar != null)
        {
            healthBar.value = health;
        }

        if (sr != null)
        {
            if (flashCoroutine != null) StopCoroutine(flashCoroutine);
            flashCoroutine = StartCoroutine(FlashRed());
        }


    }

    protected IEnumerator FlashRed()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(flashDuration); ;

        float timer = 0f;
        while (timer < flashDuration)
        {
            timer += Time.deltaTime;
            sr.color = Color.Lerp(Color.red, defaultColor, timer / flashDuration);
            yield return null;
        }

        sr.color = defaultColor;
        flashCoroutine = null;
    }

    public void Heal(int amount)
    {
        health += amount;
        health = Mathf.Clamp(health, 0, maxHealth);

        if (healthBar != null)
        {
            healthBar.value = health;
        }
    }

}