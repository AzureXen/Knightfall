using System;
using System.Collections;
using UnityEngine;

public class PlayerSword : MonoBehaviour
{
    public Boolean isAttacking = false;
    private float attackCooldownTimer;
    public GameObject hitbox;

    private PlayerDataSO playerData;


    private void Start()
    {
        playerData = GameManagerScript.Instance.playerData;
    }

    void Update()
    {
        AttackReg();

        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
            attackCooldownTimer = Mathf.Clamp(attackCooldownTimer, 0, playerData.MeleeAttackCooldown);
        }
    }
    void AttackReg()
    {
        if(Input.GetMouseButton(0) && !isAttacking && attackCooldownTimer<=0)
        {
            Debug.Log("Attack Registered, firing attack.");
            Attack();
        }
    }
    protected void Attack()
    {
        isAttacking = true;

    }

    // if during attack, the player switches their weapon, disable the hitbox
    private void OnDisable()
    {
        hitbox.SetActive(false);
        isAttacking=false;
        attackCooldownTimer = 0;
    }

    public void enableHitbox()
    {
        hitbox.SetActive(true);
    }
    public void disableHitbox()
    {
        hitbox.SetActive(false);
    }
    public void endAttack()
    {
        attackCooldownTimer = playerData.MeleeAttackCooldown;
        isAttacking = false;
    }
}
