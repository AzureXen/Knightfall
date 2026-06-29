using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerVitalityUI : MonoBehaviour
{
    public Image healthBar;
    public Image staminaBar;

    [SerializeField] private PlayerDataSO playerData;

    private void UpdateStamina(float Stamina)
    {
        staminaBar.fillAmount = Mathf.Clamp((float)playerData.Stamina / playerData.MaxStamina, 0, playerData.Stamina);
    }

    private void UpdateHealth(int Health)
    {
        healthBar.fillAmount = Mathf.Clamp((float)playerData.CurrentHealth / playerData.MaxHealth, 0, playerData.MaxHealth);
    }

    private void OnEnable()
    {
        if (GameManagerScript.Instance != null)
        {
            Subscribe();
        }
        else
        {
            GameManagerScript.OnReady += Subscribe;
        }
    }

    private void OnDisable()
    {
        GameManagerScript.OnReady -= Subscribe;
        if (GameManagerScript.Instance != null)
            GameManagerScript.Instance.playerData.OnStaminaChanged -= UpdateStamina;
        GameManagerScript.Instance.playerData.OnHealthChanged -= UpdateHealth;
    }

    private void Subscribe()
    {

        GameManagerScript.Instance.playerData.OnStaminaChanged += UpdateStamina;
        GameManagerScript.Instance.playerData.OnHealthChanged += UpdateHealth;
    }


}
