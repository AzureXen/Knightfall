using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    public Image healthBar;
    [SerializeField] private GameObject skullKnight;
    private UndeadBossHealth bossHealth;

    void Start()
    {
        bossHealth = skullKnight.GetComponent<UndeadBossHealth>();
    }

    private void Update()
    {
        if (bossHealth != null)
        {
            healthBar.fillAmount = Mathf.Clamp((float)bossHealth.health / bossHealth.maxHealth, 0, bossHealth.maxHealth);
        }
        else
        {
            healthBar.fillAmount = 0;
        }
    }
}
