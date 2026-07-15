using System;
using UnityEditor.UIElements;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatsAsset", menuName = "Game Data/Player Stats")]
public class PlayerDataSO : ScriptableObject
{

    [SerializeField] private int defaultHealth = 100;
    [SerializeField] private int defaultMaxHealth = 100;
    [SerializeField] private float defaultStamina = 120;
    [SerializeField] private float defaultMaxStamina = 120;
    [SerializeField] private float defaultGold = 0;
    [SerializeField] private int defaultMeleeDamage = 12;
    [SerializeField] private float defaultMeleeSpeed = 1.5f;
    [SerializeField] private float defaultMeleeKnockbackForce = 5;
    [SerializeField] private float defaultMeleeKnockbackDuration = 0.4f;
    [SerializeField] private float defaultMeleeAttackCooldown = 0.3f;

    

    [SerializeField] private int currentHealth = 100;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float stamina = 120;
    [SerializeField] private float maxStamina = 120;
    [SerializeField] private float gold;
    [SerializeField] private int meleeDamage = 12;
    [SerializeField] private float meleeSpeed = 1.5f;
    [SerializeField] private float meleeKnockbackForce = 5;
    [SerializeField] private float meleeKnockbackDuration = 0.4f;
    [SerializeField] private float meleeAttackCooldown = 0.3f;

    public event Action<float> OnGoldChanged;
    public event Action<int> OnHealthChanged;

    public event Action<float> OnStaminaChanged;
    public event Action<int> OnMaxHealthChanged;
    public event Action<float> OnMeleeStatsChanged;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public float Stamina => stamina;
    public float MaxStamina => maxStamina;
    public float Gold => gold;
    public int MeleeDamage => meleeDamage;
    public float MeleeSpeed => meleeSpeed;
    public float MeleeKnockbackForce => meleeKnockbackForce;
    public float MeleeKnockbackDuration => meleeKnockbackDuration;
    public float MeleeAttackCooldown => meleeAttackCooldown;



    public void AddGold(float amount)
    {
        gold += amount;
        OnGoldChanged?.Invoke(gold);
    }

    public void RemoveGold(float amount)
    {
        gold = Mathf.Max(0, gold - amount);
        OnGoldChanged?.Invoke(gold);
    }

    public void TakeDamage(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
    }
    public void SetStamina(float amount)
    {
        stamina = amount;
        OnStaminaChanged?.Invoke(stamina);
    }
    public void DecreaseStamina(float amount)
    {
        stamina -= amount;
        OnStaminaChanged?.Invoke(stamina);
    }
    public void IncreaseStamina(float amount)
    {
        stamina += amount;
        OnStaminaChanged?.Invoke(stamina);
    }

    public void IncreaseMaxStamina(float amount)
    {
        maxStamina += amount;
    }

    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth += amount;
        OnMaxHealthChanged?.Invoke(maxHealth);
    }


    public void SetMeleeDamage(int amount)
    {
        meleeDamage = amount;
        OnMeleeStatsChanged?.Invoke(meleeDamage);
    }
    public void IncreaseMeleeDamage(int amount)
    {
        meleeDamage += amount;
        OnMeleeStatsChanged?.Invoke(meleeDamage);
    }

    public void SetMeleeSpeed(float amount)
    {
        meleeSpeed = amount;
        OnMeleeStatsChanged?.Invoke(meleeSpeed);
    }

    public void SetMeleeKnockbackForce(float amount)
    {
        meleeKnockbackForce = amount;
    }

    public void SetMeleeKnockbackDuration(float amount)
    {
        meleeKnockbackDuration = amount;
    }

    public void SetMeleeAttackCooldown(float value)
    {
        meleeAttackCooldown = Mathf.Max(0.1f, value); 
    }

    public void ResetToDefaults()
    {
        currentHealth = defaultHealth;
        maxHealth = defaultMaxHealth;
        stamina = defaultStamina;
        maxStamina = defaultMaxStamina;
        gold = defaultGold;
        meleeDamage = defaultMeleeDamage;
        meleeSpeed = defaultMeleeSpeed;
        meleeKnockbackForce = defaultMeleeKnockbackForce;
        meleeKnockbackDuration = defaultMeleeKnockbackDuration;
        meleeAttackCooldown = defaultMeleeAttackCooldown;
    }   

}
