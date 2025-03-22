using UnityEngine;

public class SkeletonAnimator : MonoBehaviour
{
    private SkeletonMovement skeletonMovement;
    private SkeletonAttack skeletonAttack;
    private SkeletonActions skeletonActions;
    Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        skeletonMovement = GetComponent<SkeletonMovement>();
        skeletonAttack = GetComponent<SkeletonAttack>();
        skeletonActions = GetComponent<SkeletonActions>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (skeletonActions.currentState == SkeletonActions.SkeletonState.IDLE && !skeletonAttack.IsAttacking && !skeletonAttack.Is2ndAttacking)
        {
            animator.SetBool("isWalking", false);
        }

        if (skeletonActions.currentState == SkeletonActions.SkeletonState.APPROACH && !skeletonAttack.IsAttacking && !skeletonAttack.Is2ndAttacking) 
        {
            animator.SetBool("isWalking", true);
            animator.SetFloat("walkSpeed", 1f);
        }
        if (skeletonActions.currentState == SkeletonActions.SkeletonState.CHASE && !skeletonAttack.IsAttacking && !skeletonAttack.Is2ndAttacking)
        {
            animator.SetBool("isWalking", true);
            animator.SetFloat("walkSpeed", 2f);
        }
        if (skeletonActions.currentState == SkeletonActions.SkeletonState.BACKOFF && !skeletonAttack.IsAttacking && !skeletonAttack.Is2ndAttacking)
        {
            animator.SetBool("isWalking", true);
            animator.SetFloat("walkSpeed", -0.8f);
        }
    }
}
