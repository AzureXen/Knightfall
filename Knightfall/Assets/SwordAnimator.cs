using System;
using UnityEngine;

public class SwordAnimator : MonoBehaviour
{
    Animator am;
    [SerializeField] private PlayerSword ps;
    SpriteRenderer sr;

    private PlayerDataSO playerData;

    private PlayerAnimator playerAnimator;

    public Boolean isSwinging = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerData = GameManagerScript.Instance.playerData;
        playerAnimator = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAnimator>();
        am = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        am.SetFloat("attackSpeedMultiplier", playerData.MeleeSpeed);


        if (playerAnimator.isFlipped) { sr.flipY = true; }
        else { sr.flipY = false; }

        am.SetBool("isAttacking", ps.isAttacking);
    }


}
