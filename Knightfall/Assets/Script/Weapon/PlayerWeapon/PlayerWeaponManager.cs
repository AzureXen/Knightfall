using System;
using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    private GameObject sword;
    private GameObject bow;
    private GameObject activeWeapon;

    public float weaponSwitchCooldown = 0.6f;
    public float weaponSwitchCoolDownTimer = 0;

    public Boolean canSwitch = true;
    void Start()
    {
        sword = transform.GetChild(0).gameObject;
        bow = transform.GetChild(1).gameObject;

        activeWeapon = bow;
        EquipWeapon(bow);
    }

    // Update is called once per frame
    void Update()
    {
        WeaponSwitch();
        if(weaponSwitchCoolDownTimer > 0)
        {
            weaponSwitchCoolDownTimer -= Time.deltaTime;
            weaponSwitchCoolDownTimer = Mathf.Clamp(weaponSwitchCoolDownTimer, 0, weaponSwitchCooldown);
        }
    }

    void WeaponSwitch()
    {
        if(Input.GetKeyDown(KeyCode.Q) && weaponSwitchCoolDownTimer<=0)
        {
            if(activeWeapon == bow)
            {
                EquipWeapon((sword));
            }else EquipWeapon((bow));
            weaponSwitchCoolDownTimer = weaponSwitchCooldown;
        }
    }

    void EquipWeapon(GameObject weapon)
    {
        sword.SetActive(false);
        bow.SetActive(false);

        weapon.SetActive(true);
        activeWeapon = weapon;
    }
}
