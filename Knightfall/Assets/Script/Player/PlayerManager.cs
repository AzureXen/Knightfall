using System.Collections;
using UnityEngine;

public class PlayerManager : EntityManager
{

    public static PlayerManager Instance { get; private set; }

    Animator am;

    [SerializeField] private PlayerDataSO playerData;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public override void Start()
    {
        base.Start();
        entityMovement = GetComponent<PlayerMovement>();
        am = GetComponent<Animator>();
        EntityName = "Player";
    }

    protected override IEnumerator FlashRed()
    {
        am.SetTrigger("Hurt");
        yield return new WaitForSeconds(0.7f);
    }

    public void StartFlashing()
    {
        if (flashCoroutine == null)
        {
            flashCoroutine = StartCoroutine(base.FlashRed());
        }
    }

    public void FlashGreenTwice()
    {
        StartCoroutine(FlashGreenCoroutine());
    }

    private IEnumerator FlashGreenCoroutine()
    {
        Color flashColor = new Color(0f, 1f, 0f); 
        float flashDuration = 0.1f; 

        for (int i = 0; i < 2; i++) 
        {
            sr.color = flashColor; 
            yield return new WaitForSeconds(flashDuration);
            sr.color = defaultColor; 
            yield return new WaitForSeconds(flashDuration); 
        }
    }

    // ======================================== UPGRADES ======================================== //
    public void IncreaseMaxHealth(int health)
    {
        entityHealth.maxHealth += health;
        entityHealth.health += health;
    }

    public void FullHeal()
    {
        entityHealth.health += entityHealth.maxHealth;
    }

    public void IncreaseMaxStamina(float stamina)
    {
        PlayerMovement playerMovement = entityMovement as PlayerMovement;
        if (playerMovement != null) {
            playerData.IncreaseMaxStamina(stamina);
        }

    }

    public void IncreaseMeleeDamage(int damage)
    {
        playerData.IncreaseMeleeDamage(damage);
    }

    protected override void TakeDamage(int damage)
    {
        entityHealth.TakeDamage(damage);
        if (damage > 0)
        {
            if (flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine);
            }
            flashCoroutine = StartCoroutine(FlashRed());
            TakeIFrame(takeDamageCooldown);
        }
    }
}
