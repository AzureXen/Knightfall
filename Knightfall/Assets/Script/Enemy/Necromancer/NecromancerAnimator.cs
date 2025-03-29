using System;
using UnityEngine;

public class NecromancerAnimator : MonoBehaviour
{
    private NecromancerMovement necromancerMovement;
    private NecromancerAttack necromancerAttack;
    private NecromancerActions necromancerActions;
    Animator animator;

    private Boolean deceased = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        necromancerMovement = GetComponent<NecromancerMovement>();
        necromancerAttack = GetComponent<NecromancerAttack>();
        necromancerActions = GetComponent<NecromancerActions>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!deceased)
        {
            if (necromancerActions.currentState == NecromancerActions.NecromancerState.IDLE && !necromancerAttack.NecroIsAttacking && !necromancerAttack.NecroIsSummoning)
            {
                animator.SetBool("isWalking", false);
            }

            if (necromancerActions.currentState == NecromancerActions.NecromancerState.APPROACH && !necromancerAttack.NecroIsAttacking && !necromancerAttack.NecroIsSummoning)
            {
                animator.SetBool("isWalking", true);
                animator.SetFloat("walkSpeed", 0.8f);
            }
            if (necromancerActions.currentState == NecromancerActions.NecromancerState.CHASE && !necromancerAttack.NecroIsAttacking && !necromancerAttack.NecroIsSummoning)
            {
                animator.SetBool("isWalking", true);
                animator.SetFloat("walkSpeed", 1f);
            }
            if (necromancerActions.currentState == NecromancerActions.NecromancerState.BACKOFF && !necromancerAttack.NecroIsAttacking && !necromancerAttack.NecroIsSummoning)
            {
                animator.SetBool("isWalking", true);
                animator.SetFloat("walkSpeed", -0.2f);
            }

            if (necromancerActions.currentState == NecromancerActions.NecromancerState.DEAD)
            {
                animator.SetTrigger("Dead");
                deceased = true;
            }
        }
    }
}
