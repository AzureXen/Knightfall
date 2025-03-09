using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int health = 100;

    // UI Health Bar
    public Slider healthBar; // Thêm biến Slider

    private SpriteRenderer sr;
    private Color defaultColor;
    private Coroutine flashCoroutine;
    public float flashDuration = 0.2f;

    // used to instantiate
    public GameObject popUpDamage;
    private GameObject popUpText;

    void Start()
    {
        health = maxHealth;
        if (healthBar != null)
        {
            healthBar.minValue = 0;      // Đảm bảo minValue là 0
            healthBar.maxValue = maxHealth;
            healthBar.value = health;
        }

        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            defaultColor = sr.color; // Store default color
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

        // Cập nhật thanh máu
        if (healthBar != null)
        {
            healthBar.value = health;
        }

        if (sr != null)
        {
            if (flashCoroutine != null) StopCoroutine(flashCoroutine);
            flashCoroutine = StartCoroutine(FlashRed());
        }

        //Hiển thị số damage
        popUpText = Instantiate(popUpDamage, transform.position, Quaternion.identity);
        TextMeshPro damageDisplayMesh = popUpText.transform.GetChild(0).GetComponent<TextMeshPro>();
        //TextMeshProUGUI damageDisplayMesh = popUpText.GetComponent<TextMeshProUGUI>();
        
        damageDisplayMesh.text = amount.ToString();

        if (amount == 0)
        {
            damageDisplayMesh.color = Color.blue;
        }

        //Destroy(popUpText, 0.5f); // Xóa text sau 0.5 giây


        //popUpText = Instantiate(popUpDamage, transform.position, Quaternion.identity);

        //if (popUpText == null)
        //{
        //    Debug.LogError("Failed to instantiate popUpDamage!");
        //    return;
        //}

        //TextMeshPro damageDisplayMesh = popUpText.transform.GetChild(0).GetComponent<TextMeshPro>();
        //TextMeshPro damageDisplayMesh = popUpText.GetComponent<TextMeshPro>();


        //if (damageDisplayMesh == null)
        //{
        //    Debug.LogError("No TextMeshPro component found on popUpDamage child!");
        //    return;
        //}

        //damageDisplayMesh.text = amount.ToString();

        //if (amount == 0)
        //{
        //    damageDisplayMesh.color = Color.blue;
        //}
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
}
