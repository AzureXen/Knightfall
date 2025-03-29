using System;
using UnityEngine;

public class SkullKnightAnimator : MonoBehaviour
{
    private SkullKnightMovement skullKnightMovement;
    private SkullKnightAttack skullKnightAttack;
    private SkullKnightActions skullKnightActions;
    Animator animator;

    private Boolean deceased = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        skullKnightMovement = GetComponent<SkullKnightMovement>();
        skullKnightAttack = GetComponent<SkullKnightAttack>();
        skullKnightActions = GetComponent<SkullKnightActions>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!deceased) {
            if (skullKnightActions.currentState == SkullKnightActions.SkullKnightState.COMBATIDLE && !skullKnightAttack.IsAttacking && !skullKnightAttack.Is2ndAttacking)
            {
                animator.SetInteger("SwordDrawn", 2);
            }
            if (skullKnightActions.currentState == SkullKnightActions.SkullKnightState.CHASE && !skullKnightAttack.IsAttacking && !skullKnightAttack.Is2ndAttacking)
            {
                animator.SetBool("Walk", true);
            }
            if (skullKnightActions.currentState != SkullKnightActions.SkullKnightState.CHASE && skullKnightAttack.IsAttacking || skullKnightAttack.Is2ndAttacking)
            {
                animator.SetBool("Walk", false);
            }
            if (skullKnightActions.currentState == SkullKnightActions.SkullKnightState.DEAD)
            {
                animator.SetTrigger("Dead");
                deceased = true;
            }
        }
    }
}
