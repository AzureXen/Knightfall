using System;
using UnityEngine;

public class SkeletonHeadAnimator : MonoBehaviour
{
    private SkeletonHeadMovement skeletonMovement;
    private SkeletonHeadAttack skeletonAttack;
    private SkeletonHeadActions skeletonActions;
    Animator animator;

    private Boolean deceased = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        skeletonMovement = GetComponent<SkeletonHeadMovement>();
        skeletonAttack = GetComponent<SkeletonHeadAttack>();
        skeletonActions = GetComponent<SkeletonHeadActions>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!deceased) {
            if (skeletonActions.currentState == SkeletonHeadActions.SkeletonHeadState.DEAD)
            {
                animator.SetTrigger("Dead");
                deceased = true;
            }
        }
    }
}
