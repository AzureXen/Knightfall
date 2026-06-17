using System;
using System.Collections;
using UnityEngine;

public class PlayerSword : MonoBehaviour
{
    public float attackSpeed = 1f;
    public int meleeDamage = 10;
    public float knockbackForce = 5f;
    public float knockbackDuration = 1.5f;


    public Boolean isAttacking = false;

    public float attackCooldown = 0.5f;
    private float attackCooldownTimer;

    public GameObject hitbox;

    void Update()
    {
        AttackReg();

        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
            attackCooldownTimer = Mathf.Clamp(attackCooldownTimer, 0, attackCooldown);
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
        Debug.Log("Disabling hitbox onDisable");
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
        Debug.Log("Disabling hitbox");
        hitbox.SetActive(false);
    }
    public void endAttack()
    {
        attackCooldownTimer = attackCooldown;
        isAttacking = false;
    }
}
