using System;
using UnityEngine;

public class SwordAnimator : MonoBehaviour
{
    Animator am;
    [SerializeField] private PlayerSword ps;
    SpriteRenderer sr;

    private PlayerAnimator playerAnimator;

    public Boolean isSwinging = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerAnimator = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAnimator>();
        am = GetComponent<Animator>();
        if (ps == null) ps = FindFirstObjectByType<PlayerSword>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        am.SetFloat("attackSpeedMultiplier", ps.attackSpeed);
        if (ps == null) { return; }

        if (playerAnimator.isFlipped) { sr.flipY = true; }
        else { sr.flipY = false; }

        am.SetBool("isAttacking", ps.isAttacking);
    }


}
