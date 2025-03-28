using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthBar : MonoBehaviour
{
    public Image redBar;                  // Thanh nền
    public Image greenBar;                // Thanh máu hiện tại
    public Health bossHealth;            // Script health của boss
    public TextMeshProUGUI bossNameText; // Tên boss
    public string bossName = "BOSS NAME";

    [Range(1f, 10f)] public float smoothSpeed = 5f;

    private float targetFill = 1f;
    private bool isHidden = true;         // Ẩn từ đầu
    private int lastHealth;
    public GameObject stageClearImage;
    void Start()
    {
        if (bossNameText != null)
            bossNameText.text = bossName;

        if (bossHealth != null)
        {
            targetFill = 1f;
            lastHealth = bossHealth.health;
        }

        SetUIVisible(false);
    }


    void Update()
    {
        if (bossHealth == null) return;

        // ✨ Nếu boss vừa bị mất máu → hiển thị thanh máu
        if (isHidden && bossHealth.health < lastHealth)
        {
            SetUIVisible(true);
            isHidden = false;
        }

        lastHealth = bossHealth.health;

        // Nếu boss chết → ẩn UI
        if (bossHealth.health <= 0 && !isHidden)
        {
            HideUI();
            isHidden = true;
            return;
        }

        // Cập nhật thanh máu mượt
        float currentPercent = (float)bossHealth.health / bossHealth.maxHealth;
        targetFill = Mathf.Clamp01(currentPercent);
        greenBar.fillAmount = Mathf.Lerp(greenBar.fillAmount, targetFill, Time.deltaTime * smoothSpeed);
    }

    private void SetUIVisible(bool visible)
    {
        redBar.gameObject.SetActive(visible);
        greenBar.gameObject.SetActive(visible);
        if (bossNameText != null)
            bossNameText.gameObject.SetActive(visible);
    }

    private void HideUI()
    {
        SetUIVisible(false);
        gameObject.SetActive(false); // Ẩn thanh máu boss

        // ✅ Hiện ảnh STAGE CLEAR
        if (stageClearImage != null)
        {
            Image img = stageClearImage.GetComponent<Image>();
            if (img != null)
            {
                Color c = img.color;
                c.a = 1f; // hiện ảnh
                img.color = c;
            }

        }

        Time.timeScale = 0f;
    }
}
