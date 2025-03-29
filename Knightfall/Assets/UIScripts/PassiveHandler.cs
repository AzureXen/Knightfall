using System.Collections.Generic;
using UnityEngine;

public class PassiveHandler : MonoBehaviour
{
    [SerializeField] private GameObject DivinePassive;
    [SerializeField] private GameObject FlameShield;
    [SerializeField] private GameObject LightningSpeed;

    [SerializeField] private GameObject DivinePassiveIcon;
    [SerializeField] private GameObject FlameShieldIcon;
    [SerializeField] private GameObject LightningSpeedIcon;

    private Dictionary<string, bool> unlockedPassives = new Dictionary<string, bool>();

    private void Start()
    {
        unlockedPassives["DivinePulse"] = false;
        unlockedPassives["FlameRing"] = false;
        unlockedPassives["LightningBoost"] = false;
    }

    public bool UnlockPassive(string passiveName)
    {
        if (unlockedPassives.ContainsKey(passiveName))
        {
            unlockedPassives[passiveName] = true;
            Debug.Log("Unlocked Passive: " + passiveName);
            ActivatePassive(passiveName);
            return true;
        }
        return false;
    }

    private void ActivatePassive(string passiveName)
    {
        switch (passiveName)
        {
            case "DivinePulse":
                DivinePassiveIcon.SetActive(true);
                DivinePassive.SetActive(true);
                break;
            case "FlameRing":
                FlameShieldIcon.SetActive(true);
                FlameShield.SetActive(true);
                break;
            case "LightningBoost":
                LightningSpeedIcon.SetActive(true);
                LightningSpeed.SetActive(true);
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                PlayerMovement pm = player.GetComponent<PlayerMovement>();
                pm.speedBoost = 2;
                break;
        }
    }

    void DisableAll()
    {
        DivinePassive.SetActive(false);
        FlameShield.SetActive(false);
        LightningSpeed.SetActive(false);
    }
}
