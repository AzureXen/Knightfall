using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthBar : MonoBehaviour
{
    public Image redBar;              // Thanh nền (máu đầy)
    public Image greenBar;            // Thanh máu hiện tại (giảm)
    public Health bossHealth;         // Script health của boss
    public TextMeshProUGUI bossNameText;  // Text tên boss
    public string bossName = "BOSS NAME";

    [Range(1f, 10f)] public float smoothSpeed = 5f;

    private float targetFill = 1f;
    private bool isHidden = false;

    void Start()
    {
        if (bossHealth != null)
        {
            targetFill = 1f;

            if (bossNameText != null)
                bossNameText.text = bossName;
        }
    }

    void Update()
    {
        if (bossHealth == null) return;

        // Nếu boss chết, ẩn toàn bộ thanh máu
        if (bossHealth.health <= 0 && !isHidden)
        {
            HideUI();
            isHidden = true;
            return;
        }

        float currentPercent = (float)bossHealth.health / bossHealth.maxHealth;
        targetFill = Mathf.Clamp01(currentPercent);

        // Thanh máu giảm mượt
        greenBar.fillAmount = Mathf.Lerp(greenBar.fillAmount, targetFill, Time.deltaTime * smoothSpeed);
    }

    private void HideUI()
    {
        redBar.gameObject.SetActive(false);
        greenBar.gameObject.SetActive(false);

        if (bossNameText != null)
            bossNameText.gameObject.SetActive(false);

        gameObject.SetActive(false); // Ẩn cả khung nếu muốn
    }
}
