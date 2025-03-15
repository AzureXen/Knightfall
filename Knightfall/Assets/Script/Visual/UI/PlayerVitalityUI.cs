using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerVitalityUI : MonoBehaviour
{
    public Image healthBar;
    public Image staminaBar;
    private GameObject player;
    private Health playerHealth;
    private PlayerMovement pm;

    private void Update()
    {
        if(playerHealth != null)
        {
            healthBar.fillAmount = Mathf.Clamp((float)playerHealth.health/playerHealth.maxHealth, 0, playerHealth.maxHealth);
            staminaBar.fillAmount = Mathf.Clamp((float)pm.stamina / pm.maxStamina, 0, pm.maxStamina);
        }
        else
        {
            healthBar.fillAmount=0;
            staminaBar.fillAmount=0;
        }
    }
    void Start()
    {
        StartCoroutine(FindPlayer());
    }

    private IEnumerator FindPlayer()
    {
        while (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerHealth = player.GetComponent<Health>();
                pm = player.GetComponent <PlayerMovement>();
                yield break; // Stop coroutine once the player is found
            }
            yield return new WaitForSeconds(0.5f); // Check every 0.5 seconds instead of every frame
        }
    }

}
