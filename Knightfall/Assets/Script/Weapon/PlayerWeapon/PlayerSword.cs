using System;
using System.Collections;
using UnityEngine;

public class PlayerSword : MonoBehaviour
{
    public int meleeDamage = 10;
    public float knockbackForce = 5f;
    public float knockbackDuratiton = 1.5f;

    public float attackDuration = 1f;
    private float attackTimer;
    public Boolean isAttacking = false;

    public float attackCooldown = 1f;
    private float attackCooldownTimer;

    public GameObject hitbox;


    void Update()
    {
        AttackReg();
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
            attackTimer = Mathf.Clamp(attackTimer,0, attackDuration);
        }

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
            StartCoroutine(Attack());
        }
    }
    protected IEnumerator Attack()
    {
        hitbox.SetActive(true);
        isAttacking = true;
        attackTimer = attackDuration;
        attackCooldownTimer = attackCooldown;
        Debug.Log("Attack Started! Duration: " + attackDuration + " seconds");
        while (attackTimer > 0)
        {
            yield return null;
        }
        hitbox.SetActive(false);
        isAttacking=false;
    }

    // if during attack, the player switches their weapon, disable the hitbox
    private void OnDisable()
    {
        hitbox.SetActive(false);
        isAttacking=false;
        attackCooldownTimer = 0;
    }
}
