using System;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    Animator am;
    PlayerMovement pm;
    SpriteRenderer sr;

    public Boolean isFlipped;

    private Vector3 mousePos;
    private Camera mainCam;
    private Vector3 playerDirection;
    void Start()
    {
        am = GetComponent<Animator>();
        pm = GetComponent<PlayerMovement>();
        sr = GetComponent<SpriteRenderer>();

        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {

        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        playerDirection = mousePos - transform.position;
        SpriteDirectionCheck();
        if (pm.moveDir.x != 0 || pm.moveDir.y != 0)
        {
            am.SetBool("Move", true);
            //if(pm.moveDir.x != 0)
            //{
            //    SpriteDirectionCheck();
            //}
        }
        else
        {
            am.SetBool("Move", false);
            //if (pm.moveDir.x != 0)
            //{
            //    SpriteDirectionCheck();
            //}
        }

        if (pm.isRunning)
        {
            am.SetBool("Sprint", true);
        }
        else
        {
            am.SetBool("Sprint", false);
        }
    }

    void SpriteDirectionCheck()
    {

        if (playerDirection.x < 0)
        {
            sr.flipX = true;
        }
        else
        {
            sr.flipX = false;
        }
        isFlipped = sr.flipX;


        //if (pm.moveDir.x < 0)
        //{
        //    sr.flipX = true;
        //}
        //else
        //{
        //    sr.flipX = false;
        //}
        //isFlipped = sr.flipX;
    }
}
