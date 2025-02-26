using System;
using UnityEngine;

public class HitBoxVisualAnimator : MonoBehaviour
{
    Animator am;
    [SerializeField] private PlayerSword ps;
    [SerializeField] private SwordAnimator swordAnimator;
    SpriteRenderer sr;

    private PlayerAnimator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        am = GetComponent<Animator>();
        if (ps == null) ps = FindFirstObjectByType<PlayerSword>();
        if (swordAnimator == null) swordAnimator = FindFirstObjectByType<SwordAnimator>();
        sr = GetComponent<SpriteRenderer>();

        animator = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAnimator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (animator.isFlipped) { sr.flipY = true; }
        else { sr.flipY = false; }

        if (ps == null) { return; }

        if (ps.isAttacking && swordAnimator.isSwinging)
        {
            am.SetBool("Swing", true);
        }
        else
        {
            am.SetBool("Swing", false);
        }
    }
}
