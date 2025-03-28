using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    public Health bossHealth;
    public Slider healthSlider;
    public Image fillImage;  // Drag Fill here in Inspector

    public Color normalColor = Color.red;
    public Color damageColor = new Color(1f, 0.5f, 0.5f); // sáng hơn khi mất máu

    public float colorFadeSpeed = 5f; // tốc độ trở lại màu thường

    private float currentHealth;
    private float targetHealth;

    void Start()
    {
        healthSlider.maxValue = bossHealth.maxHealth;
        healthSlider.value = bossHealth.health;

        currentHealth = bossHealth.health;
        targetHealth = bossHealth.health;
        fillImage.color = normalColor;
    }

    void Update()
    {
        if (bossHealth == null) return;

        targetHealth = bossHealth.health;

        // Hiển thị tụt máu tức thì
        healthSlider.value = targetHealth;

        // Nếu máu tụt → đổi màu sáng lên
        if (currentHealth > targetHealth)
        {
            fillImage.color = damageColor;
        }

        // Fade màu từ damage → normal
        fillImage.color = Color.Lerp(fillImage.color, normalColor, Time.deltaTime * colorFadeSpeed);

        currentHealth = targetHealth;
    }
}
