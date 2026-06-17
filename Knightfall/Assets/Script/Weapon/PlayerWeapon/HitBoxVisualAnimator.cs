using System;
using UnityEngine;

public class HitBoxVisualAnimator : MonoBehaviour
{
    Animator am;
    [SerializeField] private PlayerSword ps;
    SpriteRenderer sr;

    private PlayerAnimator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        am = GetComponent<Animator>();
        if (ps == null) ps = FindFirstObjectByType<PlayerSword>();
        sr = GetComponent<SpriteRenderer>();

        animator = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAnimator>();
    }

    // Update is called once per frame
    void Update()
    {
        am.SetFloat("attackSpeedMultiplier", ps.attackSpeed);
        if (animator.isFlipped) { sr.flipY = true; }
        else { sr.flipY = false; }

        if (ps == null) { return; }


        am.SetBool("isAttacking", ps.isAttacking);
    }

    public void enableHitbox()
    {
        ps.enableHitbox();
    }
    public void disableHitbox()
    {
        ps.disableHitbox();
    }
    public void endAttack()
    {
        ps.endAttack();
    }
}
